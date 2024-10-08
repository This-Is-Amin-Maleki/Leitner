﻿using ModelsLeit.DTOs.Card;
using ModelsLeit.Entities;
using System.Reflection;

namespace ModelsLeit.DTOs.Container
{
    public record ContainerStudyDto
    {
        public long Id { get; set; }
        public string CollectionName { get; set; }
        public int CardPerDay { get; set; }
        public long SlotId { get; set; }
        public long LastCardId { get; set; }
        public int SlotOrder { get; set; }
        public long BoxId { get; set; }
        public ICollection<CardDto> Approved { get; set; }
        public ICollection<CardDto> Rejected { get; set; }
    }
}