using System.IO;
using FileMangementApplication.Interfaces;

public class TextFileReader : IFileReader
{
    public string Read(string filePath)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            return reader.ReadToEnd();
        }
    }
}
