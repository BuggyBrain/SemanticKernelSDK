#pragma warning disable SKEXP0050 

using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;

using Microsoft.SemanticKernel.Plugins.Core;

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


        // /// Kernel builder - First Topic 
        // var builder = Kernel.CreateBuilder();

        // builder.AddAzureOpenAIChatCompletion(
        //                                     deploymentName,
        //                                     endpoint,
        //                                     apiKey,
        //                                     modelId);
        // var kernel = builder.Build();


        // /// Passing promt for response form LLM Model 
        // var result = await kernel.InvokePromptAsync("Which is the smallest island ? ");
        // Console.WriteLine(result);



        /// Plugins for Semantic Kernel- Topic Two∏∏ 

        /// Plugins available in Microsoft.SemanticKernel.Plugins.Core 1.2.0-alpha 
        /// ConversationSummaryPlugin - Summarizes conversation
        /// FileIOPlugin - Reads and writes to the filesystem
        /// HttpPlugin - Makes requests to HTTP endpoints
        /// MathPlugin - Performs mathematical operations
        /// TextPlugin - Performs text manipulation
        /// TimePlugin - Gets time and date information∏
        /// WaitPlugin - Pauses execution for a specified amount of time

        /// Builder2 for Plugins 
        var builder2 = Kernel.CreateBuilder();
        builder2.AddAzureOpenAIChatCompletion(
                                            deploymentName,
                                            endpoint,
                                            apiKey,
                                            modelId);

        /// Adding Time Plugin to Kernel 
        // builder2.Plugins.AddFromType<TimePlugin>();
        var kernel2 = builder2.Build();

        // var currentDay = await kernel2.InvokeAsync("TimePlugin", "DayOfWeek");
        // Console.WriteLine(currentDay);


        /// Adding Conversation SUmmary Plugin to Kernel 
        
        //// THERE IS AN EXISTING ISSUE IN THE PLUGIN WHERE NO ACTION ITEMS ARE RETURNED EVEN AFTER TRYING MULTIPLE INPUTS
        //// RESPONSE IS 
        //// {
        //// "actionItems": []
        //// }
        //// GITHUB ISSUE https://github.com/microsoft/semantic-kernel/issues/4843
        

        builder2.Plugins.AddFromType<ConversationSummaryPlugin>();
        kernel2 = builder2.Build();

        string input = @"I'm a vegan in search of new recipes. I love spicy food! 
                        Can you give me a list of breakfast recipes that are vegan friendly?";

        var result2 = await kernel2.InvokeAsync(
            "ConversationSummaryPlugin",
            "GetConversationActionItems",
            new() { { "input", input } });

        Console.WriteLine(result2);

    }
}