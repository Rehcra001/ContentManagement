namespace ContentManagement.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int PostCommentId { get; set; }
        public PostCommentsModel PostComment { get; set; } = new PostCommentsModel();
    }
}
