using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebDevProject.Models;
using WebDevProject.Services;

namespace WebDevProject.Controllers;

[Authorize]
public class CommentsController(ICommentService commentService, UserManager<IdentityUser> userManager) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int taskId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return this.RedirectToAction("Details", "Tasks", new { id = taskId });
        }

        var userId = userManager.GetUserId(this.User);

        var comment = new Comment
        {
            Content = content.Trim(),
            UserId = userId!,
            TaskId = taskId,
            CreatedDate = DateTime.UtcNow,
        };

        await commentService
            .CreateCommentAsync(comment)
            .ConfigureAwait(false);

        return this.RedirectToAction("Details", "Tasks", new { id = taskId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            var taskId = await this
                .GetTaskIdFromComment(id)
                .ConfigureAwait(false);

            return this.RedirectToAction("Details", "Tasks", new { id = taskId });
        }

        var userId = userManager.GetUserId(this.User);

        var comment = await commentService
            .GetCommentByIdAsync(id, userId!)
            .ConfigureAwait(false);

        if (comment == null)
        {
            return this.NotFound();
        }

        comment.Content = content.Trim();
        comment.ModifiedDate = DateTime.UtcNow;

        await commentService
            .UpdateCommentAsync(comment)
            .ConfigureAwait(false);

        return this.RedirectToAction("Details", "Tasks", new { id = comment.TaskId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = userManager.GetUserId(this.User);
        var comment = await commentService
            .GetCommentByIdAsync(id, userId!)
            .ConfigureAwait(false);

        if (comment == null)
        {
            return this.NotFound();
        }

        var taskId = comment.TaskId;
        await commentService
            .DeleteCommentAsync(id, userId!)
            .ConfigureAwait(false);

        return this.RedirectToAction("Details", "Tasks", new { id = taskId });
    }

    private async Task<int> GetTaskIdFromComment(int commentId)
    {
        var comment = await commentService
            .GetCommentByIdAsync(commentId, userManager.GetUserId(this.User)!)
            .ConfigureAwait(false);

        return comment?.TaskId ?? 0;
    }
}