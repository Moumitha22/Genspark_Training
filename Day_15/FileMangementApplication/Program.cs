using System;

class Program
{
    static void Main(string[] args)
    {
        string textFilePath = "ExampleTextFile.txt";
        string textContent = "Hello, this is a sample text file.";

        string jsonFilePath = "exampleJsonFile.json";
        string jsonContent = "{ \"message\": \"Hello, this is a sample JSON file.\" }";

        var singleton = FileHandlerFactorySingleton.Instance;

        // --- Using Text file handler ---
        singleton.SetFactory(new TextFileHandlerFactory());
        var textFactory = singleton.GetFactory();

        var textWriter = textFactory.CreateWriter();
        var textReader = textFactory.CreateReader();

        textWriter.Write(textFilePath, textContent);
        Console.WriteLine($"Text written to {textFilePath}");

        string readText = textReader.Read(textFilePath);
        Console.WriteLine($"\nText read from {textFilePath}:\n{readText}\n");

        // --- Using JSON file handler ---
        singleton.SetFactory(new JsonFileHandlerFactory());
        var jsonFactory = singleton.GetFactory();

        var jsonWriter = jsonFactory.CreateWriter();
        var jsonReader = jsonFactory.CreateReader();

        jsonWriter.Write(jsonFilePath, jsonContent);
        Console.WriteLine($"\nJSON written to {jsonFilePath}");

        string readJson = jsonReader.Read(jsonFilePath);
        Console.WriteLine($"\nJSON read from {jsonFilePath}:\n{readJson}");
    }
}
