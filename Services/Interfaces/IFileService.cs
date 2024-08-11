using ModelsLeit.Entities;

namespace ServicesLeit.Interfaces
{
    public interface IFileService
    {
        List<Card> ReadCardsFromExcelFile(string filePath, long collectionId, int max);
        List<Card> ReadCardsFromExcelFile(string filePath, long collectionId);
    }
}