using System.ComponentModel.DataAnnotations;

namespace ManagingRestaurant.Areas.Role.Models;

public class CreateRoleModel
{
    [Required]
    public string RoleName { get; set; }
}