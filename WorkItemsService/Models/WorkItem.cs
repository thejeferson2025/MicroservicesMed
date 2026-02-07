namespace WorkItemsService.Models
{
    public class WorkItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Relevance { get; set; } = "Low"; 
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public Guid? UserId { get; set; } // usuario asignado
    }
}