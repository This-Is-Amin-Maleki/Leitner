﻿using ModelsLeit.Entities;
using System.Reflection;

namespace ModelsLeit.ViewModels
{
    public record ContainerStudyViewModel
    {
        public long Id { get; set; }
        public string CollectionName { get; set; }
        public int CardPerDay { get; set; }
        public long SlotId { get; set; }
        public long LastCardId { get; set; }
        public int SlotOrder { get; set; }
        public long BoxId { get; set; }
        public ICollection<CardViewModel> Approved { get; set; }
        //    public long Id { get; set; }
        public ICollection<CardViewModel>? Rejected { get; set; }
        //    public long Id { get; set; }
    }
}