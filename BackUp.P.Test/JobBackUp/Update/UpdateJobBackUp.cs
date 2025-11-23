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


namespace BackUp.P.Test.JobBackUp.Update
{
    public class ActualizarJobBackupTest
    {
        private readonly Mock<IJobBackupRepository> _jobBackupRepositoryMock;
        private readonly Mock<ILogger<JobBackupService>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly JobBackupService _jobBackupService;

        public ActualizarJobBackupTest()
        {
            _jobBackupRepositoryMock = new Mock<IJobBackupRepository>();
            _loggerMock = new Mock<ILogger<JobBackupService>>();
            _configurationMock = new Mock<IConfiguration>();
            _contextMock = new Mock<IApplicationDbContext>();

            _configurationMock.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);

            _jobBackupService = new JobBackupService(_jobBackupRepositoryMock.Object);
        }

        [Fact]
        public async Task ActualizarAsync_ShouldReturnFailure_WhenJobBackupIsNull()
        {
            // Arrange
            UpdateJobBackupDTO? jobBackup = null;
            const string expectedMessage = "El DTO no puede ser nulo.";

            // Act
            var result = await _jobBackupService.ActualizarAsync(jobBackup!);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
    }
}

