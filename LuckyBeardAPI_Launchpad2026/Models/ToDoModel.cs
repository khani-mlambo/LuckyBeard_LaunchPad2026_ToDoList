using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuckyBeardAPI_Launchpad2026.Models
{
    public class ToDoModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ToDoId { get; set; }

        public string Title { get; set; } 

       
        [DataType(DataType.MultilineText)]
        public string ToDoDescription { get; set; }

        public DateTime Created_At { get; set; } = DateTime.UtcNow;

        public DateTime Updated_At { get; set; }

        public bool IsDeleted { get; set; } = false;

        public TodoStatus Status { get; set; } 

        [ForeignKey(nameof(User))]
        public string UserToDoId { get; set; }
        public UserModel User { get; set; } = default!;
    }

    public enum TodoStatus
    {
        Pending = 0,
        In_Progress = 1,
        Done =  2
    }

}
