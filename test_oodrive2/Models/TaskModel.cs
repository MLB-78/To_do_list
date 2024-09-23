using System;

namespace TaskManagerApp.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
