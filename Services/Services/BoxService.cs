﻿using DataAccessLeit.Context;
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
        public async Task<List<BoxViewModel>> ReadAllAsync()
        {//use auto mapper
            return await _dbContext.Boxes
                .AsNoTracking()
                .Include(x => x.Collection)
                .ThenInclude(x => x.Cards)
                .ThenInclude(x => x.ContainerCards)
                .Select(x => new BoxViewModel()
                {
                    Id = x.Id,
                    DateAdded = x.DateAdded,
                    DateStudied = x.DateStudied,
                    CardPerDay = x.CardPerDay,
                    LastSlot = x.LastSlot,
                    LastCardId = x.LastCardId,
                    Completed = x.Completed,
                    Collection = new CollectionMiniViewModel()
                    {
                        Id = x.Collection.Id,
                        Name = x.Collection.Name,

                    },
                })
                .ToListAsync();
        }
        public async Task<List<BoxViewModel>> GetAllAsync()
        {//use auto mapper
            return await _dbContext.Boxes
                .Include(x => x.Collection)
                .Select(x => new BoxViewModel()
                {
                    Id = x.Id,
                    DateAdded = x.DateAdded,
                    DateStudied = x.DateStudied,
                    LastSlot = x.LastSlot,
                    Collection = new CollectionMiniViewModel()
                    {
                        Id = x.Collection.Id,
                        Name = x.Collection.Name
                    }
                })
                .ToListAsync();
        }
        public async Task<List<BoxViewModel>> ReadByCollectionAsync(long id)
        {//use auto mapper
            return await _dbContext.Boxes
                .AsNoTracking()
                .Include(x => x.Collection)
                .Where(x => x.CollectionId == id)
                .Select(x => new BoxViewModel()
                {
                    Id = x.Id,
                    DateAdded = x.DateAdded,
                    DateStudied = x.DateStudied,
                    LastSlot = x.LastSlot,
                    LastCardId = x.LastCardId,
                    CardPerDay = x.CardPerDay,
                    Completed = x.Completed,
                    Collection = new CollectionMiniViewModel()
                    {
                        Id = x.Collection.Id,
                        Name = x.Collection.Name,
                    },
                })
                .ToListAsync();
        }
        public async Task<List<BoxViewModel>> GetByCollectionAsync(long id)
        {//use auto mapper
            return await _dbContext.Boxes
                .Include(x => x.Collection)
                .Where(x => x.CollectionId == id)
                .Select(x => new BoxViewModel()
                {
                    Id = x.Id,
                    DateAdded = x.DateAdded,
                    DateStudied = x.DateStudied,
                    LastSlot = x.LastSlot,
                    Collection = new CollectionMiniViewModel()
                    {
                        Id = x.Collection.Id,
                        Name = x.Collection.Name
                    }
                })
                .ToListAsync();
        }

#warning check performance
        public async Task<BoxViewModel> ReadAsync(long id)
        {
            var box = await _dbContext.Boxes
                .AsNoTracking()
                .Include(x => x.Collection)
                .FirstOrDefaultAsync(x => x.Id == id);

            return box is null ?
                CreateEmptyBoxViewModel() :
                MapBoxToViewModel(box);
        }
