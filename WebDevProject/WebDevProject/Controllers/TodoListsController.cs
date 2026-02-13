using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebDevProject.Models;
using WebDevProject.Services;

namespace WebDevProject.Controllers
{
    [Authorize]
    public class TodoListsController(ITodoListService todoListService, UserManager<IdentityUser> userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(this.User);
            var todoLists = await todoListService
                .GetUserTodoListsAsync(userId!)
                .ConfigureAwait(false);

            return this.View(todoLists);
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodoList todoList)
        {
            if (todoList == null)
            {
                return this.BadRequest();
            }

            if (this.ModelState.IsValid)
            {
                var userId = userManager.GetUserId(this.User);
                todoList.UserId = userId!;

                await todoListService.CreateTodoListAsync(todoList).ConfigureAwait(false);

                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(todoList);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = userManager.GetUserId(this.User);

            var todoList = await todoListService
                .GetTodoListByUserIdAsync(id, userId!)
                .ConfigureAwait(false);

            if (todoList == null)
            {
                return this.NotFound();
            }

            return this.View(todoList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TodoList todoList)
        {
            if (todoList == null)
            {
                return this.BadRequest();
            }

            if (id != todoList.Id)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                var userId = userManager.GetUserId(this.User);
                var existingList = await todoListService
                    .GetTodoListByUserIdAsync(id, userId!)
                    .ConfigureAwait(false);

                if (existingList == null)
                {
                    return this.NotFound();
                }

                existingList.Title = todoList.Title;
                existingList.Description = todoList.Description;

                await todoListService
                    .UpdateTodoListAsync(existingList)
                    .ConfigureAwait(false);

                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(todoList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = userManager.GetUserId(this.User);

            await todoListService
                .DeleteTodoListAsync(id, userId!)
                .ConfigureAwait(false);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}