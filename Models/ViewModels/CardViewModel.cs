using Models.Entities;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

﻿namespace ModelsLeit.ViewModels
{
    public record CardViewModel
    {
        public long Id { get; set; }
        public string Ask { get; set; }
        public string Answer { get; set; }
        public string Description { get; set; }
        public bool HasMp3 { get; set; }
        public CardCollectionViewModel Collection { get; set; }
    }
}
