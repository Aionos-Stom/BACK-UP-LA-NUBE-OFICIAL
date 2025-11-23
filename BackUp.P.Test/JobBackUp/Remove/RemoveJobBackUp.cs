using BackUp.Aplication.Services.JobBackupService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using BackUp.Aplication.Dtos.JobBackup;
using BackUp.Aplication.Interfaces.Base;
using BackUp.Aplication.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.P.Test.JobBackUp.Remove
{
    public class EliminarJobBackupTest
    {
        private readonly Mock<IJobBackupRepository> _jobBackupRepositoryMock;
        private readonly Mock<ILogger<JobBackupService>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly JobBackupService _jobBackupService;

        public EliminarJobBackupTest()
        {
            _jobBackupRepositoryMock = new Mock<IJobBackupRepository>();
            _loggerMock = new Mock<ILogger<JobBackupService>>();
            _configurationMock = new Mock<IConfiguration>();
            _contextMock = new Mock<IApplicationDbContext>();

            _configurationMock.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);

            _jobBackupService = new JobBackupService(_jobBackupRepositoryMock.Object);
        }

        [Fact]
        public async Task EliminarAsync_ShouldReturnFailure_WhenIdIsInvalid()
        {
            // Arrange
            var jobBackup = new RemoveJobBackupDTO { Id = 0 };
            string expectedMessage = "El ID debe ser mayor que cero.";

            // Act
            var result = await _jobBackupService.EliminarAsync(jobBackup);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
    }
}

