using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;

using System.Web.Mvc;
using XinkRealEstate.DAL;
using XinkRealEstate.DTOs;
using XinkRealEstate.DTOs.Categories;
using XinkRealEstate.Models;

namespace XinkRealEstate.Controllers
{
    public class CategoryController : Controller
    {
        #region Actions
        const int MAX_CATEGORY_LEVEL = 2;
        private RealEstateContext db = new RealEstateContext();

        // GET: Category
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: List
        public ContentResult GetDataTable(DataTableRequest querry)
        {
            var Data = db.Categories;
            // TODO: Order
            string searchkey = querry.search?.value ?? "";
            var DataSearch = Data.Where(d => d.Name.Contains(searchkey) || d.Code.Contains(searchkey));
            var DataQuery = DataSearch.OrderBy(d => d.Id)
                .Skip(querry.start)
                .Take(querry.length).ToList()
                .Select(d => new CategoryDto(d)).ToList();
            FillTree(DataQuery);
            var test = GetTreeCategories(-1, DataQuery);
            var testConv = new DataTableResult<CategoryDto>(querry.draw, Data.Count(), DataSearch.Count(), test);
            var testresult = JsonConvert.SerializeObject(testConv);
            var dataConvert = new DataTableResult<CategoryDto>(querry.draw, Data.Count(), DataSearch.Count(), DataQuery);
            var result = JsonConvert.SerializeObject(dataConvert);
            return Content(testresult, "application/json");
        }

        // GET: Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Category/Create
        public ActionResult Create()
        {
            ViewBag.categories = SelectValueForCategory();
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ParentCategoryId,PictureId,Description,DisplayOrder,Code")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.CreateOn = DateTime.Now;
                category.UpdateOn = DateTime.Now;

                if (category.ParentCategoryId >= 0)
                {
                    var parent = db.Categories.Find(category.ParentCategoryId);
                    category.Level = parent?.Level + 1 ?? 0;
                }
                else
                {
                    category.Level = 0;
                }

                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var categories = db.Categories.Where(c => c.Level < MAX_CATEGORY_LEVEL).ToList();
            //ViewBag.categories = categories;
            ViewBag.categories = SelectValueForCategory();
            return View(category);
        }

        // GET: Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                category.UpdateOn = DateTime.Now;
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Methods

        List<SelectListItem> SelectValueForCategory()
        {
            var category = new Category { Id = -1, Name = "", Level = MAX_CATEGORY_LEVEL + 1 };
            return SelectValueForCategory(category);
        }
        List<SelectListItem> SelectValueForCategory(Category category)
        {
            var categoriesCanBeParent = db.Categories.Where(c => c.Level < category.Level);
            List<SelectListItem> categories = new List<SelectListItem>();
            // Curent select
            categories.Add(new SelectListItem { Text = "", Value = category.ParentCategoryId.ToString(), Selected = true });

            if (categoriesCanBeParent.Count() > 0)
            {
                foreach (Category c in categoriesCanBeParent)
                {
                    categories.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
                }
            }
            return categories;
        }

        List<CategoryDto> GetTreeCategories (int parentId, List<CategoryDto> baseTree)
        {
            if(baseTree.Count == 0)
            {
                return null;
            }

            List<CategoryDto> result = new List<CategoryDto>();
            foreach (var item in baseTree)
            {
                if(item.ParentCategoryId == parentId)
                {
                    item.Children = GetTreeCategories(item.Id, baseTree);
                    result.Add(item);
                }
            }
            return result;
        }

        void FillTree(List<CategoryDto> categories)
        {
            var appendItems = new List<CategoryDto>();
            foreach (var item in categories)
            {
                var parent = categories.Where(c => c.Id == item.ParentCategoryId);
                if(parent.Count() == 0)
                {
                    var insertParent = db.Categories.Find(item.ParentCategoryId);
                    if(insertParent != null)
                    {
                        appendItems.Add(new CategoryDto(insertParent));
                    }
                }
            }
            if(appendItems.Count() > 0)
            {
                categories.AddRange(appendItems);
                FillTree(categories);
            }
        }

        //void UpdateChildLevel(Category parent)
        //{
        //    var childrent = db.Categories.Where(c => c.ParentCategoryId == parent.Id);
        //    foreach (var item in childrent)
        //    {
        //        item.Level = parent.Level + 1;
        //    }
        //}

        #endregion
    }
}
