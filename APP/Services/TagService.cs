using APP.Domain;
using APP.Models;
using CORE.APP.Models;
using CORE.APP.Services;
using Microsoft.EntityFrameworkCore;

namespace APP.Services
{
    public class TagService : Service<Tag>, IService<TagRequest, TagResponse>
    {
        public TagService(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Tag> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking).Include(t => t.BlogTags);
        }

        public CommandResponse Create(TagRequest request)
        {
            if (Query().Any(t => t.Name == request.Name.Trim()))
                return Error("Tag with same name exists!");
            var entity = new Tag
            {
                Name = request.Name.Trim()
            };
            Create(entity);
            return Success("Tag created successfully.", entity.Id);
        }

        public CommandResponse Delete(int id)
        {
            var entity = Query(false).SingleOrDefault(t => t.Id == id);
            if (entity is null)
                return Error("Tag not found!");
            Delete(entity.BlogTags);
            Delete(entity);
            return Success("Tag deleted successfully.", entity.Id);
        }

        public TagRequest Edit(int id)
        {
            var entity = Query().SingleOrDefault(t => t.Id == id);
            if (entity is null)
                return null;
            var request = new TagRequest
            {
                Id = entity.Id,
                Name = entity.Name
            };
            return request;
        }

        public TagResponse Item(int id)
        {
            var entity = Query().SingleOrDefault(t => t.Id == id);
            if (entity is null)
                return null;
            var response = new TagResponse
            {
                Guid = entity.Guid,
                Id = entity.Id,
                Name = entity.Name
            };
            return response;
        }

        public List<TagResponse> List()
        {
            return Query().Select(t => new TagResponse
            {
                Guid = t.Guid,
                Id = t.Id,
                Name = t.Name
            }).ToList();
        }

        public CommandResponse Update(TagRequest request)
        {
            if (Query().Any(t => t.Id != request.Id && t.Name == request.Name.Trim()))
                return Error("Tag with same name exists!");
            var entity = Query(false).SingleOrDefault(t => t.Id == request.Id);
            if (entity is null)
                return Error("Tag not found!");
            entity.Name = request.Name.Trim();
            Update(entity);
            return Success("Tag updated successfully.", entity.Id);
        }
    }
}