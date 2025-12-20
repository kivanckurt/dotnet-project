using CORE.APP.Domain;
namespace APP.Domain;

public class BlogTag : Entity
{
    public int BlogId { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
    public Blog Blog { get; set; }
}