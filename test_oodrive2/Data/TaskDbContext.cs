using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Models;
using System.IO;

namespace TaskManagerApp.Data
{
    public class TaskDbContext : DbContext
    {
        public DbSet<TaskModel> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tasks.db");
            optionsBuilder.UseSqlite($"Filename={dbPath}");
            Console.WriteLine("Database path: " + dbPath);

        }
    }
}
