using InsuranceCoreBusiness.Application.Ports.Outbound;
using InsuranceCoreBusiness.Application.UseCases;
using InsuranceCoreBusiness.Domain.Entities;
using InsuranceCoreBusiness.Domain.Enums;
using InsuranceCoreBusiness.Domain.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace InsuranceCoreBusinessTest.Application.UseCases
{
    [TestClass]
    public class OperacoesPropostaUCTest
    {
        private Mock<IPropostaRepository> _mockPropostaRepository;
        private OperacoesPropostaUC _useCase;

        [TestInitialize]
        public void Setup()
        {
            _mockPropostaRepository = new Mock<IPropostaRepository>();
            _useCase = new OperacoesPropostaUC(_mockPropostaRepository.Object);
        }

        [TestMethod]
        public async Task AprovaPropostaPorId_ValidPropostaEmAnalise_ReturnsSuccessAndUpdatesStatus()
        {
            // Arrange
            var propostaId = "proposta123";
            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.EmAnalise,
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow.AddDays(-1)
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            _mockPropostaRepository.Setup(r => r.UpdateAsync(It.IsAny<Proposta>()))
                                  .ReturnsAsync(1);

            // Act
            var result = await _useCase.AprovaPropostaPorId(propostaId);

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(EStatusProposta.Aprovada, proposta.statusProposta);
            Assert.IsTrue(proposta.dataAtualizacao > DateTime.UtcNow.AddMinutes(-1)); // Recently updated

            _mockPropostaRepository.Verify(r => r.GetByIdAsync(propostaId), Times.Once);
            _mockPropostaRepository.Verify(r => r.UpdateAsync(It.Is<Proposta>(p => 
                p.id == propostaId && 
                p.statusProposta == EStatusProposta.Aprovada)), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AprovaPropostaPorId_PropostaNotFound_ThrowsArgumentException()
        {
            // Arrange
            var propostaId = "nonexistent123";
            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync((Proposta)null);

            // Act
            await _useCase.AprovaPropostaPorId(propostaId);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(StatusPropostaInvalidoException))]
        public async Task AprovaPropostaPorId_PropostaAlreadyApproved_ThrowsStatusPropostaInvalidoException()
        {
            // Arrange
            var propostaId = "proposta123";
            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.Aprovada, // Already approved
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            // Act
            await _useCase.AprovaPropostaPorId(propostaId);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(StatusPropostaInvalidoException))]
        public async Task AprovaPropostaPorId_PropostaRejected_ThrowsStatusPropostaInvalidoException()
        {
            // Arrange
            var propostaId = "proposta123";
            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.Rejeitada, // Rejected
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            // Act
            await _useCase.AprovaPropostaPorId(propostaId);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(StatusPropostaInvalidoException))]
        public async Task AprovaPropostaPorId_PropostaContracted_ThrowsStatusPropostaInvalidoException()
        {
            // Arrange
            var propostaId = "proposta123";
            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.Contratada, // Already contracted
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            // Act
            await _useCase.AprovaPropostaPorId(propostaId);

            // Assert - Exception expected
        }

        [TestMethod]
        public async Task ReprovaPropostaPorId_ValidPropostaEmAnalise_ReturnsSuccessAndUpdatesStatus()
        {
            // Arrange
            var propostaId = "proposta123";
            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.EmAnalise,
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow.AddDays(-1)
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            _mockPropostaRepository.Setup(r => r.UpdateAsync(It.IsAny<Proposta>()))
                                  .ReturnsAsync(1);

            // Act
            var result = await _useCase.ReprovaPropostaPorId(propostaId);

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(EStatusProposta.Rejeitada, proposta.statusProposta);
            Assert.IsTrue(proposta.dataAtualizacao > DateTime.UtcNow.AddMinutes(-1)); // Recently updated

            _mockPropostaRepository.Verify(r => r.GetByIdAsync(propostaId), Times.Once);
            _mockPropostaRepository.Verify(r => r.UpdateAsync(It.Is<Proposta>(p => 
                p.id == propostaId && 
                p.statusProposta == EStatusProposta.Rejeitada)), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ReprovaPropostaPorId_PropostaNotFound_ThrowsArgumentException()
        {
            // Arrange
            var propostaId = "nonexistent123";
            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync((Proposta)null);

            // Act
            await _useCase.ReprovaPropostaPorId(propostaId);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(StatusPropostaInvalidoException))]
        public async Task ReprovaPropostaPorId_PropostaAlreadyApproved_ThrowsStatusPropostaInvalidoException()
        {
            // Arrange
            var propostaId = "proposta123";
            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.Aprovada, // Already approved
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            // Act
            await _useCase.ReprovaPropostaPorId(propostaId);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(StatusPropostaInvalidoException))]
        public async Task ReprovaPropostaPorId_PropostaAlreadyRejected_ThrowsStatusPropostaInvalidoException()
        {
            // Arrange
            var propostaId = "proposta123";
            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.Rejeitada, // Already rejected
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            // Act
            await _useCase.ReprovaPropostaPorId(propostaId);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(StatusPropostaInvalidoException))]
        public async Task ReprovaPropostaPorId_PropostaContracted_ThrowsStatusPropostaInvalidoException()
        {
            // Arrange
            var propostaId = "proposta123";
            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.Contratada, // Already contracted
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            // Act
            await _useCase.ReprovaPropostaPorId(propostaId);

            // Assert - Exception expected
        }

        [TestMethod]
        public async Task AprovaPropostaPorId_RepositoryUpdateFails_ReturnsZero()
        {
            // Arrange
            var propostaId = "proposta123";
            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.EmAnalise,
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow.AddDays(-1)
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            _mockPropostaRepository.Setup(r => r.UpdateAsync(It.IsAny<Proposta>()))
                                  .ReturnsAsync(0); // Simulate update failure

            // Act
            var result = await _useCase.AprovaPropostaPorId(propostaId);

            // Assert
            Assert.AreEqual(0, result);
            Assert.AreEqual(EStatusProposta.Aprovada, proposta.statusProposta); // Status still changed locally
            _mockPropostaRepository.Verify(r => r.UpdateAsync(It.IsAny<Proposta>()), Times.Once);
        }

        [TestMethod]
        public async Task ReprovaPropostaPorId_RepositoryUpdateFails_ReturnsZero()
        {
            // Arrange
            var propostaId = "proposta123";
            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.EmAnalise,
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow.AddDays(-1)
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            _mockPropostaRepository.Setup(r => r.UpdateAsync(It.IsAny<Proposta>()))
                                  .ReturnsAsync(0); // Simulate update failure

            // Act
            var result = await _useCase.ReprovaPropostaPorId(propostaId);

            // Assert
            Assert.AreEqual(0, result);
            Assert.AreEqual(EStatusProposta.Rejeitada, proposta.statusProposta); // Status still changed locally
            _mockPropostaRepository.Verify(r => r.UpdateAsync(It.IsAny<Proposta>()), Times.Once);
        }

        [TestMethod]
        public async Task BuscaValidaProposta_ValidPropostaEmAnalise_ReturnsProposta()
        {
            // This is testing the private method indirectly through the public methods
            // Arrange
            var propostaId = "proposta123";
            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.EmAnalise,
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            _mockPropostaRepository.Setup(r => r.UpdateAsync(It.IsAny<Proposta>()))
                                  .ReturnsAsync(1);

            // Act - This will call BuscaValidaProposta internally
            var result = await _useCase.AprovaPropostaPorId(propostaId);

            // Assert
            Assert.AreEqual(1, result);
            _mockPropostaRepository.Verify(r => r.GetByIdAsync(propostaId), Times.Once);
        }
    }
}
