using Azure.Identity;
using ContosoHotels.Agents;
using Microsoft.SemanticKernel;

namespace ContosoHotels.Services;

public class KernelFactory
{
    private readonly IConfiguration _configuration;
    
    
    public KernelFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    /// <summary>
    /// Creates a base kernel with common configuration (Azure OpenAI connection)
    /// </summary>
    private IKernelBuilder CreateBaseKernel()
    {
        var deploymentName = _configuration["AzureOpenAI:DeploymentName"];
        var endpoint = _configuration["AzureOpenAI:Endpoint"];
        
        if (string.IsNullOrEmpty(deploymentName) || string.IsNullOrEmpty(endpoint))
        {
            throw new InvalidOperationException("Azure OpenAI configuration is missing");
        }
        
        return Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endpoint,
                credentials: new DefaultAzureCredential());
    }
    
    public Kernel CreateBaseConfiguredKernel()
    {
        return CreateBaseKernel().Build();
    }
}