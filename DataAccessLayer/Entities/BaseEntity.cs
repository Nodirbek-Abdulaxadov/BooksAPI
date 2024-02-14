using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities;

public abstract class BaseEntity
{
    [Key, Required]
    public int Id { get; set; }
}