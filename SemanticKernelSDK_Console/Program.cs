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

        Console.WriteLine("Enter 1,2,3,4 for Topic 1, 2, 3, or 4 respectively.");
        var topicToRun = Console.ReadLine();


        switch (topicToRun)
        {
            case "1":
                Console.WriteLine($"In Topic 1.");
                await Topic1(deploymentName, endpoint, apiKey, modelId);
                break;
            case "2":
                Console.WriteLine($"In Topic 2.");
                await Topic2(deploymentName, endpoint, apiKey, modelId);
                break;
            case "3":
            case "4":
            default:
                Console.WriteLine($"In Default.");
                break;
        }

    }

    private static async Task Topic1(string? deploymentName, string? endpoint, string? apiKey, string? modelId)
    {
        /****************** TOPIC 1 - Semantic Kernel Builder *****************************/
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

    private static async Task Topic2(string? deploymentName,
                                     string? endpoint,
                                     string? apiKey,
                                     string? modelId)
    {
        /****************** TOPIC 2 - Plugins for Semantic Kernel *****************************/

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

        ///// Topic 2 - Using built-in plugins
        /// Adding Time Plugin to Kernel 
        builder2.Plugins.AddFromType<TimePlugin>();
        var kernel2 = builder2.Build();

        var currentDay = await kernel2.InvokeAsync("TimePlugin", "DayOfWeek");
        Console.WriteLine("Topic 2 Time Plugin says, Today is  Awesome {{$currentDay}}");


        /// Adding Conversation Summary Plugin to Kernel 

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

        ///// Topic 2 - Optimizing Language Model Prompt 
        string history = @"In the heart of my bustling kitchen, I have embraced 
                        the challenge of satisfying my family's diverse taste buds and 
                        navigating their unique tastes. With a mix of picky eaters and 
                        allergies, my culinary journey revolves around exploring a plethora 
                        of vegetarian recipes.";

        // One of my kids is a picky eater with an aversion to anything green, 
        // while another has a peanut allergy that adds an extra layer of complexity 
        // to meal planning. Armed with creativity and a passion for wholesome 
        // cooking, I've embarked on a flavorful adventure, discovering plant-based 
        // dishes that not only please the picky palates but are also heathy and 
        // delicious.";

        string prompt = @"This is some information about the user's background: 
                        {{$history}}
                        Given this user's background, provide a list of relevant recipes.";

        result2 = await kernel2.InvokePromptAsync(prompt,
            new KernelArguments() { { "history", history } });

        Console.WriteLine(result2);
    }
}