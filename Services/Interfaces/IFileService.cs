using ModelsLeit.Entities;

namespace ServicesLeit.Interfaces
{
    public interface IFileService
    {
        Task WriteToFileAsync(string filePath, string data);
        List<Card> ReadCardsFromExcelFile(string filePath, long collectionId, int max);
        List<Card> ReadCardsFromExcelFile(string filePath, long collectionId); 

    }
}