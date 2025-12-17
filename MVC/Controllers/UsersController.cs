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
    public class UsersController : Controller
    {
        private readonly IService<UserRequest, UserResponse> _userService;
        private readonly IService<GroupRequest, GroupResponse> _groupService;

      

        /* Can be uncommented and used for many to many relationships, "entity" may be replaced with the related entity name in the controller and views. */
        private readonly IService<RoleRequest, RoleResponse> _RoleService;

        public UsersController(
            IService<UserRequest, UserResponse> userService
            , IService<GroupRequest, GroupResponse> groupService
 , IService<RoleRequest, RoleResponse> RoleService
        )
        {
            _userService = userService;
            _groupService = groupService;

          
            _RoleService = RoleService;
        }

        private void SetViewData()
        {
            /* 
            ViewBag and ViewData are the same collection (dictionary).
            They carry extra data other than the model from a controller action to its view, or between views.
            */

            // Related items service logic to set ViewData (Id and Name parameters may need to be changed in the SelectList constructor according to the model):
            ViewData["GroupId"] = new SelectList(_groupService.List(), "Id", "Title");

            /* Can be uncommented and used for many to many relationships, "entity" may be replaced with the related entity name in the controller and views. */
            ViewBag.RoleIds = new MultiSelectList(_RoleService.List(), "Id", "Name");
        }

        private void SetTempData(string message, string key = "Message")
        {
            /*
            TempData is used to carry extra data to the redirected controller action's view.
            */

            TempData[key] = message;
        }

        
        [Authorize]
        public IActionResult Index()
        {
            // Get collection service logic:
            var list = _userService.List();
            return View(list); // return response collection as model to the Index view
        }

        bool IsOwnAccount(int id) 
        {
            return id.ToString() == (User.Claims.SingleOrDefault(claim => claim.Type == "Id")?.Value ?? string.Empty);
        }

        // GET: Users/Details/5
        [Authorize] 
        public IActionResult Details(int id)
        {
            if (!IsOwnAccount(id) && !User.IsInRole("Admin"))
            {
                SetTempData("You are not authorized for this operation!");
                return RedirectToAction(nameof(Index));
            }

            var item = _userService.Item(id);
            return View(item); 
        }

        // GET: Users/Create
        [Authorize(Roles = "Admin")] 
        public IActionResult Create()
        {
            SetViewData(); 


            return View(); 
        }

        // POST: Users/Create
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] 
        public IActionResult Create(UserRequest user)
        {
            if (ModelState.IsValid) // check data annotation validation errors in the request
            {
                // Insert item service logic:
                var response = _userService.Create(user);
                if (response.IsSuccessful)
                {
                    SetTempData(response.Message); // set TempData dictionary to carry the message to the redirected action's view
                    return RedirectToAction(nameof(Details), new { id = response.Id }); // redirect to Details action with id parameter as response.Id route value
                }
                ModelState.AddModelError("", response.Message); // to display service error message in the validation summary of the view
            }
            SetViewData(); // set ViewData dictionary to carry extra data other than the model to the view


            return View(user); // return request as model to the Create view
        }

        // GET: Users/Edit/5
        [Authorize] 
        public IActionResult Edit(int id)
        {
            if (!IsOwnAccount(id) && !User.IsInRole("Admin"))
            {
                SetTempData("You are not authorized for this operation!");
                return RedirectToAction(nameof(Index));
            }

            // Get item to edit service logic:
            var item = _userService.Edit(id);
            SetViewData(); // set ViewData dictionary to carry extra data other than the model to the view


            return View(item); // return request as model to the Edit view
        }

        // POST: Users/Edit
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize] 
        public IActionResult Edit(UserRequest user)
        {
            if (!IsOwnAccount(user.Id) && !User.IsInRole("Admin"))
            {
                SetTempData("You are not authorized for this operation!");
                return RedirectToAction(nameof(Index));
            }

            if (!User.IsInRole("Admin"))
            {
                ModelState.Remove(nameof(UserRequest.Score));
                ModelState.Remove(nameof(UserRequest.RoleIds));
            }

            if (ModelState.IsValid) // check data annotation validation errors in the request
            {
                // Update item service logic:
                var response = _userService.Update(user);
                if (response.IsSuccessful)
                {
                    SetTempData(response.Message); // set TempData dictionary to carry the message to the redirected action's view
                    return RedirectToAction(nameof(Details), new { id = response.Id }); // redirect to Details action with id parameter as response.Id route value
                }
                ModelState.AddModelError("", response.Message); // to display service error message in the validation summary of the view
            }
            SetViewData(); // set ViewData dictionary to carry extra data other than the model to the view


            return View(user); // return request as model to the Edit view
        }

        // GET: Users/Delete/5
        [Authorize]
        public IActionResult Delete(int id)
        {
            // Check if the user is in Admin role or trying to make the operation on his/her own account.
            if (!IsOwnAccount(id) && !User.IsInRole("Admin"))
            {
                SetTempData("You are not authorized for this operation!");
                return RedirectToAction(nameof(Index));
            }

            // Get item to delete service logic:
            var item = _userService.Item(id);
            return View(item); // return response item as model to the Delete view
        }

        // POST: Users/Delete
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        [Authorize]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsOwnAccount(id) && !User.IsInRole("Admin"))
            {
                SetTempData("You are not authorized for this operation!");
                return RedirectToAction(nameof(Index));
            }

            // Delete item service logic:
            var response = _userService.Delete(id);
            SetTempData(response.Message); // set TempData dictionary to carry the message to the redirected action's view

            // if the user deleted his/her own account, log out the user
            if (IsOwnAccount(id))
                return RedirectToAction(nameof(Logout));

            return RedirectToAction(nameof(Index)); // redirect to the Index action
        }



      
        [Route("~/[action]")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("~/[action]")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            if (ModelState.IsValid) 
            {
               
                var userService = _userService as UserService; 
                                                              
                var response = await userService.Login(request); 
                if (response.IsSuccessful)
                    return RedirectToAction("Index", "Home");
                ModelState.AddModelError("", response.Message);
            }
            return View(); 
        }

      
        [Route("~/[action]")]
        public async Task<IActionResult> Logout()
        {
            var userService = _userService as UserService; 
                                                           
            await userService.Logout(); 
            return RedirectToAction(nameof(Login)); 
        }

        [Route("~/[action]")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, Route("~/[action]")]
        public IActionResult Register(UserRegisterRequest request)
        {
            if (ModelState.IsValid) 
            {
                var userService = _userService as UserService; 
                var response = userService.Register(request); 
                if (response.IsSuccessful)
                    return RedirectToAction(nameof(Login)); 
                ModelState.AddModelError("", response.Message); 
            }
            return View(request); 
        }
    }
}