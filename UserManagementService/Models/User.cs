namespace UserManagementService.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }

        // Relación para poder aplicar la lógica de "Saturación"
        public ICollection<WorkItem> WorkItems { get; set; } = new List<WorkItem>();
    }
}