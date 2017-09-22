using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using XinkRealEstate.Models;

namespace XinkRealEstate.DAL
{
    public class RealEstateContext: DbContext
    {
        public RealEstateContext() :base("RealEstateConnection")
        {
        }

        public DbSet<Category> Categories { get; set; }

    }
}