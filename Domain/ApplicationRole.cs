using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain
{
    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>, IEntity<string>
    {
    }
}
