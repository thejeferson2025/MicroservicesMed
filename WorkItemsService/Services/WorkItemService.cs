using System.Text.Json;
using WorkItemsService.Data;
using WorkItemsService.Dtos;
using WorkItemsService.Models;
using Microsoft.EntityFrameworkCore;

namespace WorkItemsService.Services
{
    public interface IWorkItemService
    {
        Task<WorkItem> CreateAndAssignAsync(WorkItemCreateDto dto);
        Task<IEnumerable<WorkItem>> GetAllAsync();
    }

    public class WorkItemService : IWorkItemService
    {
        private readonly WorkItemsDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public WorkItemService(WorkItemsDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IEnumerable<WorkItem>> GetAllAsync()
        {
            return await _context.WorkItems.ToListAsync();
        }

        public async Task<WorkItem> CreateAndAssignAsync(WorkItemCreateDto dto)
        {
            // Obtener lista de usuarios de UserManagementService
            var client = _httpClientFactory.CreateClient("UserClient");
            var usersResponse = await client.GetAsync(""); 
            
            if (!usersResponse.IsSuccessStatusCode)
                throw new Exception("No se pudo conectar con el servicio de Usuarios.");

            var content = await usersResponse.Content.ReadAsStringAsync();
            var allUsers = JsonSerializer.Deserialize<List<UserDto>>(content, _jsonOptions);

            if (allUsers == null || !allUsers.Any())
                throw new Exception("No hay usuarios disponibles para asignar.");

            // Obtener estadísticas actuales de usuarios
            var currentTasks = await _context.WorkItems
                                             .Where(w => !w.IsCompleted && w.UserId != null)
                                             .ToListAsync();

            // Aplicar Algoritmo de Asignación
            Guid selectedUserId;
            
            // verificar si la fecha está próxima a vencer
            bool isPanicMode = (dto.DueDate - DateTime.Now).TotalDays < 3;

            if (isPanicMode)
            {
                //  Asignar al usuario con menos ítems
                selectedUserId = allUsers
                    .OrderBy(u => currentTasks.Count(t => t.UserId == u.Id))
                    .First().Id;
            }
            else
            {
                //  Filtrar saturados y asignar al de menor carga
                var availableUsers = allUsers.Where(u => 
                {
                    int highRelevanceCount = currentTasks.Count(t => t.UserId == u.Id && t.Relevance == "High");
                    return highRelevanceCount <= 3; 
                }).ToList();

                if (!availableUsers.Any())
                {
                    // validación y asignación al que menos tenga en general
                    selectedUserId = allUsers
                        .OrderBy(u => currentTasks.Count(t => t.UserId == u.Id))
                        .First().Id;
                }
                else
                {
                    // elegir al que menos pendientes tenga en total
                    selectedUserId = availableUsers
                        .OrderBy(u => currentTasks.Count(t => t.UserId == u.Id))
                        .First().Id;
                }
            }

            // Guarda la nueva tarea
            var workItem = new WorkItem
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Relevance = dto.Relevance,
                DueDate = dto.DueDate,
                IsCompleted = false,
                UserId = selectedUserId
            };

            _context.WorkItems.Add(workItem);
            await _context.SaveChangesAsync();

            return workItem;
        }
    }
}