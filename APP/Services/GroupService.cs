using APP.Domain;
using APP.Models;
using CORE.APP.Models;
using CORE.APP.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace APP.Services
{
    public class GroupService : Service<Group>, IService<GroupRequest, GroupResponse>
    {
        public GroupService(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Group> Query(bool isNoTracking = true)
        {
            // Includes Users to check for relations before deleting
            // and to populate the new UserNames property
            return base.Query(isNoTracking)
                .Include(g => g.Users)
                .OrderBy(g => g.Title);
        }

        public List<GroupResponse> List()
        {
            return Query().Select(g => new GroupResponse
            {
                Id = g.Id,
                Guid = g.Guid,
                Title = g.Title,
                // Flatten the list of user names into a single string
                UserNames = string.Join(", ", g.Users.Select(u => u.UserName))
            }).ToList();
        }

        public GroupResponse Item(int id)
        {
            var entity = Query().SingleOrDefault(g => g.Id == id);
            if (entity == null)
                return null;

            return new GroupResponse
            {
                Id = entity.Id,
                Guid = entity.Guid,
                Title = entity.Title,
                // Flatten the list of user names into a single string
                UserNames = string.Join(", ", entity.Users.Select(u => u.UserName))
            };
        }

        public GroupRequest Edit(int id)
        {
            var entity = Query().SingleOrDefault(g => g.Id == id);
            if (entity == null)
                return null;

            return new GroupRequest
            {
                Id = entity.Id,
                Title = entity.Title
            };
        }

        public CommandResponse Create(GroupRequest request)
        {
            if (Query().Any(g => g.Title == request.Title.Trim()))
                return Error("Group with the same title already exists!");

            var entity = new Group
            {
                Title = request.Title.Trim()
            };

            Create(entity);
            return Success("Group created successfully.", entity.Id);
        }

        public CommandResponse Update(GroupRequest request)
        {
            if (Query().Any(g => g.Id != request.Id && g.Title == request.Title.Trim()))
                return Error("Group with the same title already exists!");

            var entity = Query(false).SingleOrDefault(g => g.Id == request.Id);
            if (entity == null)
                return Error("Group not found!");

            entity.Title = request.Title.Trim();
            Update(entity);
            return Success("Group updated successfully.", entity.Id);
        }

        public CommandResponse Delete(int id)
        {
            // Query(false) is used here to ensure tracking for deletion.
            // The Query() override already includes .Users, so no change is needed.
            var entity = Query(false).SingleOrDefault(g => g.Id == id);
            if (entity == null)
                return Error("Group not found!");

            // Check for relationships before deleting
            if (entity.Users.Any())
                return Error("Group can't be deleted, it is assigned to one or more users!");

            Delete(entity);
            return Success("Group deleted successfully.", entity.Id);
        }
    }
}