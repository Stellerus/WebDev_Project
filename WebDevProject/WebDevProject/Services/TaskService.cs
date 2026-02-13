using Microsoft.EntityFrameworkCore;
using WebDevProject.Data;
using WebDevProject.Models;

namespace WebDevProject.Services;

public class TaskService(ApplicationDbContext context) : ITaskService
{
    public async Task<List<TodoTask>> GetTasksByListAsync(int todoListId, string userId)
    {
        return await context.Tasks
            .Include(t => t.TodoList)
            .Where(t => t.TodoListId == todoListId && t.TodoList.UserId == userId)
            .OrderBy(t => t.DueDate)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<TodoTask?> GetTaskByIdAsync(int id, string userId)
    {
        return await context.Tasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == id && t.TodoList.UserId == userId)
            .ConfigureAwait(false);
    }

    public async Task CreateTaskAsync(TodoTask task)
    {
        context.Tasks.Add(task);

        await context.SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public async Task UpdateTaskAsync(TodoTask task)
    {
        context.Tasks.Update(task);

        await context.SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public async Task DeleteTaskAsync(int id, string userId)
    {
        var task = await this
            .GetTaskByIdAsync(id, userId)
            .ConfigureAwait(false);

        if (task != null)
        {
            context.Tasks.Remove(task);
            await context.SaveChangesAsync()
                .ConfigureAwait(false);
        }
    }

    public async Task<List<TodoTask>> GetOverdueTasksAsync(string userId)
    {
        return await context.Tasks
            .Include(t => t.TodoList)
            .Where(t => t.TodoList.UserId == userId && t.DueDate < DateTime.UtcNow && t.Status != "Completed")
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<List<TodoTask>> GetAssignedTasksAsync(string userId)
    {
        return await context.Tasks
            .Include(t => t.TodoList)
            .Where(t => t.AssignedUserId == userId)
            .OrderBy(t => t.DueDate)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<List<TodoTask>> GetAssignedTasksFilteredAsync(string userId, string statusFilter)
    {
        var query = context.Tasks
            .Include(t => t.TodoList)
            .Where(t => t.AssignedUserId == userId);

        query = statusFilter switch
        {
            "active" => query.Where(t => t.Status != "Completed"),
            "completed" => query.Where(t => t.Status == "Completed"),
            _ => query
        };

        return await query
            .OrderBy(t => t.DueDate)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<List<TodoTask>> GetAssignedTasksSortedAsync(string userId, string sortBy)
    {
        var query = context.Tasks
            .Include(t => t.TodoList)
            .Where(t => t.AssignedUserId == userId);

        query = sortBy switch
        {
            "title" => query.OrderBy(t => t.Title),
            "createddate" => query.OrderByDescending(t => t.CreatedDate),
            _ => query.OrderBy(t => t.DueDate)
        };

        return await query
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task UpdateTaskStatusAsync(int taskId, string userId, string newStatus)
    {
        var task = await context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.AssignedUserId == userId)
            .ConfigureAwait(false);

        if (task != null)
        {
            task.Status = newStatus;
            await context
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }
    }

    public async Task<List<Tag>> GetTaskTagsAsync(int taskId, string userId)
    {
        var task = await context.Tasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.TodoList.UserId == userId)
            .ConfigureAwait(false);

        return task?.Tags?.ToList() ?? new List<Tag>();
    }

    public async Task<List<TodoTask>> SearchTasksAsync(string userId, string searchText)
    {
        return await context.Tasks
            .Include(t => t.TodoList)
            .Where(t => t.TodoList.UserId == userId &&
                       (t.Title.Contains(searchText) || t.Description.Contains(searchText)))
            .OrderBy(t => t.DueDate)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}