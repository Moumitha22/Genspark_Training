using DocShareApi.Interfaces;
using DocShareApi.Models;

namespace DocShareApi.RServices
{

    public class DocumentService : IDocumentService
    {
        private readonly IRepository<Guid, Document> _documentRepository;

        public DocumentService(IRepository<Guid, Document> documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<Document> UploadDocument(Document doc)
        {
            return await _documentRepository.Add(doc);
        }

        public async Task<IEnumerable<Document>> GetAllDocuments()
        {
            return await _documentRepository.GetAll();
        }

        public async Task<Document> GetDocument(Guid id)
        {
            return await _documentRepository.Get(id);
        }
    }
}
