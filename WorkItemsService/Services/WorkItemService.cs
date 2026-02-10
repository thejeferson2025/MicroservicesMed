using System.Net.Http.Json; 
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
            // Preparamos el item temporalmente
            var tempItem = new WorkItem
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Relevance = dto.Relevance,
                DueDate = dto.DueDate,
                IsCompleted = false
            };

            //  Conectamos con UserManagementService
            // En lugar de traer TODOS los usuarios, enviamos el item y dejamos que ELLOS decidan (Lógica Centralizada)
            var client = _httpClientFactory.CreateClient("UserClient");
            
            // Llamamos al endpoint "Assign"
            var response = await client.PostAsJsonAsync("api/Users/assign", tempItem);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al asignar usuario: {errorMsg}");
            }

            // Leemos la respuesta para obtener a quién se le asignó
            var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();
            
            if (responseData.TryGetProperty("assignedUser", out JsonElement assignedUserElement))
            {
                if (assignedUserElement.TryGetProperty("id", out JsonElement idElement))
                {
                    // Asignamos el ID que decidió el otro microservicio
                    tempItem.UserId = Guid.Parse(idElement.GetString()!);
                }
            }
            else
            {
                // Fallback por si la respuesta no trae el usuario
                throw new Exception("El servicio de usuarios no devolvió una asignación válida.");
            }

            // 4. Guardamos en nuestra base de datos local de WorkItems
            _context.WorkItems.Add(tempItem);
            await _context.SaveChangesAsync();

            return tempItem;
        }
    }
}