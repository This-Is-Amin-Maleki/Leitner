using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.DTOs;
using Models.Entities;
using Models.ViewModels;
using System.Data;

namespace Services.Services
{
    public class BoxService
    //: IBoxService
    {
        private readonly ILogger<BoxService> _logger;

        private readonly ApplicationDbContext _dbContext;
        private readonly CardService _cardService;
        public BoxService(ApplicationDbContext dbContext, CardService cardService, ILogger<BoxService> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _cardService = cardService;
        }
    }
}