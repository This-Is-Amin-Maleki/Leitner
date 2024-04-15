﻿using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public record Collection
    {
        public long Id { get; set; }
        public String Name { get; set; }
        public String? Description { get; set; }
        public DateTime Date { get; set; }
        public CollectionStatus Status { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public long UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
