
namespace LuckyBeardAPI_Launchpad2026.Models
{

    public class UserConstants
    {
        public static List<UserModel> Users = new List<UserModel>()
        {
           new UserModel
            {
                UserId = Guid.NewGuid().ToString(),
                Name = "John",
                Surname = "Doe",
                Email = "john@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Password123"),
                IsDeleted = false
            },
            new UserModel
            {
                UserId = Guid.NewGuid().ToString(),
                Name = "Sarah",
                Surname = "Nkosi",
                Email = "sarah@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Secret456"),
                IsDeleted = false
            },
            new UserModel
            {
                UserId = Guid.NewGuid().ToString(),
                Name = "Michael",
                Surname = "Dlamini",
                Email = "michael@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin789"),
                IsDeleted = false
            }
        };


    }
}
