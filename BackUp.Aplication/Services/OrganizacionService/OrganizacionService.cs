using BackUp.Aplication.Dtos.organizacion;
using BackUp.Aplication.Interfaces.Base;
using BackUp.Aplication.Interfaces.IService;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Domain.Base;
using BackUp.Domain.Entities.Bac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace BackUp.Aplication.Services.OrganizacionService
{
    public sealed class OrganizacionService : IOrganizacionService
    {
        private readonly IOrganizacionRepository _organizacionRepository;

        public OrganizacionService(IOrganizacionRepository organizacionRepository)
        {
            _organizacionRepository = organizacionRepository;
        }

        public async Task<OperationResult> ObtenerPorIdAsync(int id)
        {
            return await _organizacionRepository.ObtenerPorIdAsync(id);
        }

        public async Task<OperationResult> ObtenerTodosAsync()
        {
            return await _organizacionRepository.ObtenerTodosAsync();
        }

        public async Task<OperationResult> AgregarAsync(SaveOrganizacionDTO saveOrganizacion)
        {
            // Validaciones adicionales
            if (string.IsNullOrWhiteSpace(saveOrganizacion.Nombre))
                return OperationResult.Failure("El nombre es requerido");

            if (saveOrganizacion.MaxUsuarios <= 0)
                return OperationResult.Failure("El número máximo de usuarios debe ser mayor a 0");

            return await _organizacionRepository.AgregarAsync(saveOrganizacion);
        }

        public async Task<OperationResult> ActualizarAsync(UpdateOrganizacionDTO updateOrganizacion)
        {
            if (updateOrganizacion.Id <= 0)
                return OperationResult.Failure("ID de organización inválido");

            if (string.IsNullOrWhiteSpace(updateOrganizacion.Nombre))
                return OperationResult.Failure("El nombre es requerido");

            return await _organizacionRepository.ActualizarAsync(updateOrganizacion);
        }

        public async Task<OperationResult> EliminarAsync(RemoveOrganizacionDTO removeOrganizacion)
        {
            if (removeOrganizacion.Id <= 0)
                return OperationResult.Failure("ID de organización inválido");

            return await _organizacionRepository.EliminarAsync(removeOrganizacion);
        }
    }
}

