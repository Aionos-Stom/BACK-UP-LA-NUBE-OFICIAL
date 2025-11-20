using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BackUp.Aplication.Dtos.CloudStorage;
using BackUp.Aplication.Interfaces.IService;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Domain.Base;
using BackUp.Domain.Entities.Bac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Services.CloudStorageService
{
    public sealed class CloudStorageService : ICloudStorageService
    {
        private readonly ILogger<CloudStorageService> _logger;
        private readonly ICloudStorageRepository _cloudStorageRepository;

        public CloudStorageService(
            ILogger<CloudStorageService> logger,
            ICloudStorageRepository cloudStorageRepository)
        {
            _logger = logger;
            _cloudStorageRepository = cloudStorageRepository;
        }

        public async Task<OperationResult> ObtenerPorIdAsync(int id)
        {
            return await _cloudStorageRepository.ObtenerPorIdAsync(id);
        }

        public async Task<OperationResult> ObtenerTodosAsync()
        {
            return await _cloudStorageRepository.ObtenerTodosAsync();
        }

        public async Task<OperationResult> AgregarAsync(SaveCloudStorageDTO saveCloudStorage)
        {
            return await _cloudStorageRepository.AgregarAsync(saveCloudStorage);
        }

        public async Task<OperationResult> ActualizarAsync(UpdateCloudStorageDTO updateCloudStorage)
        {
            return await _cloudStorageRepository.ActualizarAsync(updateCloudStorage);
        }

        public async Task<OperationResult> EliminarAsync(RemoveCloudStorageDTO removeCloudStorage)
        {
            return await _cloudStorageRepository.EliminarAsync(removeCloudStorage);
        }

        public async Task<OperationResult> ObtenerPorOrganizacionAsync(int organizacionId)
        {
            return await _cloudStorageRepository.ObtenerPorOrganizacionAsync(organizacionId);
        }
    }
}
