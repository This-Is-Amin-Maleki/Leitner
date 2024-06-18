using ModelsLeit.Entities;
using ModelsLeit.ViewModels;
using SharedLeit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLeit.DTOs
{
    public record CollectionDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int Count { get; set; }
    }
}
