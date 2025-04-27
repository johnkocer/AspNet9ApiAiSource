using System.ComponentModel.DataAnnotations;

namespace TodoApi
{
  public class Todo
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }

    public bool IsDone { get; set; }
  }
}