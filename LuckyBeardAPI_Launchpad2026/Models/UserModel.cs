using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuckyBeardAPI_Launchpad2026.Models
{
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string UserId { get; set; }
       
        public string Name { get; set; } = default!;
       
        public string Surname { get; set; } = default!;
      
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;
      
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        public bool IsDeleted { get; set; } = false;

        public ICollection<ToDoModel> TodosList { get; set; } = new List<ToDoModel>();
    }
}
