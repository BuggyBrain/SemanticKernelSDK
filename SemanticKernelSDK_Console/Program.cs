using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;

namespace SemanticKernelSDK;

class Program
{

    /// <summary>
    ///  Main, entry point. Converted from static to async. 
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static async Task Main(string[] args)
    {
        
        // See https://aka.ms/new-console-template for more information
        Console.WriteLine("Hello, World Of Semantic Kernel SDK!");

        /// Reading config file. To update it from.dev.json to .json
        var configuration = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("configuration.dev.json", optional: true, reloadOnChange: true)
                          .Build();


        /// Config to connect to Azure Open AI 
        var deploymentName = configuration.GetValue<string>("deploymentName");
        var endpoint = configuration.GetValue<string>("endpoint");
        var apiKey = configuration.GetValue<string>("apiKey");
        var modelId = configuration.GetValue<string>("modelId");


        /// Kernel builder
        var builder = Kernel.CreateBuilder();

        builder.AddAzureOpenAIChatCompletion(
                                            deploymentName,
                                            endpoint,
                                            apiKey,
                                            modelId);
        var kernel = builder.Build();


        /// Passing promt for response form LLM Model 
        var result = await kernel.InvokePromptAsync("Which is the smallest island ? ");
        Console.WriteLine(result);

    }
}