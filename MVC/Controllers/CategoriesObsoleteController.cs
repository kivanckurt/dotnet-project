using APP.Models;
using APP.Services;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace MVC.Controllers
{
    public class CategoriesObsoleteController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoriesObsoleteController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Categories
        // GET: Categories/Index
        public IActionResult Index()
        {
            var list = _categoryService.Query().ToList();
            return View(list);
        }

        // GET: Categories/Details/3
        public IActionResult Details(int id)
        {
            //var item = _categoryService.Query().Where(categoryResponse => categoryResponse.Id == id).SingleOrDefault();
            //var item = _categoryService.Query().FirstOrDefault(categoryResponse => categoryResponse.Id == id);
            //var item = _categoryService.Query().LastOrDefault(categoryResponse => categoryResponse.Id == id);
            var item = _categoryService.Query().SingleOrDefault(categoryResponse => categoryResponse.Id == id);
            if (item == null)
                return NotFound();
            //ViewData["Message"] = "Today is Monday, another boring MVC lecture";
            ViewBag.Message = "Today is Monday, another boring MVC lecture";
            return View(item);
        }

        public IActionResult GetContent(string name, string surname)
        {
            //return Content("Name: " + name + ", Surname: " + surname);
            return Content($"Name: {name}, Surname: {surname}");
        }

        public IActionResult GetContent1()
        {
            return View();
        }

        public IActionResult GetContent2(string name, string surname)
        {
            return Content($"Name: {name}, Surname: {surname}");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = _categoryService.Create(request);
                if (response.IsSuccessful)
                {
                    TempData["CreatedMessage"] = response.Message;
                    //return RedirectToAction("Index");
                    return RedirectToAction(nameof(Details), new { id = response.Id });
                }
                //ViewBag.CreateMessage = response.Message;
                //ViewData["CreateMessage"] = response.Message;
                ModelState.AddModelError("", response.Message);
            }
            return View(request);
        }
    }
}
