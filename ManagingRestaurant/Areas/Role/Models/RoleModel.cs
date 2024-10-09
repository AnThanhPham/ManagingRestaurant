using Microsoft.AspNetCore.Identity;

namespace ManagingRestaurant.Areas.Role.Models;

public class RoleModel : IdentityRole
{
    public List<string> Claims { get; set; }
}
