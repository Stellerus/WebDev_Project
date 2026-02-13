using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebDevProject.Services;

namespace WebDevProject.Controllers;

[Authorize]
public class AssignedTasksController(ITaskService taskService, UserManager<IdentityUser> userManager) : Controller
{
    public async Task<IActionResult> Index(string statusFilter = "active", string sortBy = "duedate")
    {
        var userId = userManager.GetUserId(this.User);

        this.ViewData["StatusFilter"] = statusFilter;
        this.ViewData["SortBy"] = sortBy;

        var tasks = await taskService
            .GetAssignedTasksFilteredAsync(userId!, statusFilter)
            .ConfigureAwait(false);

        tasks = sortBy switch
        {
            "title" => tasks.OrderBy(t => t.Title).ToList(),
            "createddate" => tasks.OrderByDescending(t => t.CreatedDate).ToList(),
            _ => tasks.OrderBy(t => t.DueDate).ToList()
        };

        return this.View(tasks);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int taskId, string newStatus)
    {
        var userId = userManager.GetUserId(this.User);

        await taskService
            .UpdateTaskStatusAsync(taskId, userId!, newStatus)
            .ConfigureAwait(false);

        return this.RedirectToAction(nameof(this.Index));
    }
}