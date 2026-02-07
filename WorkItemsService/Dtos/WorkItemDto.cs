namespace WorkItemsService.Dtos
{
    // Lo que se envia para crear una tarea
    public record WorkItemCreateDto(
        string Title,
        string Description,
        string Relevance, 
        DateTime DueDate
    );

    // DTO auxiliar para traer usuarios de UserManagementService
    public record UserDto(Guid Id, string Name, string Email);
}