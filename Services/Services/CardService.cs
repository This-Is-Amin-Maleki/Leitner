using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.ViewModels;
using Services.Interfaces;
using Shared;
using Models.DTOs;
using System.Collections;
using System.Collections.Generic;

namespace Services.Services
{
    public class CardService : ICardService
    {
        private readonly ApplicationDbContext _dbContext;

        public CardService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

    }
}