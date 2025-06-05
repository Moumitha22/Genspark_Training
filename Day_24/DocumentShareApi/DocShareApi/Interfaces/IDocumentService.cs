using DocShareApi.Models;

namespace DocShareApi.Interfaces
{
    public interface IDocumentService
    {
        Task<Document> UploadDocument(Document doc);
        Task<IEnumerable<Document>> GetAllDocuments();
        Task<Document> GetDocument(Guid id);
    }
}
