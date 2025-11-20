using BackUp.Aplication.Services.JobBackupService;
using BackUp.Domain.Entities.Bac;
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

namespace BackUp.Aplication.Test.JobBackUp.JobBackUpSevice
{
    public class JobBackUpS
    {
        private readonly Mock<IJobBackupRepository> _jobBackupRepositoryMock;
        private readonly Mock<ILogger<JobBackupService>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly JobBackupService _jobBackupService;

        public JobBackUpS()
        {
            _jobBackupRepositoryMock = new Mock<IJobBackupRepository>();
            _loggerMock = new Mock<ILogger<JobBackupService>>();
            _configurationMock = new Mock<IConfiguration>();
            _contextMock = new Mock<IApplicationDbContext>();

            _jobBackupService = new JobBackupService(
                _loggerMock.Object,
                _contextMock.Object
            );
        }

        [Fact]
        public async Task EjecutarJobAsync_ShouldReturnFailure_WhenJobNotFound()
        {
            // Arrange
            int jobId = 999;
            const string expectedMessage = "Job no encontrado.";

            _contextMock.Setup(x => x.JobBackup.FindAsync(jobId))
                .ReturnsAsync((JobBackup)null!);

            // Act
            var result = await _jobBackupService.EjecutarJobAsync(jobId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
    }
}
