using System.ComponentModel.DataAnnotations;

namespace WebDevProject.Models;

public class Comment
{
    public int Id { get; set; }

    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedDate { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int TaskId { get; set; }

    public TodoTask Task { get; set; } = null!;
}