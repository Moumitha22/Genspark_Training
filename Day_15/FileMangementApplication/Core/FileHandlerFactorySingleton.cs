using System;
using FileMangementApplication.Interfaces;

public class FileHandlerFactorySingleton
{
    private static FileHandlerFactorySingleton _instance;
    private static readonly object _lock = new object();

    private IFileHandlerFactory _factory;

    private FileHandlerFactorySingleton()
    {
        // Default factory (text files)
        _factory = new TextFileHandlerFactory();
    }

    public static FileHandlerFactorySingleton Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                    _instance = new FileHandlerFactorySingleton();
                return _instance;
            }
        }
    }

    public void SetFactory(IFileHandlerFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public IFileHandlerFactory GetFactory()
    {
        return _factory;
    }
}
