#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CORE.APP.Services;
using APP.Models;

// Generated from Custom MVC Template.

namespace MVC.Controllers
{
    public class UsersController : Controller
    {
        // Service injections:
        private readonly IService<UserRequest, UserResponse> _userService;
        private readonly IService<GroupRequest, GroupResponse> _groupService;
        private readonly IService<RoleRequest, RoleResponse> _roleService;

        public UsersController(
            IService<UserRequest, UserResponse> userService,
            IService<GroupRequest, GroupResponse> groupService,
            IService<RoleRequest, RoleResponse> roleService
        )
        {
            _userService = userService;
            _groupService = groupService;
            _roleService = roleService;
        }

        // This private method is what populates your ViewBag
        private void SetViewData()
        {
            ViewData["GroupId"] = new SelectList(_groupService.List(), "Id", "Title");
            ViewBag.RoleIds = new MultiSelectList(_roleService.List(), "Id", "Name");
        }

        private void SetTempData(string message, string key = "Message")
        {
            TempData[key] = message;
        }

        // GET: Users
        public IActionResult Index()
        {
            var list = _userService.List();
            return View(list);
        }

        // GET: Users/Details/5
        public IActionResult Details(int id)
        {
            var item = _userService.Item(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            SetViewData(); // <-- THIS LINE FIXES THE ERROR
            return View();
        }

        // POST: Users/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(UserRequest user)
        {
            if (ModelState.IsValid)
            {
                var response = _userService.Create(user);
                if (response.IsSuccessful)
                {
                    SetTempData(response.Message);
                    return RedirectToAction(nameof(Details), new { id = response.Id });
                }
                ModelState.AddModelError("", response.Message);
            }
            SetViewData(); // <-- This line repopulates the dropdowns if validation fails
            return View(user);
        }

        // GET: Users/Edit/5
        public IActionResult Edit(int id)
        {
            var item = _userService.Edit(id);
            if (item == null) return NotFound();

            // Re-populate dropdowns for the Edit view, selecting current values
            ViewData["GroupId"] = new SelectList(_groupService.List(), "Id", "Title", item.GroupId);
            ViewBag.RoleIds = new MultiSelectList(_roleService.List(), "Id", "Name", item.RoleIds);

            return View(item);
        }

        // POST: Users/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(UserRequest user)
        {
            if (ModelState.IsValid)
            {
                var response = _userService.Update(user);
                if (response.IsSuccessful)
                {
                    SetTempData(response.Message);
                    return RedirectToAction(nameof(Details), new { id = response.Id });
                }
                ModelState.AddModelError("", response.Message);
            }

            // Re-populate dropdowns if model state is invalid
            ViewData["GroupId"] = new SelectList(_groupService.List(), "Id", "Title", user.GroupId);
            ViewBag.RoleIds = new MultiSelectList(_roleService.List(), "Id", "Name", user.RoleIds);

            return View(user);
        }

        // GET: Users/Delete/5
        public IActionResult Delete(int id)
        {
            var item = _userService.Item(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: Users/Delete
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var response = _userService.Delete(id);
            SetTempData(response.Message);
            return RedirectToAction(nameof(Index));
        }
    }
}