using System.ComponentModel.DataAnnotations;
public class Todo
{
  [Key]
  public int Id { get; set; }

  [Required]
  [MaxLength(200)]
  public required string  Title { get; set; }

  public bool IsCompleted { get; set; }
}
