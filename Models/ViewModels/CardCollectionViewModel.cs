using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public record CardCollectionViewModel
    {
        public long Id { get; set; }
        public string? Name { get; set; }
    }
}
