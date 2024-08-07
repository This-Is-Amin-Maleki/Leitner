﻿using ModelsLeit.Entities;
using System.Reflection;

namespace ModelsLeit.ViewModels
{
    public record ContainerStudiedViewModel
    {
        public long Id { get; set; }
        public long SlotId { get; set; }
        public long LastCardId { get; set; }
        public int SlotOrder { get; set; }
        public long BoxId { get; set; }
        public long[]? Approved { get; set; }
        //    public long Id { get; set; }
        public long[]? Rejected { get; set; }
        //    public long Id { get; set; }
    }
}