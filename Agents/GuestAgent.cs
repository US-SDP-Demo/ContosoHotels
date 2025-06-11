using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using System.Reflection;

namespace ContosoHotels.Agents;

public class GuestAgent
{
  private readonly Kernel _kernel;

  public GuestAgent(Kernel kernel)
  {
    _kernel = kernel;
  }

  public async Task<string> GetHotelInfoAsync(string hotelName)
  {
    // Create the prompt function from the YAML resource
    var templateFactory = new HandlebarsPromptTemplateFactory();

    using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("YourNamespace.orchestrator.yaml");
    using var reader = new StreamReader(stream);
    var handlebarsPromptYaml = await reader.ReadToEndAsync(); var function = _kernel.CreateFunctionFromPromptYaml(handlebarsPromptYaml, templateFactory);

    // Input data for the prompt rendering and execution
    var arguments = new KernelArguments()
      {
          { "guest_name", "John Doe"},
          { "guest_id", "12345" },
          { "history", new[]
              {
                  new { role = "user", content = "What ameneties are available in this hotel?" },
              }
          },
      };

    // Invoke the prompt function
    var response = await _kernel.InvokeAsync(function, arguments);

    return response.ToString();
  }
}