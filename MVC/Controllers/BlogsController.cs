#nullable disable
using APP.Models;
using APP.Services;
using CORE.APP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

// Generated from Custom MVC Template.

namespace MVC.Controllers
{
    [Authorize]
    public class BlogsController : Controller
    {
        // Service injections:
        private readonly IService<BlogRequest, BlogResponse> _blogService;
        private readonly IService<UserRequest, UserResponse> _userService;
        private readonly IBlogSessionService _blogSessionService;

        /* Can be uncommented and used for many to many relationships, "entity" may be replaced with the related entity name in the controller and views. */
        private readonly IService<TagRequest, TagResponse> _tagService;

        public BlogsController(
			IService<BlogRequest, BlogResponse> blogService
            , IService<UserRequest, UserResponse> userService
            , IService<TagRequest, TagResponse> tagService
            , IBlogSessionService blogSessionService
        )
        {
            _blogService = blogService;
            _userService = userService;
            _blogSessionService = blogSessionService;

            /* Can be uncommented and used for many to many relationships, "entity" may be replaced with the related entity name in the controller and views. */
            _tagService = tagService;
        }

        private int GetUserId()
        {
            return Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == "Id")?.Value);
        }

        private void SetViewData()
        {
            /* 
            ViewBag and ViewData are the same collection (dictionary).
            They carry extra data other than the model from a controller action to its view, or between views.
            */

            // Related items service logic to set ViewData (Id and Name parameters may need to be changed in the SelectList constructor according to the model):
            var users = _userService.List() ?? new List<UserResponse>();
            var tags = _tagService.List() ?? new List<TagResponse>();

            ViewBag.UserId = new SelectList(_userService.List(), "Id", "UserName");

            /* Can be uncommented and used for many to many relationships, "entity" may be replaced with the related entity name in the controller and views. */
            ViewBag.TagIds = new MultiSelectList(_tagService.List(), "Id", "Name");
        }

        private void SetTempData(string message, string key = "Message")
        {
            /*
            TempData is used to carry extra data to the redirected controller action's view.
            */

            TempData[key] = message;
        }

        // GET: Blogs
        public IActionResult Index()
        {
            // Get collection service logic:
            var list = _blogService.List();
            return View(list); // return response collection as model to the Index view
        }

        // GET: Blogs/Details/5
        public IActionResult Details(int id)
        {
            // Get item service logic:
            var item = _blogService.Item(id);
            return View(item); // return response item as model to the Details view
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            SetViewData(); // set ViewData dictionary to carry extra data other than the model to the view
            var draft = _blogSessionService.GetDraft(GetUserId(), null);
            return View(draft); // return Create view with draft or no model
        }

        // POST: Blogs/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(BlogRequest blog, string saveAction)
        {
            var userId = GetUserId();
            if (saveAction == "SaveDraft")
            {
                _blogSessionService.SaveDraft(userId, blog);
                SetTempData("Draft saved.");
                ModelState.Clear();
                SetViewData();
                return View(blog);
            }

            if (ModelState.IsValid) // check data annotation validation errors in the request
            {
                // Insert item service logic:
                var response = _blogService.Create(blog);
                if (response.IsSuccessful)
                {
                    _blogSessionService.ClearDraft(userId, null);
                    SetTempData(response.Message); // set TempData dictionary to carry the message to the redirected action's view
                    return RedirectToAction(nameof(Details), new { id = response.Id }); // redirect to Details action with id parameter as response.Id route value
                }
                ModelState.AddModelError("", response.Message); // to display service error message in the validation summary of the view
            }
            SetViewData(); // set ViewData dictionary to carry extra data other than the model to the view
            return View(blog); // return request as model to the Create view
        }

        // GET: Blogs/Edit/5
        public IActionResult Edit(int id)
        {
            // Get item to edit service logic:
            var userId = GetUserId();
            var draft = _blogSessionService.GetDraft(userId, id);
            var item = draft ?? _blogService.Edit(id);
            SetViewData(); // set ViewData dictionary to carry extra data other than the model to the view
            return View(item); // return request as model to the Edit view
        }

        // POST: Blogs/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(BlogRequest blog, string saveAction)
        {
            var userId = GetUserId();
            if (saveAction == "SaveDraft")
            {
                _blogSessionService.SaveDraft(userId, blog);
                SetTempData("Draft saved.");
                ModelState.Clear();
                SetViewData();
                return View(blog);
            }

            if (ModelState.IsValid) // check data annotation validation errors in the request
            {
                // Update item service logic:
                var response = _blogService.Update(blog);
                if (response.IsSuccessful)
                {
                    _blogSessionService.ClearDraft(userId, blog.Id);
                    SetTempData(response.Message); // set TempData dictionary to carry the message to the redirected action's view
                    return RedirectToAction(nameof(Details), new { id = response.Id }); // redirect to Details action with id parameter as response.Id route value
                }
                ModelState.AddModelError("", response.Message); // to display service error message in the validation summary of the view
            }
            SetViewData(); // set ViewData dictionary to carry extra data other than the model to the view
            return View(blog); // return request as model to the Edit view
        }

        // GET: Blogs/Delete/5
        public IActionResult Delete(int id)
        {
            // Get item to delete service logic:
            var item = _blogService.Item(id);
            return View(item); // return response item as model to the Delete view
        }

        // POST: Blogs/Delete
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            // Delete item service logic:
            var userId = GetUserId();
            _blogSessionService.ClearDraft(userId, id);
            var response = _blogService.Delete(id);
            SetTempData(response.Message); // set TempData dictionary to carry the message to the redirected action's view
            return RedirectToAction(nameof(Index)); // redirect to the Index action
        }
    }
}
