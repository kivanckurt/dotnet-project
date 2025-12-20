using APP.Domain;
using APP.Models;
using CORE.APP.Domain;
using CORE.APP.Models;
using CORE.APP.Services;
using Microsoft.EntityFrameworkCore;

namespace APP.Services
{
    public class BlogService : Service<Blog>, IService<BlogRequest, BlogResponse>            
    {
        public BlogService(DbContext db) : base(db)
        {
        }
        protected override IQueryable<Blog> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking).Include(g => g.BlogTags).ThenInclude(gt => gt.Tag)
                .Include(b=> b.User)
                .OrderByDescending(g => g.PublishDate)
                .ThenBy(g => g.Title);
        }

        public CommandResponse Create(BlogRequest request)
        {
            if (Query().Any(g => g.Title == request.Title.Trim()))
                return Error("Blog with same title exists!");
             var entity = new Blog
             {
                 UserId = request.UserId ?? 0,
                 Title = request.Title.Trim(),
                 Content = request.Content,
                 Rating = request.Rating,
                 PublishDate = request.PublishDate,
                 TagIds = request.TagIds
             };
            Create(entity);
            return Success("Blog created successfully.", entity.Id);
        }

        public CommandResponse Delete(int id)
        {
            var entity = Query(false).SingleOrDefault(g => g.Id == id);
            if (entity is null)
                return Error("Blog not found!");
            Delete(entity.BlogTags);
            Delete(entity);
            return Success("Blog deleted successfully.", entity.Id);
        }

        public BlogRequest Edit(int id)
        {
            var entity = Query().SingleOrDefault(g => g.Id == id);
            if (entity is null)
                return null;
            return new BlogRequest
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Title = entity.Title.Trim(),
                Content = entity.Content,
                Rating = entity.Rating,
                PublishDate = entity.PublishDate,
                TagIds = entity.TagIds
            };
        }

        public BlogResponse Item(int id)
        {
            var entity = Query().SingleOrDefault(g => g.Id == id);
            if (entity is null)
                return null;
            return new BlogResponse
            {
                Id = entity.Id,
                Guid = entity.Guid,

                UserId = entity.UserId,
                Title = entity.Title,
                Content = entity.Content,
                Rating = entity.Rating,
                PublishDate = entity.PublishDate,

                PublishDateF = entity.PublishDate.HasValue ? entity.PublishDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                RatingF = entity.Rating.HasValue ? entity.Rating.Value.ToString("C2") : string.Empty,

                User = entity.User.UserName,
                UserResponse = new UserResponse
                {
                    Guid = entity.User.Guid,
                    Id = entity.User.Id,
                    UserName = entity.User.UserName
                },

                TagIds = entity.TagIds,

                Tags = string.Join("<br>", entity.BlogTags.OrderBy(gt => gt.Tag.Name).Select(gt => gt.Tag.Name)),
                TagsResponse = entity.BlogTags.OrderBy(gt => gt.Tag.Name).Select(gt => new TagResponse
                {
                    Guid = gt.Tag.Guid,
                    Id = gt.Tag.Id,
                    Name = gt.Tag.Name
                }).ToList()
            };
        }

        public List<BlogResponse> List()
        {
            return Query().Select(g => new BlogResponse
            {
                Id = g.Id,
                Guid = g.Guid,

                UserId = g.UserId,
                Title = g.Title,
                Content = g.Content,
                Rating = g.Rating,
                PublishDate = g.PublishDate,

                PublishDateF = g.PublishDate.HasValue ? g.PublishDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                RatingF = g.Rating.HasValue ? g.Rating.Value.ToString("C2") : string.Empty,

                User = g.User.UserName,
                UserResponse = new UserResponse
                {
                    Guid = g.User.Guid,
                    Id = g.User.Id,
                    UserName = g.User.UserName
                },


                TagIds = g.TagIds,

                Tags = string.Join("<br>", g.BlogTags.OrderBy(gt => gt.Tag.Name).Select(gt => gt.Tag.Name)),
                TagsResponse = g.BlogTags.OrderBy(gt => gt.Tag.Name).Select(gt => new TagResponse
                {
                    Guid = gt.Tag.Guid,
                    Id = gt.Tag.Id,
                    Name = gt.Tag.Name
                }).ToList()
            }).ToList();
        }

        public CommandResponse Update(BlogRequest request)
        {
            if (Query().Any(g => g.Id != request.Id && g.Title == request.Title.Trim()))
                return Error("Blog with same title exists!");
            var entity = Query(false).SingleOrDefault(g => g.Id == request.Id);
            if (entity is null)
                return Error("Blog not found!");
            Delete(entity.BlogTags);
            entity.UserId = request.UserId ?? 0;
            entity.Title = request.Title.Trim();
            entity.Content = request.Content;
            entity.Rating = request.Rating;
            entity.PublishDate = request.PublishDate;
            entity.TagIds = request.TagIds;
            Update(entity);
            return Success("Blog updated successfully.", entity.Id);

        }
    }
}
