using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ModelsLeit.Entities;
using IronXL;
using System.Data;
using Microsoft.Extensions.Logging;
using ServicesLeit.Interfaces;

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
            WorkBook workBook = WorkBook.Load(filePath);
            WorkSheet workSheet = workBook.WorkSheets.First();

            List<Card> cards = new();
            workSheet.Rows.FirstOrDefault().RemoveRow();
            foreach (var row in workSheet.Rows)
            {
                if (max-- is 0 || row.Columns[0].IsEmpty)
                {
                    break;
                }
                cards.Add(new Card()
                {
                    CollectionId = collectionId,
                    Ask = row.Columns[0].ToString(),
                    Answer = row.Columns[1].ToString(),
                    Description = row.Columns[2].ToString(),
                });
            }
            return cards;
        }
        public List<Card> ReadCardsFromExcelFile(string filePath, long collectionId)
        {
            WorkBook workBook = WorkBook.Load(filePath);
            WorkSheet workSheet = workBook.WorkSheets.First();

            List<Card> cards = new();
            workSheet.Rows.FirstOrDefault().RemoveRow();
            foreach (var row in workSheet.Rows)
            {
                cards.Add(new Card()
                {
                    CollectionId = collectionId,
                    Ask = row.Columns[0].ToString(),
                    Answer = row.Columns[1].ToString(),
                    Description = row.Columns[2].ToString(),
                });
            }
            return cards;
        }
    }
}