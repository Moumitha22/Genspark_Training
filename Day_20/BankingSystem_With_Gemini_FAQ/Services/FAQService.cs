using System.Text;
using System.Text.Json;
using BankManagementSystem.Configuration;
using Microsoft.Extensions.Options;

namespace BankManagementSystem.Services
{
    public class FAQService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public FAQService(HttpClient httpClient, IOptions<GeminiSettings> options)
        {
            _httpClient = httpClient;
            _apiKey = options.Value.ApiKey;
        }

        private async Task<string> GetResponseAsync(string userPrompt)
        {

            var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash-001:generateContent?key={_apiKey}";


            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = userPrompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode){
                if ((int)response.StatusCode == 503)
                {
                    throw new ApplicationException("The Gemini model is currently overloaded. Please try again in a few moments.");
                }

                Console.WriteLine("Gemini error response body:\n" + responseString);
                throw new Exception($"Gemini API Error: {response.StatusCode} - {responseString}");
            }

            using var jsonDoc = JsonDocument.Parse(responseString);

            var textResponse = jsonDoc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return textResponse;
        }

        public async Task<string> GetBankingAnswerAsync(string question)
        {
            try
            {
                string faqContext = """
                Welcome to our Bank. Here’s what we offer:

                - **Account Types**: Savings account (including Senior Citizen & Student variants), Current/Checking account.
                - **Loans**: Personal, education, auto, home loans — all come with competitive flexible EMIs and interest rates.
                - **Credit Cards**: Cashback, travel rewards, business, and low-interest credit cards.
                - **Digital Banking**: Internet banking, mobile banking app (available on iOS & Android), 24/7 chatbot assistant, and UPI services.
                - **Account Opening**: Open an account online or by visiting any branch. KYC is mandatory for verification.
                - **Security**: 2FA authentication, encrypted transactions, and fraud monitoring.
                - **Customer Support**: 24/7 toll-free helpline, live chat, email assistance and branch assistance during working hours.
                - **Promotions & Offers**: Festival loan deals, 0% EMI on select cards, and salary advance programs.

                """;

                string userPrompt = $"""
                You are an intelligent banking assistant. Use only the provided context below to answer the user's question accurately and clearly. 
                If the question is unrelated to the context, politely inform the user that you can only answer questions related to the bank's services listed.

                Context:
                {faqContext}

                Question: {question}
                Answer:
                """;

                return await GetResponseAsync(userPrompt);
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException("Failed to connect to Gemini API.", ex);
            }
            catch (JsonException ex)
            {
                throw new ApplicationException("Failed to parse Gemini response.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An unexpected error occurred while processing your request.", ex);
            }
        }

        public async Task<string> ListModelsAsync()
        {
            var url = $"https://generativelanguage.googleapis.com/v1/models?key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Gemini API Error: {response.StatusCode} - {responseString}");
            }

            return responseString;
        }
    }
}


