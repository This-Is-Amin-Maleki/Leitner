using Microsoft.AspNetCore.Identity;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Models
{
    public class ApplicationUser : IdentityUser<long>
    {
        public UserType Type { get; set; }
        public ICollection<Collection> Collections { get; set; }
        public ICollection<Box> Boxes { get; set; }

    }
}
