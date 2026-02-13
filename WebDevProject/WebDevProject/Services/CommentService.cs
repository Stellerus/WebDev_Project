using Microsoft.EntityFrameworkCore;
using WebDevProject.Data;
using WebDevProject.Models;

namespace WebDevProject.Services;

public class CommentService(ApplicationDbContext context) : ICommentService
{
    public async Task<List<Comment>> GetTaskCommentsAsync(int taskId, string userId)
    {
        return await context.Comments
            .Include(c => c.Task)
            .ThenInclude(t => t.TodoList)
            .Where(c => c.TaskId == taskId && c.Task.TodoList.UserId == userId)
            .OrderByDescending(c => c.CreatedDate)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<Comment?> GetCommentByIdAsync(int commentId, string userId)
    {
        return await context.Comments
            .Include(c => c.Task)
            .ThenInclude(t => t.TodoList)
            .FirstOrDefaultAsync(c => c.Id == commentId && c.Task.TodoList.UserId == userId)
            .ConfigureAwait(false);
    }

    public async Task<Comment> CreateCommentAsync(Comment comment)
    {
        context.Comments.Add(comment);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return comment;
    }

    public async Task UpdateCommentAsync(Comment comment)
    {
        ArgumentNullException.ThrowIfNull(comment, nameof(comment));

        comment.ModifiedDate = DateTime.UtcNow;
        context.Comments.Update(comment);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteCommentAsync(int commentId, string userId)
    {
        var comment = await this
            .GetCommentByIdAsync(commentId, userId)
            .ConfigureAwait(false);
        if (comment != null)
        {
            context.Comments.Remove(comment);
            await context
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }
    }
}