#warning check performance
        public async Task<BoxViewModel> GetAsync(long id)
        {
            var box = await _dbContext.Boxes
                .Include(x => x.Collection)
                .FirstOrDefaultAsync(x => x.Id == id);

            return box is null ?
                CreateEmptyBoxViewModel() :
                MapBoxToViewModel(box);
        }

        public async Task AddAsync(BoxAddViewModel model)
        {
            //just 
            Box box = new()
            {
                CollectionId = model.CollectionId,
                DateAdded = DateTime.Now,
                Id = 0,
                LastCardId = 0,
                LastSlot = 0,
                CardPerDay = model.CardPerDay,
                DateStudied = new(),
                Slots = new List<Slot>(),
                Completed = false,

            };

            for (int i = -3; i < 5; i++)
            {
                Slot slot = new()
                {
                    Order = i,
                    Containers = new List<ModelsLeit.Entities.Container>(),
                };
                int containersNum = GetContainersNum(i);
                for (int j = 0; j < containersNum || j == 0; j++)
                {
                    slot.Containers.Add(new());
                }
                box.Slots.Add(slot);
            }

            await _dbContext.Boxes.AddAsync(box);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<ReviewViewModel> ReviewAsync(long id)
        {
            var model = await _dbContext.Boxes
                .AsNoTracking()
                .Include(x => x.Collection)
                .Include(x => x.Slots)
                .ThenInclude(x => x.Containers)
                .ThenInclude(x => x.ContainerCards)
                .ThenInclude(x => x.Card)
                .Where(x => x.Id == id && x.LastCardId > 0)
                .Select(x => new ReviewViewModel
                {
                    BoxId = x.Id,
                    CollectionName = x.Collection.Name,
                    Cards = x.Slots
                        .Where(y => y.Order < -1)
                        .SelectMany(slot =>
                            slot.Containers.SelectMany(container =>
                                container.ContainerCards.Select(containerCard => containerCard.Card)))
                        .Select(x=>new CardViewModel
                        {
                            Id = x.Id,
                            HasMp3 = x.HasMp3,
                            Description = x.Description,
                            Ask=x.Ask,
                            Answer=x.Answer,
                        })
                        .ToList()

                })
                .FirstOrDefaultAsync();

            if (model is null)
            {
                throw new Exception("Box Not Found");
            }
            if (model.Cards.Count == 0)
            {
                throw new Exception("No cards available for review.");
            }

            return model;
        }

        public async Task<ContainerStudyViewModel> StudyAsync(long id)
        {
            var boxDto = await _dbContext.Boxes
                .AsNoTracking()
                .Include(x => x.Collection)
                .Select(x => new BoxDto()
                {
                    Id = x.Id,
                    DateStudied = x.DateStudied,
                    CardPerDay = x.CardPerDay,
                    LastCardId = x.LastCardId,
                    LastSlot = x.LastSlot,
                    Completed = x.Completed,
                    Collection = new()
                    {
                        Id = x.CollectionId,
                        Name = x.Collection.Name,
                        Count = x.Collection.Count,
                    },
                    Containers = new List<ContainersDto>(),
                })
                .FirstOrDefaultAsync(x =>x.Id == id);

            if (boxDto is null)
            {
                throw new Exception("Box Not Found");
            }
            if (boxDto.Completed)
            {
                throw new Exception("Box is Completed");
            }
            /*if(boxDto.DateStudied > DateTime.Now.AddDays(-1) && boxDto.LastSlot is 0)
            {
                throw new Exception("Today's study for this flashcard deck has been completed.");
            }*/

            boxDto.Containers = await _dbContext.Slots
                .Include(x => x.Containers)
                .ThenInclude(x => x.ContainerCards)
                .ThenInclude(x => x.Card)
                .Where(x => x.BoxId == id)
                .SelectMany(x =>
                        x.Containers.OrderBy(y => y.DateModified).Take(1),
                        (slot, container) =>
                    new ContainersDto()
                    {
                        Id = container.Id,
                        SlotId = slot.Id,
                        ContainerCards = container.ContainerCards,
                        DateModified = container.DateModified,
                        SlotOrder = container.Slot.Order,
                    }
                ).ToListAsync();

            ContainerReadDto read= await ReadLastContainerOfSlotAsync(boxDto);
            ContainerStudyViewModel container = read.Container;
            Container mainContainer = new();
            Box box = new();
            while (read.AnyCard is false)// || container.SlotOrder == -1 && cards are empty)
            {
                boxDto.LastSlot = ForwardEmptyContainer(boxDto);
                if (boxDto.LastSlot == 0) {
                    break;
                }
                read = await ReadLastContainerOfSlotAsync(boxDto);
                container = read.Container;
            }

            box = new()
            {
                Id = boxDto.Id,
                LastSlot = boxDto.LastSlot,
            };
            _dbContext.Attach(box);
            _dbContext.Entry(box).Property(x => x.LastSlot).IsModified = true;
            
            if (read.AnyReqCard)
            {
                var dropedCards = boxDto.Containers.Where(x => x.SlotOrder < -1).Select(x => x.ContainerCards.Count).ToArray().Sum();
                if(boxDto.Collection.Count == dropedCards)
                {
                    box.Completed = true;
                    _dbContext.Entry(box).Property(x => x.Completed).IsModified = true;
                }
            }

            var allCards = container.Approved.Concat(container.Rejected);
            if (boxDto.LastSlot is 0 && allCards.Any() is false) //new cards is latest study level
            {
                box.DateStudied = DateTime.Now;
                _dbContext.Entry(box).Property(x => x.DateStudied).IsModified = true;
            }
            //if (mainContainer.Id is not 0)
            //{
            //_dbContext.Update(mainContainer);
            //_dbContext.Attach(box);
            //_dbContext.Entry(box).Property(x => x.LastSlot).IsModified = true;
            //await _dbContext.SaveChangesAsync();
            //}

            await _dbContext.SaveChangesAsync();
            return container;
        }

        public async Task UpdateAsync(ContainerStudiedViewModel model)
        {
            // 1 - 4 = Learning Cards
            //   0   = Today Learning Cards
            //  -1   = Today Rejected Cards
            //  -2   = Known Cards
            //  -3   = Covered cards
            int order = model.SlotOrder;
            //0 => 1 => 2 => 3 => 4 => -1 |=> 0
            int nextOrder = order is 4 ? -1 : order + 1;
            int rejectOrder = nextOrder < 1 ? order - 2 : -1 ;
            long[] orders = [nextOrder, order, rejectOrder];

            var slots = _dbContext.Slots
                    .AsNoTracking()
                    .Include(x => x.Containers)
                    .ThenInclude(x => x.ContainerCards)
                    .Where(x =>
                        x.BoxId == model.BoxId &&
                        orders.Contains(x.Order))
                    .Select(x => new Slot
                    {
                        Id = x.Id,
                        Order = x.Order,
                        Containers = x.Containers.OrderBy(y=>y.DateModified).Take(1).ToList(),
                    })
                    .ToList();

            model.Approved=model.Approved ?? [];
            model.Rejected=model.Rejected ?? [];

            if (nextOrder is < 1) // order 4 && -1
            {
                var tempCards = model.Approved;
                model.Approved = model.Rejected;
                model.Rejected = tempCards;
            }

            if (model.Rejected.Length != 0)
            {

                ////clear current slot
                //var currentSlot = slots
                //        .FirstOrDefault(x => x.Order == order);
                //Container currentContainer = currentSlot.Containers.FirstOrDefault();
                //currentContainer.SlotId = currentSlot.Id;
                //currentContainer.DateModified = DateTime.Now;
                //currentContainer.ContainerCards.Clear();

                //_dbContext.Update(currentContainer);

                //_dbContext.SaveChanges();
                //rejects to slot -1
                var rejectSlot = slots
                    .FirstOrDefault(x => x.Order == rejectOrder);

                Container rejectContainer = rejectSlot.Containers.FirstOrDefault();
                AddCardsIds2Container(rejectContainer, model.Rejected, rejectContainer.Id);
                rejectContainer.DateModified = DateTime.Now;
                //rejectContainer.SlotId = rejectSlot.Id;   Fix SlotId  [-1,-2,-3]

                _dbContext.Update(rejectContainer);
            }
            //_dbContext.SaveChanges();

                //next slot
                var nextSlot = slots
                        .FirstOrDefault(x => x.Order == nextOrder);
                //remove all containcards in container
                var containerCards = _dbContext.ContainerCards
                    .AsNoTracking()
                    .Where(x => x.ContainerId == model.Id)
                    .ToList();
                _dbContext.ContainerCards.RemoveRange(containerCards);
                //recreate this contain and containcards

                var cards = CardsIds2ContainerCard(model.Approved);
                Container mainContainer = new()
                {
                    Id = model.Id,
                    SlotId = nextSlot.Id,
                    DateModified = DateTime.Now,
                    ContainerCards = cards,
                };

                _dbContext.Containers.Update(mainContainer);
            
            //var trackedCards = _dbContext.ChangeTracker.Entries<Card>().ToList();
            //foreach (var cardEntry in trackedCards)
            //{
            //    cardEntry.Property(x => x.Answer).IsModified = false;
            //    cardEntry.Property(x => x.Ask).IsModified = false;
            //    cardEntry.Property(x => x.CollectionId).IsModified = false;
            //    cardEntry.Property(x => x.Description).IsModified = false;
            //    cardEntry.Property(x => x.HasMp3).IsModified = false;
            //}

            Box box = new()
            {
                Id = model.BoxId,
                LastSlot = nextOrder,
            };
            _dbContext.Entry(box).Property(x => x.LastSlot).IsModified = true;

            if (nextOrder is 0) //new cards is latest study level
            {
                var allCards = model.Approved.Concat(model.Rejected);
                var maxCardId = allCards.Max();
                box.LastCardId = model.LastCardId < maxCardId ? maxCardId : model.LastCardId ;
                box.DateStudied = DateTime.Now;
                _dbContext.Entry(box).Property(x => x.LastCardId).IsModified = true;
                _dbContext.Entry(box).Property(x => x.DateStudied).IsModified = true;
            }

            _dbContext.SaveChanges();
        }

        public async Task<ContainerStudyViewModel> StudyFailAsync(ContainerStudiedViewModel model)
        {
            var allCards = (model.Approved ?? []).Concat(model.Rejected ?? []);

            var boxData = _dbContext.Boxes
                .AsNoTracking()
                .Include(x => x.Collection)
                .Select(x => new
                {
                    BoxId = x.Id,
                    CollectionName = x.Collection.Name,
                    CardPerDay = x.CardPerDay,
                })
                .FirstOrDefault(x => x.BoxId == model.BoxId);

            var cards =await _cardService.ReadCardsAsync(allCards);
            //var cards = _dbContext.Cards
            //    .AsNoTracking()
            //    .Where(x => allCards.Contains(x.Id))
            //    .ToList();

            return new ContainerStudyViewModel
            {
                Approved = CardsIds2Card(model.Approved),
                Rejected = CardsIds2Card(model.Rejected),
                CollectionName = boxData.CollectionName,
                CardPerDay = boxData.CardPerDay,
                BoxId = model.BoxId,
                SlotId = model.SlotId,
                SlotOrder = model.SlotOrder,
                Id = model.Id,
            };
        }

        public async Task DeleteAsync(long id)
        {
            var box = await _dbContext.Boxes
                //.AsNoTracking()
                .Include(x => x.Slots)
                .ThenInclude(x => x.Containers)
                .ThenInclude(x => x.ContainerCards)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (box is null)
            {
                throw new Exception("Not Found");
            }
            /*
            var Ss= box.Slots.ToList();
            var Cs= Ss.Select(x=> x.Containers.FirstOrDefault()??new()).ToList();
            var CCs = Cs.Select(x => x.ContainerCards.ToList().ForEach(y=>y.cou)?? new List<ContainerCard>());
            var CCz = CCs.Select(x=> x.ForEach(y=>y.Id))

            _dbContext.RemoveRange(CCs.);
            _dbContext.SaveChanges();
            _dbContext.RemoveRange(Cs);
            _dbContext.SaveChanges();
            _dbContext.RemoveRange(Ss);
            _dbContext.SaveChanges();*/
            _dbContext.Remove(box);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<CardViewModel> ReadNextCardAsync(long boxId,int num)
        {
            //check box
            var box = await _dbContext.Boxes
                .AsNoTracking()
                .FirstOrDefaultAsync(x=>x.Id == boxId);

            if(box is null)
            {
                throw new Exception("Box Not Found");
            }

            //get new cardsArray
            var cards = await _cardService.ReadCardsAsync(box.LastCardId, box.CollectionId,1,(num-1));
            var card = cards.FirstOrDefault();

            if(card is null)
            {
                throw new Exception("No cards available for review.");
            }

            return card;
        }
        ////////////////////////////////////////////////////////

        private int ForwardEmptyContainer(BoxDto model)
        {
            int order = model.LastSlot;
            //0 => 1 => 2 => 3 => 4 => -1 |=> 0
            int nextOrder = order is 4 ? -1 : order + 1;
            
            var currentContainer = model.Containers
                .FirstOrDefault(x => x.SlotOrder == order);
            
            var nextSlotContainer = model.Containers
                .FirstOrDefault(x => x.SlotOrder == nextOrder)??new();

            if (nextSlotContainer.Id is 0)//No Continer in Slot, So get SlotId to move new container to that.
            {
                nextSlotContainer.SlotId = _dbContext.Slots
                    .FirstOrDefault(x => x.BoxId == model.Id && x.Order == nextOrder).Id;
            }

            Container mainContainer = new()
            {
                Id = currentContainer.Id,
                SlotId = nextSlotContainer.SlotId,
                DateModified = DateTime.Now,
                ContainerCards = currentContainer.ContainerCards//it's empty so it doesnt have any card!!
            };

            _dbContext.Update(mainContainer);
            _dbContext.SaveChanges();
            return nextOrder;
        }
        
        private static List<CardViewModel> CardsIds2Card(long[] cards)
        {
            //cardsArray id array to cardsArray list
            return cards
                .Select(id => new CardViewModel { Id = id })
                .ToList();
        }
        private static List<ContainerCard> CardsIds2ContainerCard(long[] cards)
        {
            //cardsArray id array to cardsArray list
            return cards
                .Select(id => new ContainerCard { CardId = id })
                .ToList();
        }
        private Container AddCardsIds2Container(Container container, long[] cards, long containerId)
        {
            foreach (var item in cards)
            {
                container.ContainerCards.Add(new ContainerCard
                {
                    CardId = item,
                    ContainerId = containerId,
                });
            }
            return container;
            ////cardsArray id array to cardsArray list
            //return cards
            //    .Select(id => )
            //    .ToList();
        }

        private static int GetContainersNum(int i)
        {
            return (int)Math.Round(Math.Pow(2, i));
        }

        private BoxViewModel CreateEmptyBoxViewModel()
        {
            return new BoxViewModel();
        }

        private BoxViewModel MapBoxToViewModel(Box box) => new BoxViewModel();
        private async Task<ContainerReadDto> ReadLastContainerOfSlotAsync(BoxDto model)
        {
            //                         new  +  rej  fin
            // 0 => 1 => 2 => 3 => 4 =>    -1      => 0
            //                        0+new   0+rej
            //                               BoxDate
            //read last container of slot
            bool anyReqCard = false;
            var container = model.Containers
                .Where(x => x.SlotOrder == model.LastSlot)
                .Select(x => new ContainerStudyViewModel()
                {
                    Approved = x.ContainerCards
                        .Select(x => new CardViewModel
                        {
                            Answer = x.Card.Answer,
                            Ask = x.Card.Ask,
                            Description = x.Card.Description,
                            HasMp3 = x.Card.HasMp3,
                            Id = x.Card.Id,
                        })
                        .ToList(),
                    Id = x.Id,
                    BoxId = model.Id,
                    LastCardId=model.LastCardId,
                    SlotId = x.SlotId,
                    SlotOrder = model.LastSlot,//x.SlotOrder,
                    CardPerDay = model.CardPerDay,
                    CollectionName = model.Collection.Name,
                    Rejected = new List<CardViewModel>(),
                })
                .FirstOrDefault();

            if (container is null)
            {
                if (model.LastSlot is 0)
                {
                    container = model.Containers
                    .Where(x => x.SlotOrder == -1)
                    .Select(x => new ContainerStudyViewModel()
                    {
                        Approved = x.ContainerCards.Select(x => new CardViewModel
                        {
                            Answer = x.Card.Answer,
                            Ask = x.Card.Ask,
                            Description = x.Card.Description,
                            HasMp3 = x.Card.HasMp3,
                            Id = x.Card.Id,
                        }).ToList(),
                        Id = x.Id,
                        BoxId = model.Id,
                        SlotId = x.SlotId,
                        SlotOrder = model.LastSlot,//x.SlotOrder,
                        CardPerDay = model.CardPerDay,
                        CollectionName = model.Collection.Name,
                        Rejected = new List<CardViewModel>(),
                    })
                    .FirstOrDefault();
                }
                else
                {
                    throw new Exception("Container Not Found (ReadLastContainerOfSlotAsync)");
                }
            }

            if (model.LastSlot == -1)
            {

                //get new cardsArray
                var reqCards = await _cardService.ReadCardsAsync(model.LastCardId, model.Collection.Id, model.CardPerDay);

                anyReqCard = reqCards.Count is 0;
                // move DayRejected from cardsArray to rejected list
                container.Rejected = container.Approved;
                // set new cardsArray to cardsArray list
                container.Approved = reqCards ?? new();
                //foreach (var card in reqCards)
                //{
                //    container.Approved.Add(card);
                //}
            }

            return new()
            {
                AnyCard = container.Approved.Any(),
                Container =container,
                AnyReqCard = anyReqCard,
            };
        }
    }
}