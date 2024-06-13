using Microsoft.EntityFrameworkCore;
using ModelsLeit.DTOs;
using ModelsLeit.Entities;
using ModelsLeit.ViewModels;
using System.Data;

namespace ServicesLeit.Services
{
    public class BoxServiceBase
    {
        /** /
        
        private async Task<ContainerStudyViewModel> ReadLastContainerOfSlotAsync(BoxDto model)
        {
            //read last container of slot
            var container = model.Slots
                .Where(x => x.Order == model.LastSlot)
                .SelectMany(x => x.Containers)
                .OrderBy(x => x.DateModified)
                .Select(x => new ContainerStudyViewModel()
                {
                    Approved = x.Approved,
                    DateModified = x.DateModified,
                    Id = x.Id,
                    Slot = new ContainerSlotViewModel()
                    {
                        Id = x.SlotId,
                        Order = model.LastSlot,
                        BoxId = model.Id
                    }
                })
                .FirstOrDefault();

            if (container is null)
            {
                throw new Exception("Not Found");
            }

            int cardsNeeded = model.CardPerDay - container.Approved.Count;
            if (model.LastSlot == 4 && cardsNeeded > 0)
            {
                //get some cards
                var reqCards = _dbContext.Approved
                    .Where(x => x.Id > model.LastCardId && x.CollectionId == model.Collection.Id)
                    .Take(cardsNeeded)
                    .ToList();
                foreach (var card in reqCards)
                {
                    container.Approved.Add(card);
                }
            }
            return container;
        }
        private async Task<ContainerStudyViewModel> ReadLastContainerOfSlotAsync(BoxStudyViewModel model)
        {

            //read last container of slot
            var container = await _dbContext.Slots
                .Include(x => x.Containers)
                .ThenInclude(x => x.Approved)
                .Where(x =>
                    x.BoxId == model.Id &&
                    x.Order == model.LastSlot)
                .SelectMany(x => x.Containers)
                .OrderBy(x => x.DateModified)
                .Select(x => new ContainerStudyViewModel()
                {
                    Approved = x.Approved,
                    DateModified = x.DateModified,
                    Id = x.Id,
                    Slot = new ContainerSlotViewModel()
                    {
                        Id = x.SlotId,
                        Order = model.LastSlot,
                        BoxId = model.Id
                    }
                })
                .FirstOrDefaultAsync();

            if (container is null)
            {
                throw new Exception("Not Found");
            }

            int cardsNeeded = model.CardPerDay - container.Approved.Count;
            if (model.LastSlot == -1 && cardsNeeded > 0)
            {
                //get some cards
                var reqCards = _dbContext.Approved
                    .Where(x => x.Id > model.LastCardId && x.CollectionId == model.CollectionId)
                    .Take(cardsNeeded)
                    .ToList();
                foreach (var card in reqCards)
                {
                    container.Approved.Add(card);
                }
            }
            return container;
        }


        private static BoxStudyViewModel MapBoxToBoxStudyViewModel(long id, Box box)
        {
            return new()
            {
                Id = id,
                LastSlot = box.LastSlot,
                DateStudied = box.DateStudied,
                CardPerDay = box.CardPerDay,
                LastCardId = box.LastCardId,
                CollectionId = box.CollectionId,
            };
        }
        //////////////////////////////////////////////////////
        public async Task<ContainerStudyViewModel> StudyAsync2(long id)
        {
            var box = await _dbContext.Boxes
                .Include(x => x.Collection)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (box is null)
            {
                throw new Exception("Box Not Found");
            }
            /// Default value should be 0, indicating: 'Start studying from slot 4 and decrease the slot number after each study, until it reaches 0'.
            /// -1 indicating : 'It's tempSlot, add some new cards to fill and study mistakes'.
            /// 0-4 indicating : 'Start studying cards of slot[(2^StdySlot) - 1]'.
            BoxStudyViewModel model = MapBoxToBoxStudyViewModel(id, box);
            //if (model.LastSlot is 0)
            //{
            //    model.LastSlot = 4;
            //}

            ContainerStudyViewModel container = await ReadLastContainerOfSlotAsync(model);

            while (container.Approved.Any() is false)
            {
                //dont need to check lastslot < -1!! @-1 ReadLastContainerOfSlot add some cards
                model.LastSlot -= 1;
                container = await ReadLastContainerOfSlot(model);
            }
            box.LastSlot = model.LastSlot;
            container.CollectionName = box.Collection.Name;
            _dbContext.Update(box);
            await _dbContext.SaveChangesAsync();
            return container;
        }
        //  ContainerStudyViewModel
        public async Task<ContainerStudyViewModel> StudyAsync3(long id)
        {
            var boxDto = await _dbContext.Boxes
                .AsNoTracking()
                .Include(x => x.Collection)
                .Include(x => x.Slots)
                .ThenInclude(x => x.Containers)
                .ThenInclude(x => x.Approved)
                .Select(x => new BoxDto()
                {
                    Id = x.Id,
                    CardPerDay = x.CardPerDay,
                    DateAdded = x.DateAdded,
                    DateStudied = x.DateStudied,
                    LastCardId = x.LastCardId,
                    LastSlot = x.LastSlot,
                    Collection = new()
                    {
                        Id = x.CollectionId,
                        Name = x.Collection.Name
                    },
                    Slots = x.Slots.Select(y => new SlotDto()
                    {
                        BoxId = y.BoxId,
                        Containers = y.Containers
                            .OrderBy(z => z.DateModified)
                            .Take(1)
                            .ToList(),// [old] maybe OrderByDescending
                        Id = y.Id,
                        Order = y.Order,
                    })
                }
                ).FirstOrDefaultAsync(x => x.Id == id);

            if (boxDto is null)
            {
                throw new Exception("Box Not Found");
            }
            Box box = MapBoxDtoToBox(boxDto);

            /// Default value should be 0, indicating: 'Start studying from slot 4 and decrease the slot number after each study, until it reaches 0'.
            /// -1 indicating : 'It's tempSlot, add some new cards to fill and study mistakes'.
            /// 0-4 indicating : 'Start studying cards of slot[(2^StdySlot) - 1]'.
            //if (boxDto.LastSlot is 0)
            //{
            //    boxDto.LastSlot = 4;
            //}

            ContainerStudyViewModel container = await ReadLastContainerOfSlotAsync(boxDto);

            while (container.Approved.Any() is false)
            {
                var mainContainer = box.Slots
                    .SelectMany(slot => slot.Containers)
                    .FirstOrDefault(container => container.Id == container.Id);

                var nextSlotOrder = boxDto.LastSlot is 4 ? -1 : boxDto.LastSlot + 1;
                //calc next Slot Order, x +1 [next]
                var addToSlot = box.Slots
                    .FirstOrDefault(x => x.Order == nextSlotOrder);
                //get Slot 
                var deleteFromSlot = box.Slots
                    .FirstOrDefault(x => x.Order == boxDto.LastSlot);


                mainContainer.DateModified = DateTime.Now;
                mainContainer.SlotId = addToSlot.Id;
                //attach some changes in Container to Context
                _dbContext.Attach(mainContainer);
                _dbContext.Entry(mainContainer).Property(x => x.DateModified).IsModified = true;

                //add container to nextSlot
                addToSlot.Containers.Add(mainContainer);
                //attach some changes in nowSlot to Context
                _dbContext.Attach(addToSlot);

                //remove container from nowSlot
                deleteFromSlot.Containers.Remove(mainContainer);
                //attach some changes in nowSlot to Context
                _dbContext.Attach(deleteFromSlot);

                //dont need to check lastslot < -1!! @-1 ReadLastContainerOfSlot add some cards
                boxDto.LastSlot -= 1;
                container = await ReadLastContainerOfSlot(boxDto);
            }
            box.LastSlot = boxDto.LastSlot;
            _dbContext.Attach(box);
            _dbContext.Entry(box).Property(x => x.LastSlot).IsModified = true;
            await _dbContext.SaveChangesAsync();

            container.CollectionName = boxDto.Collection.Name;
            return container;
        }


        public async Task UpdateAsync(ContainerStudyViewModel model)
        {
            bool dropCards = model.Slot.Order > 3;
            bool newCards = model.Slot.Order == -1;
            int[] InOrder = [model.Slot.Order, -1];
            if (!dropCards)
            {
                InOrder = InOrder.Append(model.Slot.Order + 1).ToArray();
            }

            var boxDto = await _dbContext.Boxes
                .Include(x => x.Slots)
                .ThenInclude(x => x.Containers)
                //.ThenInclude(x => x.Approved)
                .Select(x => new BoxDto()
                {
                    Id = x.Id,
                    CardPerDay = x.CardPerDay,
                    DateAdded = x.DateAdded,
                    DateStudied = x.DateStudied,
                    LastCardId = x.LastCardId,
                    LastSlot = x.LastSlot,
                    Collection = new()
                    {
                        Id = x.CollectionId,
                    },
                    Slots = x.Slots.Select(y => new SlotDto()
                    {
                        BoxId = y.BoxId,
                        Containers = y.Containers.OrderBy(z => z.DateModified).Take(1).ToList(),
                        Id = y.Id,
                        Order = y.Order,
                    })
                }
                ).FirstOrDefaultAsync(x => x.Id == model.Slot.BoxId);

            if (boxDto is null)
            {
                throw new Exception("Box Not Found");
            }
            Box box = MapBoxDtoToBox(boxDto);
            var box2 = MapBoxDtoToBox(boxDto);
            Slot addToSlot;
            if (newCards)       //New Approved
            {
                box.DateStudied = DateTime.Now;
                box.LastSlot = 0;
                //set lastCardId [in -1 if cards count are lower than perDay get some cards]
                var lastCardId = model.Approved.OrderBy(x => x.Id).Last().Id;
                box.LastCardId = box.LastCardId < lastCardId ? lastCardId : box.LastCardId;

                //next is 0 Slot Order, -1 [new+rejected] +1 [next]= 0
                addToSlot = box.Slots
                    .FirstOrDefault(x => x.Order == 0);
                //merge Approved [Aproved] + Rejected
                model.Approved = model.Approved.Concat(model.Rejected).ToList();
            }
            else if (dropCards) //Del Approved && add Rejecteds
            {
                //drop approved cards & rejected as cards in slot -1
                model.Approved = model.Rejected;
                addToSlot = box.Slots
                    .FirstOrDefault(x => x.Order == -1);
            }
            else                //Holds Approved...
            {
                //calc next Slot Order, x +1 [next]
                addToSlot = box.Slots
                    .FirstOrDefault(x => x.Order == model.Slot.Order + 1);

                //reject cards
                var rejectedContainer = box.Slots
                    .FirstOrDefault(x => x.Order == -1)?
                    .Containers
                    .FirstOrDefault();
                foreach (var card in model.Rejected)
                {
                    rejectedContainer.Approved.Add(card);
                }
                //save to [model.Slot.Order+1]
                //send rejected to -1
            }
            var deleteFromSlot = box.Slots
                .FirstOrDefault(x => x.Order == model.Slot.Order);
            var mainContainer = deleteFromSlot?.Containers
                .OrderBy(x => x.DateModified)// [old] maybe OrderByDescending
                .FirstOrDefault();

            //main container
            mainContainer.Approved = model.Approved;
            mainContainer.DateModified = DateTime.Now;
            //add container to this
            addToSlot.Containers.Add(mainContainer);

            //remove container
            deleteFromSlot.Containers.Remove(mainContainer);

            //_dbContext.Approved.Update(container);
            //_dbContext.Slots.Update(nextSlot);
            //_dbContext.Slots.Update(nowSlot);
            _dbContext.Boxes.Update(box);
            _dbContext.Approved.AsNoTracking();
            _dbContext.SaveChanges();

            var box3 = _dbContext.Boxes
                .Include(x => x.Slots)
                .ThenInclude(x => x.Containers)
                .FirstOrDefault(x => x.Id == box.Id);
            var box4 = _dbContext.Boxes
                .Include(x => x.Slots)
                .FirstOrDefault(x => x.Id == box.Id);
            if (box3 == box2)
            {
                var t = true;
            }
        }
        /**/
    }
}