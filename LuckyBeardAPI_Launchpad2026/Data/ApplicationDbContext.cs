using LuckyBeardAPI_Launchpad2026.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace LuckyBeardAPI_Launchpad2026.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        //the "entity <>" is the model you're using (Asset), and next to it will be your table name for your database
        public DbSet<UserModel> ToDoUsers { get; set; }
        public DbSet<ToDoModel> ToDos { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            // Unique email
            b.Entity<UserModel>().HasIndex(u => u.Email).IsUnique();

            // Soft delete filters
            b.Entity<UserModel>().HasQueryFilter(u => !u.IsDeleted);
            b.Entity<ToDoModel>().HasQueryFilter(t => !t.IsDeleted);

            // Store enum as tinyint
            b.Entity<ToDoModel>().Property(t => t.Status).HasConversion<byte>(); 

            // Relationship: one User has many Todos
            b.Entity<ToDoModel>()
                .HasOne(t => t.User)
                .WithMany(u => u.TodosList)
                .HasForeignKey(t => t.UserToDoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}