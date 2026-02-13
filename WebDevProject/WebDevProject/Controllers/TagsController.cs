using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebDevProject.Models;
using WebDevProject.Services;

namespace WebDevProject.Controllers;

[Authorize]
public class TagsController(ITagService tagService, UserManager<IdentityUser> userManager) : Controller
{
    public async Task<IActionResult> Index()
    {

        var userId = userManager.GetUserId(this.User);

        if (userId == null)
        {
            return this.Unauthorized();
        }

        var tags = await tagService
            .GetUserTagsAsync(userId)
            .ConfigureAwait(false);

        return this.View(tags);
    }

    public IActionResult Create()
    {
        return this.View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            var userId = userManager.GetUserId(this.User);
            var tag = new Tag
            {
                Name = name.Trim(),
                UserId = userId!,
            };

            await tagService
                .CreateTagAsync(tag)
                .ConfigureAwait(false);

            return this.RedirectToAction(nameof(this.Index));
        }

        this.ModelState.AddModelError(string.Empty, "Tag name is required");
        return this.View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = userManager.GetUserId(this.User);

        if (userId == null)
        {
            return this.View(new List<Tag>());
        }

        await tagService
            .DeleteTagAsync(id, userId)
            .ConfigureAwait(false);

        return this.RedirectToAction(nameof(this.Index));
    }

    public async Task<IActionResult> TasksByTag(int id)
    {
        var userId = userManager.GetUserId(this.User);

        if (userId == null)
        {
            return this.View(new List<Tag>());
        }

        var tasks = await tagService
            .GetTasksByTagAsync(id, userId)
            .ConfigureAwait(false);

        var tags = await tagService
            .GetUserTagsAsync(userId)
            .ConfigureAwait(false);

        var tag = tags.FirstOrDefault(t => t.Id == id);

        this.ViewBag.TagName = tag?.Name ?? "Tag";

        return this.View(tasks);
    }
}