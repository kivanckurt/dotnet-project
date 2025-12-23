using System.Collections.Generic;
using System.Linq;
using APP.Models;
using CORE.APP.Services.Session.MVC;

namespace APP.Services
{
    public class BlogSessionService : IBlogSessionService
    {
        const string SESSIONKEY = "blog-drafts";

        private readonly SessionServiceBase _sessionService;

        public BlogSessionService(SessionServiceBase sessionService)
        {
            _sessionService = sessionService;
        }

        private class BlogDraft
        {
            public int UserId { get; set; }
            public int BlogId { get; set; }
            public BlogRequest Request { get; set; }
        }

        private List<BlogDraft> GetAllDrafts()
        {
            return _sessionService.GetSession<List<BlogDraft>>(SESSIONKEY) ?? new List<BlogDraft>();
        }

        private void SaveAllDrafts(List<BlogDraft> drafts)
        {
            _sessionService.SetSession(SESSIONKEY, drafts);
        }

        public BlogRequest GetDraft(int userId, int? blogId)
        {
            var key = blogId ?? 0;
            var drafts = GetAllDrafts();
            var draft = drafts.FirstOrDefault(d => d.UserId == userId && d.BlogId == key);
            return draft?.Request;
        }

        public void SaveDraft(int userId, BlogRequest blog)
        {
            if (blog == null)
                return;

            var key = blog.Id > 0 ? blog.Id : 0;
            var drafts = GetAllDrafts();

            drafts.RemoveAll(d => d.UserId == userId && d.BlogId == key);

            drafts.Add(new BlogDraft
            {
                UserId = userId,
                BlogId = key,
                Request = blog
            });

            SaveAllDrafts(drafts);
        }

        public void ClearDraft(int userId, int? blogId)
        {
            var key = blogId ?? 0;
            var drafts = GetAllDrafts();

            drafts.RemoveAll(d => d.UserId == userId && d.BlogId == key);

            SaveAllDrafts(drafts);
        }
    }
}

