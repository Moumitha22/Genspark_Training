using System.IO;
using System.Text.Json;
using FileMangementApplication.Interfaces;

public class JsonFileReader : IFileReader
{
    public string Read(string filePath)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            string json = reader.ReadToEnd();
            var obj = JsonSerializer.Deserialize<object>(json);
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
