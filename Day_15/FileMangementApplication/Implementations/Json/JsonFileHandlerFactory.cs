using FileMangementApplication.Interfaces;

public class JsonFileHandlerFactory : IFileHandlerFactory
{
    public IFileReader CreateReader()
    {
        return new JsonFileReader();
    }

    public IFileWriter CreateWriter()
    {
        return new JsonFileWriter();
    }
}
