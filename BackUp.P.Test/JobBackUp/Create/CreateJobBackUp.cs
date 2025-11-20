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

namespace BackUp.P.Test.JobBackUp.Create
{
    public class AgregarJobBackupTest
    {
        private readonly Mock<IJobBackupRepository> _jobBackupRepositoryMock;
        private readonly Mock<ILogger<JobBackupService>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly JobBackupService _jobBackupService;

        public AgregarJobBackupTest()
        {
            _jobBackupRepositoryMock = new Mock<IJobBackupRepository>();
            _loggerMock = new Mock<ILogger<JobBackupService>>();
            _configurationMock = new Mock<IConfiguration>();
            _contextMock = new Mock<IApplicationDbContext>();

            _configurationMock.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);

            _jobBackupService = new JobBackupService(
                _loggerMock.Object,
                _contextMock.Object
            );
        }

        [Fact]
        public async Task AgregarAsync_ShouldReturnFailure_WhenJobBackupIsNull()
        {
            // Arrange
            SaveJobBackupDTO? jobBackup = null;
            const string expectedMessage = "El DTO no puede ser nulo.";

            // Act
            var result = await _jobBackupService.AgregarAsync(jobBackup!);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
    }
}
