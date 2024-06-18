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
    public record ContainerReadDto
    {
        public ContainerStudyViewModel Container { get; set; }
        public bool AnyCard { get; set; }
        public bool AnyReqCard { get; set; }
    }
}
