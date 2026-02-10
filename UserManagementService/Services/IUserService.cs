using UserManagementService.Models;

namespace UserManagementService.Services
{
    /// <summary>
    /// Interfaz para la gestión de usuarios y asignación de tareas.
    /// Define los contratos para la creación de usuarios y la distribución de carga de trabajo.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Obtiene todos los usuarios registrados junto con sus tareas asignadas.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// Crea un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="user">Objeto usuario con la información básica.</param>
        /// <returns>El usuario creado.</returns>
        Task<User> CreateAsync(User user);

        /// <summary>
        /// Asigna inteligentemente un ítem de trabajo a un usuario basándose en reglas de negocio:
        /// 1. Prioridad a usuarios con menos carga.
        /// 2. Validación de saturación (máx 3 tareas de alta relevancia).
        /// 3. Manejo de urgencias (fechas próximas).
        /// </summary>
        /// <param name="workItem">La tarea a asignar.</param>
        /// <returns>El usuario al que se le asignó la tarea, o null si todos están saturados.</returns>
        Task<User?> AssignWorkItemAsync(WorkItem workItem);
    }
}