using System.IO;
using FileMangementApplication.Interfaces;

public class TextFileWriter : IFileWriter
{
    public void Write(string filePath, string content)
    {
        
        // StreamWriter writer = null;
        // try
        // {
        //     writer = new StreamWriter(filePath);
        //     writer.Write(content);
        // }
        // finally
        // {
        //     if (writer != null)
        //         writer.Dispose(); // Closes the file
        // }
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.Write(content);
        }
    }
}
