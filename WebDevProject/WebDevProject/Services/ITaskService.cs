using WebDevProject.Models;

namespace WebDevProject.Services;

public interface ITaskService
{
    Task<List<TodoTask>> GetTasksByListAsync(int todoListId, string userId);

    Task<TodoTask?> GetTaskByIdAsync(int id, string userId);

    Task CreateTaskAsync(TodoTask task);

    Task UpdateTaskAsync(TodoTask task);

    Task DeleteTaskAsync(int id, string userId);

    Task<List<TodoTask>> GetOverdueTasksAsync(string userId);

    Task<List<TodoTask>> GetAssignedTasksAsync(string userId);

    Task<List<TodoTask>> GetAssignedTasksFilteredAsync(string userId, string statusFilter);

    Task<List<TodoTask>> GetAssignedTasksSortedAsync(string userId, string sortBy);

    Task UpdateTaskStatusAsync(int taskId, string userId, string newStatus);

    Task<List<Tag>> GetTaskTagsAsync(int taskId, string userId);

    Task<List<TodoTask>> SearchTasksAsync(string userId, string searchText);
}