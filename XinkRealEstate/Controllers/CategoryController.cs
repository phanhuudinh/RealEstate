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
        const int MAX_CATEGORY_LEVEL = 2;

        #region Actions
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
            var DataQuery = DataSearch.OrderBy(d => d.DisplayOrder)
                .Skip(querry.start)
                .Take(querry.length).ToList()
                .Select(d => new CategoryDto(d)).ToList();

            FillTree(DataQuery);

            var categrories = GetTreeCategories(-1, DataQuery);

            var categoriesDto = new DataTableResult<CategoryDto>(querry.draw, Data.Count(), DataSearch.Count(), categrories);

            var result = JsonConvert.SerializeObject(categoriesDto);

            var dataConvert = new DataTableResult<CategoryDto>(querry.draw, Data.Count(), DataSearch.Count(), DataQuery);

            return Content(result, "application/json");
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

            var listRemove = new List<Category>();
            DeleteTree(category, listRemove);

            db.Categories.RemoveRange(listRemove);
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

        /// <summary>
        /// Create a list option categories for select box
        /// </summary>
        /// <returns></returns>
        List<SelectListItem> SelectValueForCategory()
        {
            var category = new Category { Id = -1, Name = "", Level = MAX_CATEGORY_LEVEL + 1, ParentCategoryId = -1 };
            return SelectValueForCategory(category);
        }

        /// <summary>
        /// Create a list option categories for select box
        /// </summary>
        /// <param name="category">The category had selected before</param>
        /// <returns></returns>
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

        /// <summary>
        /// Build category tree
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="baseTree"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Fill tree where have no parent after query
        /// </summary>
        /// <param name="categories"></param>
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

        /// <summary>
        /// Delete category and all its children 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="listRemove"></param>
        void DeleteTree(Category root, List<Category> listRemove)
        {
            var children = db.Categories.Where(c => c.ParentCategoryId == root.Id).ToList();
            foreach (var item in children)
            {
                DeleteTree(item, listRemove);
            }
            listRemove.Add(root);
        }
        #endregion
    }
}
