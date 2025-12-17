using APP.Domain;
using APP.Models;
using CORE.APP.Models;
using CORE.APP.Services;
using CORE.APP.Services.Authentication.MVC;
using Microsoft.EntityFrameworkCore;

namespace APP.Services
{
    public class UserService : Service<User>, IService<UserRequest, UserResponse>
    {
       
        private readonly ICookieAuthService _cookieAuthService;

       
        public UserService(DbContext db, ICookieAuthService cookieAuthService) : base(db)
        {
            _cookieAuthService = cookieAuthService;
        }



        protected override IQueryable<User> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.Group)
                .OrderByDescending(u => u.IsActive).ThenBy(u => u.RegistrationDate).ThenBy(u => u.UserName);
        }

        public CommandResponse Create(UserRequest request)
        {
            if (Query().Any(u => u.UserName == request.UserName.Trim() && u.IsActive == request.IsActive))
                return Error("Active user with the same user name exists!");
            var entity = new User
            {
                UserName = request.UserName,
                Password = request.Password,
                FirstName = request.FirstName?.Trim(),
                LastName = request.LastName?.Trim(),
                Gender = request.Gender,
                BirthDate = request.BirthDate,
                RegistrationDate = DateTime.Now, // set registration date to the current date and time 
                Score = request.Score ?? 0,
                IsActive = request.IsActive,
                Address = request.Address?.Trim(),
                GroupId = request.GroupId,
                RoleIds = request.RoleIds
            };
            Create(entity);
            return Success("User created successfully.", entity.Id);
        }

        public CommandResponse Delete(int id)
        {
            var entity = Query(false).SingleOrDefault(u => u.Id == id); // isNoTracking is false for being tracked by EF Core to delete the entity
            if (entity is null)
                return Error("User not found!");
            Delete(entity.UserRoles);
            Delete(entity);
            return Success("User deleted successfully.", entity.Id);
        }

        public UserRequest Edit(int id)
        {
            var entity = Query().SingleOrDefault(u => u.Id == id);
            if (entity is null)
                return null;
            return new UserRequest
            {
                Id = entity.Id,
                UserName = entity.UserName,
                Password = entity.Password,
                IsActive = entity.IsActive,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Gender = entity.Gender,
                BirthDate = entity.BirthDate,
                Score = entity.Score,
                Address = entity.Address,
                GroupId = entity.GroupId,
                RoleIds = entity.RoleIds,
              
            };
        }

        public UserResponse Item(int id)
        {
            var entity = Query().SingleOrDefault(u => u.Id == id);
            if (entity is null)
                return null;
            return new UserResponse
            {
                Id = entity.Id,
                Guid = entity.Guid,
                UserName = entity.UserName,
                Password = entity.Password,
                IsActive = entity.IsActive,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Gender = entity.Gender,
                BirthDate = entity.BirthDate,
                RegistrationDate = entity.RegistrationDate,
                Score = entity.Score,
                Address = entity.Address,
                GroupId = entity.GroupId,
                RoleIds = entity.RoleIds,
              

                IsActiveF = entity.IsActive ? "Active" : "Inactive",
                FullName = entity.FirstName + " " + entity.LastName,
                GenderF = entity.Gender.ToString(), // will assign Woman or Man
                BirthDateF = entity.BirthDate.HasValue ? entity.BirthDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                RegistrationDateF = entity.RegistrationDate.ToString("MM/dd/yyyy"),
                ScoreF = entity.Score.ToString("N1"),
                Group = entity.Group != null ? entity.Group.Title : null,
                Roles = entity.UserRoles.Select(ur => ur.Role.Name).ToList()
            };
        }

        public List<UserResponse> List()
        {
            return Query().Select(u => new UserResponse
            {
                Id = u.Id,
                Guid = u.Guid,
                UserName = u.UserName,
                Password = u.Password,
                IsActive = u.IsActive,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Gender = u.Gender,
                BirthDate = u.BirthDate,
                RegistrationDate = u.RegistrationDate,
                Score = u.Score,
                Address = u.Address,
                GroupId = u.GroupId,
                RoleIds = u.RoleIds,

                IsActiveF = u.IsActive ? "Active" : "Inactive",
                FullName = u.FirstName + " " + u.LastName,
                GenderF = u.Gender.ToString(), // will assign Woman or Man
                BirthDateF = u.BirthDate.HasValue ? u.BirthDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                RegistrationDateF = u.RegistrationDate.ToString("MM/dd/yyyy"),
                ScoreF = u.Score.ToString("N1"),
                Group = u.Group != null ? u.Group.Title : null,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            }).ToList();
        }

        public CommandResponse Update(UserRequest request)
        {
            if (Query().Any(u => u.Id != request.Id && u.UserName == request.UserName.Trim() && u.IsActive == request.IsActive))
                return Error("Active user with the same user name exists!");
            var entity = Query(false).SingleOrDefault(u => u.Id == request.Id); // isNoTracking is false for being tracked by EF Core to update the entity
            if (entity is null)
                return Error("User not found!");
            Delete(entity.UserRoles);
            entity.UserName = request.UserName;
            entity.Password = request.Password;
            entity.FirstName = request.FirstName?.Trim();
            entity.LastName = request.LastName?.Trim();
            entity.Gender = request.Gender;
            entity.BirthDate = request.BirthDate;
            entity.Score = request.Score ?? 0;
            entity.IsActive = request.IsActive;
            entity.Address = request.Address?.Trim();
            entity.GroupId = request.GroupId;
            entity.RoleIds = request.RoleIds;
            Update(entity);
            return Success("User updated successfully.", entity.Id);
        }
        public async Task<CommandResponse> Login(UserLoginRequest request)
        {
           
            var entity = Query().SingleOrDefault(
                u => u.UserName == request.UserName
                  && u.Password == request.Password
                  && u.IsActive);

            if (entity is null)
                return Error("Invalid user name or password!");

            await _cookieAuthService.SignIn(
                entity.Id,
                entity.UserName,
                entity.UserRoles.Select(ur => ur.Role.Name).ToArray());

            return Success("User logged in successfully.", entity.Id);
        }

      
        public async Task Logout()
        {
            await _cookieAuthService.SignOut();
        }

        public CommandResponse Register(UserRegisterRequest request)
        {
            var roleEntity = Query<Role>().SingleOrDefault(r => r.Name == "User");
            if (roleEntity is null)
                return Error("\"User\" role not found!");

            return Create(new UserRequest
            {
                UserName = request.UserName,
                Password = request.Password,
                IsActive = true,

                RoleIds = [roleEntity.Id]
            });
        }
    }
}