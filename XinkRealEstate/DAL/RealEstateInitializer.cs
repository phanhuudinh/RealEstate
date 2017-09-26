using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using XinkRealEstate.Models;

namespace XinkRealEstate.DAL
{
    public class RealEstateInitializer : DropCreateDatabaseIfModelChanges<RealEstateContext>
    {
        protected override void Seed(RealEstateContext context)
        {
            var categories = new List<Category>
            {
                new Category{Id = 0, Name="Loại Hình BĐS ", Code="BDS_TYPE ", DisplayOrder=0, Level=0, ParentCategoryId= -1, CreateOn=DateTime.Now, UpdateOn=DateTime.Now},
                new Category{Id = 1, Name="Nhà", Code="BDS_TYPE_NHA", DisplayOrder=1, Level=1, ParentCategoryId= 0, CreateOn=DateTime.Now, UpdateOn=DateTime.Now},
                new Category{Id = 2, Name="Đất ", Code="BDS_TYPE_DAT", DisplayOrder=1, Level=1, ParentCategoryId= 0, CreateOn=DateTime.Now, UpdateOn=DateTime.Now},
                new Category{Id = 3, Name="Hướng", Code="BDS_DIRECTION ", DisplayOrder=1, Level=0, ParentCategoryId= -1, CreateOn=DateTime.Now, UpdateOn=DateTime.Now},
                new Category{Id = 4, Name="Bắc", Code="BDS_DIRECTION_BAC", DisplayOrder=1, Level=1, ParentCategoryId= 3, CreateOn=DateTime.Now, UpdateOn=DateTime.Now},
                new Category{Id = 5, Name="Nam", Code="BDS_DIRECTION_NAM", DisplayOrder=1, Level=1, ParentCategoryId= 3, CreateOn=DateTime.Now, UpdateOn=DateTime.Now}
            };
            categories.ForEach(c => context.Categories.Add(c));
            context.SaveChanges();
        }
    }
}