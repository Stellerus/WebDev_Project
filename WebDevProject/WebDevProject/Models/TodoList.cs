using System.ComponentModel.DataAnnotations;

namespace WebDevProject.Models;

public class TodoList
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public ICollection<TodoTask> Tasks { get; set; } = new List<TodoTask>();
}