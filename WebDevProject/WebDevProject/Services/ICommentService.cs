using WebDevProject.Models;

namespace WebDevProject.Services;

public interface ICommentService
{
    Task<List<Comment>> GetTaskCommentsAsync(int taskId, string userId);

    Task<Comment?> GetCommentByIdAsync(int commentId, string userId);

    Task<Comment> CreateCommentAsync(Comment comment);

    Task UpdateCommentAsync(Comment comment);

    Task DeleteCommentAsync(int commentId, string userId);
}