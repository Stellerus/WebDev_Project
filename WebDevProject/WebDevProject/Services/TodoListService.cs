using Microsoft.EntityFrameworkCore;
using WebDevProject.Data;
using WebDevProject.Models;

namespace WebDevProject.Services;

public class TodoListService(ApplicationDbContext context) : ITodoListService
{
    public async Task<List<TodoList>> GetUserTodoListsAsync(string userId)
    {
        return await context.TodoLists
            .Include(tl => tl.Tasks)
            .Where(tl => tl.UserId == userId)
            .OrderBy(tl => tl.Title)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<TodoList?> GetTodoListByUserIdAsync(int id, string userId)
    {
        return await context.TodoLists
            .Include(tl => tl.Tasks)
            .FirstOrDefaultAsync(tl => (tl.Id == id) && (tl.UserId == userId))
            .ConfigureAwait(false);
    }

    public async Task CreateTodoListAsync(TodoList todoList)
    {
        context.TodoLists.Add(todoList);

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public async Task UpdateTodoListAsync(TodoList todoList)
    {
        context.TodoLists.Update(todoList);

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public async Task DeleteTodoListAsync(int id, string userId)
    {
        var todoList = await this
            .GetTodoListByUserIdAsync(id, userId)
            .ConfigureAwait(false);

        if (todoList == null)
        {
            return;
        }

        context.TodoLists.Remove(todoList);

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }
}