using Microsoft.AspNetCore.Identity;

namespace ManagingRestaurant.Areas.Role.Models;

public class EditRoleModel
{
    public IdentityRole Role { get; set; }
    public List<IdentityRoleClaim<string>> RoleClaims { get; set; }
}