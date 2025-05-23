using FileMangementApplication.Interfaces;

public class TextFileHandlerFactory : IFileHandlerFactory
{
    public IFileReader CreateReader()
    {
        return new TextFileReader();
    }

    public IFileWriter CreateWriter()
    {
        return new TextFileWriter();
    }
}
