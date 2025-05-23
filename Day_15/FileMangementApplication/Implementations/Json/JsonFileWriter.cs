using System.IO;
using FileMangementApplication.Interfaces;

public class JsonFileWriter : IFileWriter
{
    public void Write(string filePath, string content)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.Write(content);
        }
    }
}
