using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLeit.Entities
{
    public class ApplicationUser:IdentityUser<long>
    {
        public bool Active {  get; set; }
    }
}
