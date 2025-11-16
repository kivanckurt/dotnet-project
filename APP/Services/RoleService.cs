using APP.Domain;
using APP.Models;
using CORE.APP.Models;
using CORE.APP.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace APP.Services
{
    public class RoleService : Service<Role>, IService<RoleRequest, RoleResponse>
    {
        public RoleService(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Role> Query(bool isNoTracking = true)
        {
            // Includes UserRoles to check for relations before deleting
            return base.Query(isNoTracking)
                .Include(r => r.UserRoles)
                .OrderBy(r => r.Name);
        }

        public List<RoleResponse> List()
        {
            return Query().Select(r => new RoleResponse
            {
                Id = r.Id,
                Guid = r.Guid,
                Name = r.Name
            }).ToList();
        }

        public RoleResponse Item(int id)
        {
            var entity = Query().SingleOrDefault(r => r.Id == id);
            if (entity == null)
                return null;

            return new RoleResponse
            {
                Id = entity.Id,
                Guid = entity.Guid,
                Name = entity.Name
            };
        }

        public RoleRequest Edit(int id)
        {
            var entity = Query().SingleOrDefault(r => r.Id == id);
            if (entity == null)
                return null;

            return new RoleRequest
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public CommandResponse Create(RoleRequest request)
        {
            if (Query().Any(r => r.Name == request.Name.Trim()))
                return Error("Role with the same name already exists!");

            var entity = new Role
            {
                Name = request.Name.Trim()
            };

            Create(entity);
            return Success("Role created successfully.", entity.Id);
        }

        public CommandResponse Update(RoleRequest request)
        {
            if (Query().Any(r => r.Id != request.Id && r.Name == request.Name.Trim()))
                return Error("Role with the same name already exists!");

            var entity = Query(false).SingleOrDefault(r => r.Id == request.Id);
            if (entity == null)
                return Error("Role not found!");

            entity.Name = request.Name.Trim();
            Update(entity);
            return Success("Role updated successfully.", entity.Id);
        }

        public CommandResponse Delete(int id)
        {
            var entity = Query(false).SingleOrDefault(r => r.Id == id);
            if (entity == null)
                return Error("Role not found!");

            // Check for relationships before deleting (like in CategoryService)
            if (entity.UserRoles.Any())
                return Error("Role can't be deleted, it is assigned to one or more users!");

            Delete(entity);
            return Success("Role deleted successfully.", entity.Id);
        }
    }
}