using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Domain;
using Repositories.DBContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
namespace Repositories.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Repositories.DBContext.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            this.CommandTimeout = 60 * 30;
        }
    }        
}
