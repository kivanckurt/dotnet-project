using APP.Domain;
using APP.Models;
using CORE.APP.Models;
using CORE.APP.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace APP.Services
{
    public class UserService : Service<User>, IService<UserRequest, UserResponse>
    {
        public UserService(DbContext db) : base(db)
        {
        }

        protected override IQueryable<User> Query(bool isNoTracking = true)
        {
            // Eagerly load all relations needed for List() and Item()
            return base.Query(isNoTracking)
                .Include(u => u.Group)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .OrderBy(u => u.UserName);
        }

        public List<UserResponse> List()
        {
            return Query().Select(u => new UserResponse
            {
                Id = u.Id,
                Guid = u.Guid,
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Gender = u.Gender,
                BirthDate = u.BirthDate,
                RegistrationDate = u.RegistrationDate,
                Score = u.Score,
                IsActive = u.IsActive,
                Address = u.Address,

                FullName = u.FirstName + " " + u.LastName,

                GenderF = u.Gender.ToString(), 
                BirthDateF = u.BirthDate.HasValue ? u.BirthDate.Value.ToString("MM/dd/yyyy") : string.Empty,

                RegistrationDateF = u.RegistrationDate.ToShortDateString(),
                ScoreF = u.Score.ToString("N1"), 
                IsActiveF = u.IsActive ? "Active" : "Inactive",
                //Roles = string.Join(", ", u.UserRoles.Select(ur => ur.Role.Name))
            }).ToList();
        }

        public UserResponse Item(int id)
        {
            var entity = Query().SingleOrDefault(u => u.Id == id);
            if (entity == null)
                return null;

            return new UserResponse
            {
                Id = entity.Id,
                Guid = entity.Guid,
                UserName = entity.UserName,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Gender = entity.Gender,
                BirthDate = entity.BirthDate,
                RegistrationDate = entity.RegistrationDate,
                Score = entity.Score,
                IsActive = entity.IsActive,
                Address = entity.Address,
                FullName = entity.FirstName + " " + entity.LastName,
                GenderF = entity.Gender.ToString(),
                BirthDateF = entity.BirthDate.HasValue ? entity.BirthDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                RegistrationDateF = entity.RegistrationDate.ToShortDateString(),
                ScoreF = entity.Score.ToString("N1"),
                IsActiveF = entity.IsActive ? "Active" : "Inactive",
                GroupTitle=entity?.Group?.Title
                //Roles = string.Join(", ", entity.UserRoles.Select(ur => ur.Role.Name))
            };
        }

        public UserRequest Edit(int id)
        {
            var entity = Query().SingleOrDefault(u => u.Id == id);
            if (entity == null)
                return null;

            return new UserRequest
            {
                Id = entity.Id,
                UserName = entity.UserName,
                Password = "", // IMPORTANT: Never send the password hash to the view
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Gender = entity.Gender,
                BirthDate = entity.BirthDate,
                IsActive = entity.IsActive,
                Address = entity.Address,
                CountryId = entity.CountryId,
                CityId = entity.CityId,
                GroupId = entity.GroupId,
                //RoleIds = entity.RoleIds // Use the NotMapped property to get the List<int>
            };
        }

        public CommandResponse Create(UserRequest request)
        {
            if (Query().Any(u => u.UserName == request.UserName.Trim() && u.IsActive))
                return Error("User with the same username already exists!");

            //if (string.IsNullOrWhiteSpace(request.Password))
            //    return Error("Password is required for a new user!");


            var hashedPassword = request.Password; 
            var entity = new User
            {
                UserName = request.UserName.Trim(),
                Password = hashedPassword,
                FirstName = request.FirstName?.Trim(),
                LastName = request.LastName?.Trim(),
                Gender = request.Gender,
                BirthDate = request.BirthDate,
                RegistrationDate = System.DateTime.Now,
                Score = 0,
                IsActive = request.IsActive,
                Address = request.Address?.Trim(),
                CountryId = request.CountryId,
                CityId = request.CityId,
                GroupId = request.GroupId,
                RoleIds = request.RoleIds // Use NotMapped property to set UserRoles
            };

            Create(entity);
            return Success("User created successfully.", entity.Id);
        }

        public CommandResponse Update(UserRequest request)
        {
            if (Query().Any(u => u.Id != request.Id && u.UserName == request.UserName.Trim() && u.IsActive))
                return Error("User with the same username already exists!");

            // Get the entity with tracking enabled and include relations to be updated
            var entity = Query(false)
                
                .SingleOrDefault(u => u.Id == request.Id);

            if (entity == null)
                return Error("User not found!");

            // Use the base Delete<T> method to remove old many-to-many records
            // This is the pattern from your ProductService example
            Delete(entity.UserRoles);

            // Update all properties from the request
            entity.UserName = request.UserName.Trim();
            entity.FirstName = request.FirstName?.Trim();
            entity.LastName = request.LastName?.Trim();
            entity.Gender = request.Gender;
            entity.BirthDate = request.BirthDate;
            entity.IsActive = request.IsActive;
            entity.Address = request.Address?.Trim();
            entity.CountryId = request.CountryId;
            entity.CityId = request.CityId;
            entity.GroupId = request.GroupId;
            entity.RoleIds = request.RoleIds;

            // Only update password if a new one was provided
            //if (!string.IsNullOrWhiteSpace(request.Password))
            //{
            //    entity.Password = request.Password; 
            //}

            Update(entity);
            return Success("User updated successfully.", entity.Id);
        }

        public CommandResponse Delete(int id)
        {
            // Get entity with tracking and include join tables for cascading delete
            var entity = Query(false)
                
                .SingleOrDefault(u => u.Id == id);

            if (entity == null)
                return Error("User not found!");

            // Manually delete related data from the join table first
            Delete(entity.UserRoles);

            // Now delete the main entity
            Delete(entity);
            return Success("User deleted successfully.", entity.Id);
        }
    }
}