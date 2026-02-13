using System.ComponentModel.DataAnnotations;

namespace WebDevProject.Models;

public class TodoTask
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Due Date")]
    public DateTime DueDate { get; set; }

    public DateTime CreatedDate { get; set; }

    [Required]
    public string Status { get; set; }

    public int TodoListId { get; set; }

    public TodoList TodoList { get; set; }

    public string AssignedUserId { get; set; }

    public List<Tag> Tags { get; set; }

    public bool IsOverdue => DueDate < DateTime.UtcNow && Status != "Completed";

    public List<Comment> Comments { get; set; } = new List<Comment>();
}