using BackUp.Aplication.Services.JobBackupService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using BackUp.Aplication.Interfaces.Base;
using BackUp.Aplication.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.P.Test.JobBackUp.Get
{
    public class ObtenerJobBackupTest
    {
        private readonly Mock<IJobBackupRepository> _jobBackupRepositoryMock;
        private readonly Mock<ILogger<JobBackupService>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly JobBackupService _jobBackupService;

        public ObtenerJobBackupTest()
        {
            _jobBackupRepositoryMock = new Mock<IJobBackupRepository>();
            _loggerMock = new Mock<ILogger<JobBackupService>>();
            _configurationMock = new Mock<IConfiguration>();
            _contextMock = new Mock<IApplicationDbContext>();

            _configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
                             .Returns(new Mock<IConfigurationSection>().Object);

     
            // OPCIÓN 4: Solo 1 parámetro - Repository
             _jobBackupService = new JobBackupService(_jobBackupRepositoryMock.Object);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_ShouldReturnFailure_WhenIdIsInvalid()
        {
            // Arrange
            int invalidId = 0;
            const string expectedMessage = "El ID debe ser mayor que cero.";

            // Act
            var result = await _jobBackupService.ObtenerPorIdAsync(invalidId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
    }
}

