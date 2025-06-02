using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Banking_FAQ.Models;

[ApiController]
[Route("api/[controller]")]
public class FaqProxyController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public FaqProxyController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpPost("ask")]
    public async Task<IActionResult> AskQuestion([FromBody] QuestionRequest questionRequest)
    {
        if (string.IsNullOrWhiteSpace(questionRequest?.Question))
            return BadRequest("Question cannot be empty.");

        var json = JsonSerializer.Serialize(questionRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("http://127.0.0.1:8000/ask", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, $"FAQ API error: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var answer = JsonSerializer.Deserialize<AnswerResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (answer == null)
                return StatusCode(500, "Failed to deserialize FAQ API response.");

            return Ok(answer);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(503, $"FAQ API unreachable: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error contacting FAQ API: {ex.Message}");
        }
    }
}


