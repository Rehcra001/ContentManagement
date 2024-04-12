namespace ContentManagement.Models
{
    public class PostTagModel
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int TagId { get; set; }
        public TagModel Tag { get; set; } = new TagModel();
    }
}
