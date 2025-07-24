using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using TodoAppELK.Models.Domain;
using TodoAppELK.Services.Abstract;

namespace TodoAppELK.Services.Concrete
{
    public class OpenAIService : IOpenAIService
    {
        // This is a placeholder for the actual OpenAI API client.
        private readonly ChatClient _openAIClient;
        private readonly string _model;
        private readonly ILogger<OpenAIService> _logger;

        // Constructor to initialize the OpenAI client and model. ( Dependency Injection )
        public OpenAIService(IConfiguration configuration, ILogger<OpenAIService> logger)
        {
            var baseUrl = configuration["OpenAI:BaseUrl"];
            var apiKey = configuration["OpenAI:ApiKey"];
            _model = configuration["OpenAI:Model"];
            _logger = logger;

            // Düzeltilmiş client oluşturma
            //_openAIClient = new OpenAI.OpenAIClient(
            //    apiKey: apiKey,
            //    options: new OpenAIClientOptions()
            //    {
            //        Endpoint = new Uri(baseUrl)
            //    }
            //);

            _openAIClient = new(
                model: _model,
                credential: new ApiKeyCredential(apiKey),
                options: new OpenAIClientOptions()
                {
                    Endpoint = new Uri(baseUrl)
                }
            );

            
        }

        public async Task<string> AnalyzeTodoListAsync(List<Todo> todos)
        {

            try
            {

                var todoList = String.Join("\n", todos.Select(t => $"- {t.Title} {(t.IsCompleted ? "Completed" : "Pending")})"));
                var response = await _openAIClient.CompleteChatAsync(
                    new SystemChatMessage("You are a productivity coach. Analyze the todo list and provide insights about productivity patterns, suggestions for improvement, and motivational advice."),
                    new UserChatMessage($"Analyze this todo list:\n{todoList}")
                );

                _logger.LogInformation("Todo list analysis completed for {TodoCount} items", todos.Count);
                return response.Value.Content[0].Text;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error analyzing todo list");
                return "Unable to analyze todo list at this time.";
            }
        }

        public async Task<string> GenerateMotivationalMessageAsync(int completedCount, int totalCount)
        {
            try
            {
                var completionRate = totalCount > 0 ? (double)completedCount / totalCount * 100 : 0;

                var response = await _openAIClient.CompleteChatAsync(
                    new SystemChatMessage("You are a motivational coach. Generate encouraging messages based on task completion rates."),
                    new UserChatMessage($"Generate a motivational message for someone who completed {completedCount} out of {totalCount} tasks ({completionRate:F1}% completion rate)")
                );

                _logger.LogInformation("Motivational message generated for completion rate: {CompletionRate}%", completionRate);
                return response.Value.Content[0].Text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating motivational message");
                return "Keep up the great work!";
            }
        }

        public async Task<string> GenerateTodoSuggestAsync(string userInput)
        {
            try
            {
                var response = await _openAIClient.CompleteChatAsync(
                    new SystemChatMessage("You are a helpful assistant that generates practical todo items based on user input. Keep responses concise and actionable."),
                    new UserChatMessage($"Generate 3-5 practical todo items based on this input: {userInput}")
                );

                _logger.LogInformation("AI suggestion generated for input: {UserInput}", userInput);
                return response.Value.Content[0].Text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating todo suggestion");
                return "Unable to generate suggestions at this time.";
            }
        }
    }
}
