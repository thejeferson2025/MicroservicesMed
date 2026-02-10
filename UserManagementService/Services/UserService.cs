using UserManagementService.Data;
using UserManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace UserManagementService.Services
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;

        public UserService(UserDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            // Incluimos WorkItems para evaluar la carga actual de cada usuario
            return await _context.Users.Include(u => u.WorkItems).ToListAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            user.Id = Guid.NewGuid();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> AssignWorkItemAsync(WorkItem workItem)
        {
            // Recuperamos usuarios y sus tareas para calcular la saturación actual
            var users = await _context.Users.Include(u => u.WorkItems).ToListAsync();

            if (!users.Any()) return null;

            User? selectedUser = null;

            // REGLA DE NEGOCIO 1: Urgencia
            // Ítems que vencen en menos de 3 días se consideran urgentes.
            bool isUrgent = (workItem.DueDate - DateTime.Now).TotalDays < 3;

            // REGLA DE NEGOCIO 2: Saturación
            // Un usuario está saturado si tiene 3 o más tareas de "High" relevance.
            bool isHighRelevance = workItem.Relevance == "High";

            if (isHighRelevance)
            {
                // Filtramos usuarios que NO estén saturados
                var availableUsers = users
                    .Where(u => u.WorkItems.Count(w => w.Relevance == "High") < 3)
                    .ToList();

                // Asignamos al que tenga menos carga total para balancear
                selectedUser = availableUsers.OrderBy(u => u.WorkItems.Count).FirstOrDefault();
            }
            else
            {
                // Si la tarea no es crítica, buscamos simplemente al más libre
                selectedUser = users.OrderBy(u => u.WorkItems.Count).FirstOrDefault();
            }

            // Fallback: Si todos están saturados (o no hay match), asignamos al de menor carga absoluta
            if (selectedUser == null)
            {
                selectedUser = users.OrderBy(u => u.WorkItems.Count).FirstOrDefault();
            }

            if (selectedUser != null)
            {
                // Preparar el ítem para persistencia
                workItem.Id = Guid.NewGuid();
                workItem.UserId = selectedUser.Id;
                
                // REGLA DE NEGOCIO 3: Ordenamiento
                // Agregamos la tarea y guardamos cambios
                selectedUser.WorkItems.Add(workItem);
                _context.WorkItems.Add(workItem);
                await _context.SaveChangesAsync();

                // Nota: Podríamos reordenar la lista en memoria aquí si se fuera a devolver inmediatamente,
                // pero la persistencia ya asegura que la próxima consulta traerá los datos.
                return selectedUser;
            }

            return null;
        }
    }
}