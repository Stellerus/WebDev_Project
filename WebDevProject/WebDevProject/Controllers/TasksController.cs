using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebDevProject.Models;
using WebDevProject.Services;

namespace WebDevProject.Controllers;

[Authorize]
public class TasksController(ITaskService taskService, ITodoListService todoListService,
                      UserManager<IdentityUser> userManager, ITagService tagService,
                      ICommentService commentService) : Controller
{
    public async Task<IActionResult> Index(int listId)
    {
        var userId = userManager.GetUserId(this.User);
        var todoList = await todoListService
            .GetTodoListByUserIdAsync(listId, userId!)
            .ConfigureAwait(false);

        if (todoList == null)
        {
            return this.NotFound();
        }

        this.ViewData["TodoList"] = todoList;
        var tasks = await taskService
            .GetTasksByListAsync(listId, userId!)
            .ConfigureAwait(false);

        return this.View(tasks);
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = userManager.GetUserId(this.User);
        var task = await taskService
            .GetTaskByIdAsync(id, userId!)
            .ConfigureAwait(false);

        if (task == null)
        {
            return this.NotFound();
        }

        this.ViewBag.CommentService = commentService;
        this.ViewBag.CurrentUserId = userId;

        return this.View(task);
    }

    public async Task<IActionResult> Create(int listId)
    {
        var userId = userManager.GetUserId(this.User);

        var todoList = await todoListService
            .GetTodoListByUserIdAsync(listId, userId!)
            .ConfigureAwait(false);

        if (todoList == null)
        {
            return this.NotFound();
        }

        var task = new TodoTask
        {
            TodoListId = listId,
            AssignedUserId = userId!,
            DueDate = DateTime.UtcNow.AddDays(1),
            Status = "Not Started",
        };

        var allTags = await tagService
            .GetUserTagsAsync(userId!)
            .ConfigureAwait(false);

        this.ViewBag.AllTags = allTags;

        return this.View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoTask task, Collection<int> selectedTagIds)
    {
        if (task == null)
        {
            return this.BadRequest();
        }

        var userId = userManager.GetUserId(this.User);

        var todoList = await todoListService
            .GetTodoListByUserIdAsync(task.TodoListId, userId!)
            .ConfigureAwait(false);

        if (todoList == null)
        {
            return this.NotFound();
        }

        task.AssignedUserId = userId!;
        task.CreatedDate = DateTime.UtcNow;

        this.ModelState.Remove("TodoList");
        this.ModelState.Remove("Tags");

        if (this.ModelState.IsValid)
        {
            await taskService
                .CreateTaskAsync(task)
                .ConfigureAwait(false);

            if (selectedTagIds != null)
            {
                foreach (var tagId in selectedTagIds)
                {
                    await tagService
                        .AddTagToTaskAsync(task.Id, tagId, userId!)
                        .ConfigureAwait(false);
                }
            }

            return this.RedirectToAction(nameof(this.Index), new { listId = task.TodoListId });
        }

        var allTags = await tagService
            .GetUserTagsAsync(userId!)
            .ConfigureAwait(false);

        this.ViewBag.AllTags = allTags;

        return this.View(task);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var userId = userManager.GetUserId(this.User);

        var task = await taskService
            .GetTaskByIdAsync(id, userId!)
            .ConfigureAwait(false);

        if (task == null)
        {
            return this.NotFound();
        }

        var allTags = await tagService
            .GetUserTagsAsync(userId!)
            .ConfigureAwait(false);

        var taskTags = await taskService
            .GetTaskTagsAsync(id, userId!)
            .ConfigureAwait(false);

        this.ViewBag.AllTags = allTags;
        this.ViewBag.TaskTags = taskTags.Select(t => t.Id).ToList();

        return this.View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TodoTask task, Collection<int> selectedTagIds)
    {
        if (task == null || selectedTagIds == null)
        {
            return this.View(task);
        }

        if (id != task.Id)
        {
            return this.NotFound();
        }

        var userId = userManager.GetUserId(this.User);

        var existingTask = await taskService
            .GetTaskByIdAsync(id, userId!)
            .ConfigureAwait(false);

        if (existingTask == null)
        {
            return this.NotFound();
        }

        this.ModelState.Remove("TodoList");
        this.ModelState.Remove("AssignedUserId");
        this.ModelState.Remove("CreatedDate");
        this.ModelState.Remove("Tags");

        if (this.ModelState.IsValid)
        {
            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.Status = task.Status;

            await taskService
                .UpdateTaskAsync(existingTask)
                .ConfigureAwait(false);

            var currentTags = await taskService
                .GetTaskTagsAsync(id, userId!)
                .ConfigureAwait(false);

            var currentTagIds = currentTags
                .Select(t => t.Id)
                .ToList();

            if (selectedTagIds != null)
            {
                foreach (var tagId in selectedTagIds)
                {
                    if (!currentTagIds.Contains(tagId))
                    {
                        await tagService
                            .AddTagToTaskAsync(id, tagId, userId!)
                            .ConfigureAwait(false);
                    }
                }
            }

            foreach (var currentTagId in currentTagIds)
            {
                if (selectedTagIds == null || !selectedTagIds.Contains(currentTagId))
                {
                    await tagService
                        .RemoveTagFromTaskAsync(id, currentTagId, userId!)
                        .ConfigureAwait(false);
                }
            }

            return this.RedirectToAction(nameof(this.Index), new { listId = existingTask.TodoListId });
        }

        var allTags = await tagService
            .GetUserTagsAsync(userId!)
            .ConfigureAwait(false);

        var taskTags = await taskService
            .GetTaskTagsAsync(id, userId!)
            .ConfigureAwait(false);

        this.ViewBag.AllTags = allTags;
        this.ViewBag.TaskTags = taskTags.Select(t => t.Id).ToList();

        return this.View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = userManager.GetUserId(this.User);

        var task = await taskService
            .GetTaskByIdAsync(id, userId!)
            .ConfigureAwait(false);

        if (task == null)
        {
            return this.NotFound();
        }

        var listId = task.TodoListId;

        await taskService
            .DeleteTaskAsync(id, userId!)
            .ConfigureAwait(false);

        return this.RedirectToAction(nameof(this.Index), new { listId });
    }
}