using ModelsLeit.Entities;
using ModelsLeit.ViewModels;
using SharedLeit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLeit.DTOs.Container
{
    public record ContainersDto
    {
        public long Id { get; set; }
        public long SlotId { get; set; }
        public int SlotOrder { get; set; }
        public DateTime DateModified { get; set; }
        public ICollection<ContainerCard> ContainerCards { get; set; }
    }
}
