namespace OnlineAuctionAPI.Exceptions;

public class RepositoryOperationException : Exception
{
    public RepositoryOperationException(string operation, Exception innerException)
     : base($"An error occured while performing the operation : {operation}.", innerException)
    {
        
     }
}