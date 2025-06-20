using CommunityToolkit.Diagnostics;
using ContosoHotels.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.Handoff;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using System.Reflection;

namespace ContosoHotels.Agents;

public class OrchestratorService
{
  private readonly KernelFactory _kernelFactory;
  private readonly HousekeepingTools _housekeepingTools;
  private readonly RoomServiceTools _roomServiceTools;
  private readonly ILoggerFactory _loggerFactory;

  public OrchestratorService(
    KernelFactory kernelFactory,
    HousekeepingTools housekeepingTools,
    RoomServiceTools roomServiceTools,
    ILoggerFactory loggerFactory)
  {
    Guard.IsNotNull(housekeepingTools);
    _housekeepingTools = housekeepingTools;

    Guard.IsNotNull(roomServiceTools);
    _roomServiceTools = roomServiceTools;

    Guard.IsNotNull(kernelFactory);
    _kernelFactory = kernelFactory;

    Guard.IsNotNull(loggerFactory);
    _loggerFactory = loggerFactory;
  }

  public async Task<string> HandleGuestQueryAsync(int guestId, string guestName, string guestQuery)
  {

    var guestKernelArguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
    {
      { "guest_id", guestId },
      { "guest_name", guestName }
    };

    // Create the orchestrator agent
    var orchestratorKernel = _kernelFactory.CreateBaseConfiguredKernel();
    var orchestratorAgent = await CreateAgentFromKernelTemplateYamlResource(
      orchestratorKernel,
      guestKernelArguments,
      "orchestrator.yaml");

    var housekeepingKernel = _kernelFactory.CreateBaseConfiguredKernel();
    housekeepingKernel.Plugins.Add(KernelPluginFactory.CreateFromObject(_housekeepingTools));

    var housekeepingAgent = await CreateAgentFromKernelTemplateYamlResource(
      housekeepingKernel,
      guestKernelArguments,
      "housekeeping.yaml");

    var roomServiceKernel = _kernelFactory.CreateBaseConfiguredKernel();
    roomServiceKernel.Plugins.Add(KernelPluginFactory.CreateFromObject(_roomServiceTools));
    var roomServiceAgent = await CreateAgentFromKernelTemplateYamlResource(
      roomServiceKernel,
      guestKernelArguments,
      "roomservice.yaml");


#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    var handoffs = OrchestrationHandoffs
      .StartWith(orchestratorAgent)
      .Add(orchestratorAgent, roomServiceAgent, housekeepingAgent)
      .Add(housekeepingAgent, orchestratorAgent, "Transfer to this agent if the issue is not related to housekeeping.")
      .Add(roomServiceAgent, orchestratorAgent, "Transfer to this agent if the issue is not related to room service.");

    HandoffOrchestration orchestration = new(
      handoffs,
      orchestratorAgent,
      housekeepingAgent,
      roomServiceAgent)
    {
      ResponseCallback = LoggingResponseCallback,
      LoggerFactory = _loggerFactory
    };

    InProcessRuntime runtime = new();
    await runtime.StartAsync();

    OrchestrationResult<string> result = await orchestration.InvokeAsync(guestQuery, runtime);

    var output = await result.GetValueAsync();

#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    return output;
  }

  private async Task<ChatCompletionAgent> CreateAgentFromKernelTemplateYamlResource(Kernel kernel, KernelArguments kernelArguments, string yamlResourceName)
  {
    var templateFactory = new KernelPromptTemplateFactory();

    using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"ContosoHotels.Agents.Templates.{yamlResourceName}");

    if (stream == null)
    {
      throw new InvalidOperationException($"Resource '{yamlResourceName}' not found in assembly.");
    }

    using var reader = new StreamReader(stream);
    var kernelTemplatePromptYaml = await reader.ReadToEndAsync();
    var templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(kernelTemplatePromptYaml);

    var agent = new ChatCompletionAgent(templateConfig, templateFactory)
    {
      Kernel = kernel,
      Arguments = kernelArguments
    };

    return agent;
  }

  private ValueTask LoggingResponseCallback(ChatMessageContent response)
  {
    var logger = _loggerFactory.CreateLogger<OrchestratorService>();
    logger.LogInformation("Agent response: {Response}", response.Content);
    return ValueTask.CompletedTask;
  }
}