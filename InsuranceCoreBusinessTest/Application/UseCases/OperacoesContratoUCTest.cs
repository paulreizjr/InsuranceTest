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
    public class OperacoesContratoUCTest
    {
        private Mock<IContratoPropostaRepository> _mockContratoRepository;
        private Mock<IPropostaRepository> _mockPropostaRepository;
        private OperacoesContratoUC _useCase;

        [TestInitialize]
        public void Setup()
        {
            _mockContratoRepository = new Mock<IContratoPropostaRepository>();
            _mockPropostaRepository = new Mock<IPropostaRepository>();
            _useCase = new OperacoesContratoUC(_mockContratoRepository.Object, _mockPropostaRepository.Object);
        }

        [TestMethod]
        public async Task GeraContratoProposta_ValidData_ReturnsNewContrato()
        {
            // Arrange
            var propostaId = "proposta123";
            var userId = "user123";
            var dataInicio = DateOnly.FromDateTime(DateTime.Now);
            var dataFim = DateOnly.FromDateTime(DateTime.Now.AddYears(1));

            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.Aprovada,
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            _mockContratoRepository.Setup(r => r.AddAsync(It.IsAny<ContratoProposta>()))
                                  .ReturnsAsync(1);

            _mockPropostaRepository.Setup(r => r.UpdateAsync(It.IsAny<Proposta>()))
                                  .ReturnsAsync(1);

            // Act
            var result = await _useCase.GeraContratoProposta(propostaId, userId, dataInicio, dataFim);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.id);
            Assert.AreEqual(propostaId, result.proposta.id);
            Assert.AreEqual(dataInicio, result.dataVigenciaInicio);
            Assert.AreEqual(dataFim, result.dataVigenciaFim);

            _mockPropostaRepository.Verify(r => r.GetByIdAsync(propostaId), Times.Once);
            _mockContratoRepository.Verify(r => r.BeginTransaction(), Times.Once);
            _mockContratoRepository.Verify(r => r.AddAsync(It.IsAny<ContratoProposta>()), Times.Once);
            _mockPropostaRepository.Verify(r => r.UpdateAsync(It.Is<Proposta>(p => p.statusProposta == EStatusProposta.Contratada)), Times.Once);
            _mockContratoRepository.Verify(r => r.CommitTransaction(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GeraContratoProposta_PropostaNotFound_ThrowsArgumentException()
        {
            // Arrange
            var propostaId = "nonexistent123";
            var userId = "user123";
            var dataInicio = DateOnly.FromDateTime(DateTime.Now);
            var dataFim = DateOnly.FromDateTime(DateTime.Now.AddYears(1));

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync((Proposta)null);

            // Act
            await _useCase.GeraContratoProposta(propostaId, userId, dataInicio, dataFim);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(StatusPropostaInvalidoException))]
        public async Task GeraContratoProposta_PropostaNotApproved_ThrowsStatusPropostaInvalidoException()
        {
            // Arrange
            var propostaId = "proposta123";
            var userId = "user123";
            var dataInicio = DateOnly.FromDateTime(DateTime.Now);
            var dataFim = DateOnly.FromDateTime(DateTime.Now.AddYears(1));

            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.EmAnalise, // Not approved
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            // Act
            await _useCase.GeraContratoProposta(propostaId, userId, dataInicio, dataFim);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GeraContratoProposta_InvalidDateRange_ThrowsArgumentException()
        {
            // Arrange
            var propostaId = "proposta123";
            var userId = "user123";
            var dataInicio = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            var dataFim = DateOnly.FromDateTime(DateTime.Now); // End before start

            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.Aprovada,
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            // Act
            await _useCase.GeraContratoProposta(propostaId, userId, dataInicio, dataFim);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GeraContratoProposta_SameDateRange_ThrowsArgumentException()
        {
            // Arrange
            var propostaId = "proposta123";
            var userId = "user123";
            var dataInicio = DateOnly.FromDateTime(DateTime.Now);
            var dataFim = dataInicio; // Same date

            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.Aprovada,
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            // Act
            await _useCase.GeraContratoProposta(propostaId, userId, dataInicio, dataFim);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task GeraContratoProposta_RepositoryAddFails_ThrowsExceptionAndRollsBack()
        {
            // Arrange
            var propostaId = "proposta123";
            var userId = "user123";
            var dataInicio = DateOnly.FromDateTime(DateTime.Now);
            var dataFim = DateOnly.FromDateTime(DateTime.Now.AddYears(1));

            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.Aprovada,
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            _mockContratoRepository.Setup(r => r.AddAsync(It.IsAny<ContratoProposta>()))
                                  .ReturnsAsync(0); // Simulate failure

            // Act
            try
            {
                await _useCase.GeraContratoProposta(propostaId, userId, dataInicio, dataFim);
            }
            finally
            {
                // Assert
                _mockContratoRepository.Verify(r => r.BeginTransaction(), Times.Once);
                _mockContratoRepository.Verify(r => r.RollbackTransaction(), Times.Once);
                _mockContratoRepository.Verify(r => r.CommitTransaction(), Times.Never);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task GeraContratoProposta_PropostaUpdateFails_ThrowsExceptionAndRollsBack()
        {
            // Arrange
            var propostaId = "proposta123";
            var userId = "user123";
            var dataInicio = DateOnly.FromDateTime(DateTime.Now);
            var dataFim = DateOnly.FromDateTime(DateTime.Now.AddYears(1));

            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.Aprovada,
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            _mockContratoRepository.Setup(r => r.AddAsync(It.IsAny<ContratoProposta>()))
                                  .ReturnsAsync(1);

            _mockPropostaRepository.Setup(r => r.UpdateAsync(It.IsAny<Proposta>()))
                                  .ThrowsAsync(new Exception("Database error"));

            // Act
            try
            {
                await _useCase.GeraContratoProposta(propostaId, userId, dataInicio, dataFim);
            }
            finally
            {
                // Assert
                _mockContratoRepository.Verify(r => r.BeginTransaction(), Times.Once);
                _mockContratoRepository.Verify(r => r.RollbackTransaction(), Times.Once);
                _mockContratoRepository.Verify(r => r.CommitTransaction(), Times.Never);
            }
        }

        [TestMethod]
        public async Task GeraContratoProposta_ValidScenarioWithRejectedStatus_ThrowsStatusPropostaInvalidoException()
        {
            // Arrange
            var propostaId = "proposta123";
            var userId = "user123";
            var dataInicio = DateOnly.FromDateTime(DateTime.Now);
            var dataFim = DateOnly.FromDateTime(DateTime.Now.AddYears(1));

            var proposta = new Proposta
            {
                id = propostaId,
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.Rejeitada, // Rejected status
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync(proposta);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<StatusPropostaInvalidoException>(
                () => _useCase.GeraContratoProposta(propostaId, userId, dataInicio, dataFim)
            );

            _mockPropostaRepository.Verify(r => r.GetByIdAsync(propostaId), Times.Once);
            _mockContratoRepository.Verify(r => r.BeginTransaction(), Times.Never);
        }

        [TestMethod]
        public async Task GeraContratoProposta_ValidScenarioWithContractedStatus_ThrowsStatusPropostaInvalidoException()
        {
            // Arrange
            var propostaId = "proposta123";
            var userId = "user123";
            var dataInicio = DateOnly.FromDateTime(DateTime.Now);
            var dataFim = DateOnly.FromDateTime(DateTime.Now.AddYears(1));

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

            // Act & Assert
            await Assert.ThrowsExceptionAsync<StatusPropostaInvalidoException>(
                () => _useCase.GeraContratoProposta(propostaId, userId, dataInicio, dataFim)
            );

            _mockPropostaRepository.Verify(r => r.GetByIdAsync(propostaId), Times.Once);
            _mockContratoRepository.Verify(r => r.BeginTransaction(), Times.Never);
        }
    }
}
