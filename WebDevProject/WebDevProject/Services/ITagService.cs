using WebDevProject.Models;

namespace WebDevProject.Services;

public interface ITagService
{
    Task<List<Tag>> GetUserTagsAsync(string userId);

    Task<Tag> CreateTagAsync(Tag tag);

    Task DeleteTagAsync(int id, string userId);

    Task AddTagToTaskAsync(int taskId, int tagId, string userId);

    Task RemoveTagFromTaskAsync(int taskId, int tagId, string userId);

    Task<List<TodoTask>> GetTasksByTagAsync(int tagId, string userId);
}