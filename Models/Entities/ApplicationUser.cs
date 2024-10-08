﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLeit.Entities
{
    public class ApplicationUser : IdentityUser<long>
    {
        public bool Active {  get; set; }
        [Required]
        [Length(5,100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Bio { get; set; }

        public ICollection<Collection> Collections { get; set; }
    }
}
