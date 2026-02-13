using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebDevProject.Services;

namespace WebDevProject.Controllers;

[Authorize]
public class SearchController(ITaskService taskService, UserManager<IdentityUser> userManager) : Controller
{
    public IActionResult Index()
    {
        return this.View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            this.ModelState.AddModelError(string.Empty, "Please enter search text");
            return this.View();
        }

        var userId = userManager.GetUserId(this.User);

        var tasks = await taskService
            .SearchTasksAsync(userId!, searchText)
            .ConfigureAwait(false);

        this.ViewBag.SearchText = searchText;
        return this.View("Results", tasks);
    }
}