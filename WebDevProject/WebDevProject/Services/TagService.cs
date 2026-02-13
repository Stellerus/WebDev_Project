using Microsoft.EntityFrameworkCore;
using WebDevProject.Data;
using WebDevProject.Models;

namespace WebDevProject.Services;

public class TagService(ApplicationDbContext context) : ITagService
{
    public async Task<List<Tag>> GetUserTagsAsync(string userId)
    {
        return await context.Tags
            .Where(t => t.UserId == userId)
            .OrderBy(t => t.Name)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        context.Tags.Add(tag);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return tag;
    }

    public async Task DeleteTagAsync(int id, string userId)
    {
        var tag = await context.Tags
            .FirstOrDefaultAsync(t => (t.Id == id) && (t.UserId == userId))
            .ConfigureAwait(false);

        if (tag != null)
        {
            context.Tags.Remove(tag);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public async Task AddTagToTaskAsync(int taskId, int tagId, string userId)
    {
        var task = await context.Tasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => (t.Id == taskId) && (t.TodoList.UserId == userId))
            .ConfigureAwait(false);

        var tag = await context.Tags
            .FirstOrDefaultAsync(t => t.Id == tagId && (t.UserId == userId))
            .ConfigureAwait(false);

        if (task != null && tag != null)
        {
            if (task.Tags == null)
            {
                task.Tags = new List<Tag>();
            }

            if (!task.Tags.Any(t => t.Id == tagId))
            {
                task.Tags.Add(tag);
                await context
                    .SaveChangesAsync()
                    .ConfigureAwait(false);
            }
        }
    }

    public async Task RemoveTagFromTaskAsync(int taskId, int tagId, string userId)
    {
        var task = await context.Tasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.TodoList.UserId == userId)
            .ConfigureAwait(false);

        if (task != null && task.Tags != null)
        {
            var tagToRemove = task.Tags.FirstOrDefault(t => t.Id == tagId);
            if (tagToRemove != null)
            {
                task.Tags.Remove(tagToRemove);
                await context
                    .SaveChangesAsync()
                    .ConfigureAwait(false);
            }
        }
    }

    public async Task<List<TodoTask>> GetTasksByTagAsync(int tagId, string userId)
    {
        return await context.Tasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .Where(t => t.TodoList.UserId == userId &&
                       t.Tags != null &&
                       t.Tags.Any(tag => tag.Id == tagId))
            .OrderBy(t => t.DueDate)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}