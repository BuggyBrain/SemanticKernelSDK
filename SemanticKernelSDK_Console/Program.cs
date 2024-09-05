using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;

namespace SemanticKernelSDK;

class Program
{

    static async Task  Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        // See https://aka.ms/new-console-template for more information
        Console.WriteLine("Hello, World Of Semantic Kernel SDK!");


        // var configuration = new ConfigurationBuilder()
        //                     .AddInMemoryCollection(new Dictionary<string, string?>()
        //                     {
        //                         ["SomeKey"] = "SomeValue"
        //                     })
        //                     .Build();
       
          var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("configuration.dev.json", optional: true, reloadOnChange: true)
                            .Build();
                            

        var deploymentName = configuration.GetValue<string>("deploymentName");
        var endpoint = configuration.GetValue<string>("endpoint");
        var apiKey = configuration.GetValue<string>("apiKey");
        var modelId = configuration.GetValue<string>("modelId");
       
        var builder = Kernel.CreateBuilder();
        
        builder.AddAzureOpenAIChatCompletion(
                                            deploymentName,
                                            endpoint,
                                            apiKey,
                                            modelId);
        var kernel = builder.Build();
      
        var result = await kernel.InvokePromptAsync("Give me a list of breakfast foods with eggs and cheese");
        Console.WriteLine(result);

    }
}