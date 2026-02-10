using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagementService.Models
{
    // Mapeamos a la tabla que ya existe 
    [Table("WorkItems")] 
    public class WorkItem
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; } = string.Empty;

        // Valores esperados: "High" o "Low"
        public string Relevance { get; set; } = "Low"; 

        public DateTime DueDate { get; set; }
        
        public bool IsCompleted { get; set; }

        public Guid? UserId { get; set; }
    }
}