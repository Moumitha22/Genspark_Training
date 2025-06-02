using System.Text.Json.Serialization;

namespace Banking_FAQ.Models {
    public class QuestionRequest
    {
        [JsonPropertyName("question")]
        public string Question { get; set; } = string.Empty;
    }

}



