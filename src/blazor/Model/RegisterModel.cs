namespace blazor.Model;

public class RegisterModel
{
    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }
    
    public string? Role { get; set; } = Roles.GetRoles[1].Name;
    

    
}

public class RoleModel
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    
    
}

public static class Roles
{
    public static readonly List<RoleModel> GetRoles =
    [
        new RoleModel { Id = "1", Name = "Admin" },
        new RoleModel { Id = "2", Name = "User" }
    ];
} 