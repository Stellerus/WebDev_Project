using System.ComponentModel.DataAnnotations;

namespace WebDevProject.Models;

public class Tag
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    public string UserId { get; set; }
}