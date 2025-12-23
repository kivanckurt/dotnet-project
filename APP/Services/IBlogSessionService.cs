using APP.Models;

namespace APP.Services
{
    public interface IBlogSessionService
    {
        BlogRequest GetDraft(int userId, int? blogId);
        void SaveDraft(int userId, BlogRequest blog);
        void ClearDraft(int userId, int? blogId);
    }
}

