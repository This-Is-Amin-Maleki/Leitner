using ModelsLeit.Entities;
using Microsoft.Extensions.Logging;
using ServicesLeit.Interfaces;
using SharedLeit;
using Ganss.Excel;

namespace ServicesLeit.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }
        public async Task WriteToFileAsync(string filePath, string data)
        {
            if (!File.Exists(filePath))
            {
                File.CreateText(filePath);
            }
            await File.AppendAllTextAsync(filePath, data+"\r\n");
        }
        public List<Card> ReadCardsFromExcelFile(string filePath,long collectionId,int max)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("The specified file does not exist");
            }

            var excel = new ExcelMapper(filePath);
            excel.AddMapping<Card>(1, p => p.Ask);
            excel.AddMapping<Card>(2, p => p.Answer);
            excel.AddMapping<Card>(3, p => p.Description);

            if (excel is null || excel.Fetch<Card>().Count() is 0)
            {
                throw new Exception("The file is empty or the first row is empty");
            }

            var cards = excel.Fetch<Card>().Take(max+1).ToList();
            cards.ForEach(x => x.CollectionId = collectionId);

            var surplusCard = cards.First();
            bool isFirstSurplus = surplusCard.Ask.Equals("ask", StringComparison.OrdinalIgnoreCase);
            if (isFirstSurplus is false)
            {
                surplusCard = cards.Last();
            }
            if (isFirstSurplus || cards.Count > max)
            {
                cards.Remove(surplusCard);
            }
            
            return cards;
        }
        public List<Card> ReadCardsFromExcelFile(string filePath, long collectionId)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("The specified file does not exist");
            }

            var excel = new ExcelMapper(filePath);
            excel.AddMapping<Card>(1, p => p.Ask);
            excel.AddMapping<Card>(2, p => p.Answer);
            excel.AddMapping<Card>(3, p => p.Description);

            if (excel is null || excel.Fetch<Card>().Count() is 0)
            {
                throw new Exception("The file is empty or the first row is empty");
            }

            var cards = excel.Fetch<Card>().ToList();
            cards.ForEach(x => x.CollectionId = collectionId);

            var firstCard = cards.First();
            bool isFirstHeader = firstCard.Ask.Equals("ask", StringComparison.OrdinalIgnoreCase);
            if (isFirstHeader)
            {
                cards.Remove(firstCard);
            }

            return cards;
        }
    }
}