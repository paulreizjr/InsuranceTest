using InsuranceCoreBusiness.Application.Ports.Outbound;
using InsuranceCoreBusiness.Application.UseCases;
using InsuranceCoreBusiness.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceCoreBusinessTest.Application.UseCases
{
    [TestClass]
    public class CrudContratoPropostaUCTest
    {
        private Mock<IContratoPropostaRepository> _mockRepository;
        private CrudContratoPropostaUC _useCase;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IContratoPropostaRepository>();
            _useCase = new CrudContratoPropostaUC(_mockRepository.Object);
        }

        [TestMethod]
        public async Task GetContratoPropostaByIdAsync_ValidId_ReturnsContratoProposta()
        {
            // Arrange
            var contratoId = "contrato123";
            var expectedContrato = new ContratoProposta
            {
                id = contratoId,
                proposta = new Proposta { id = "proposta123" },
                dataVigenciaInicio = DateOnly.FromDateTime(DateTime.Now),
                dataVigenciaFim = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
                dataAtualizacao = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.GetByIdAsync(contratoId))
                          .ReturnsAsync(expectedContrato);

            // Act
            var result = await _useCase.GetContratoPropostaByIdAsync(contratoId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(contratoId, result.id);
            Assert.AreEqual(expectedContrato.proposta.id, result.proposta.id);
            _mockRepository.Verify(r => r.GetByIdAsync(contratoId), Times.Once);
        }

        [TestMethod]
        public async Task GetContratoPropostaByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var contratoId = "invalid123";
            _mockRepository.Setup(r => r.GetByIdAsync(contratoId))
                          .ReturnsAsync((ContratoProposta)null);

            // Act
            var result = await _useCase.GetContratoPropostaByIdAsync(contratoId);

            // Assert
            Assert.IsNull(result);
            _mockRepository.Verify(r => r.GetByIdAsync(contratoId), Times.Once);
        }

        [TestMethod]
        public async Task GetAllContratoPropostaAsync_ValidCall_ReturnsAllContratos()
        {
            // Arrange
            var expectedContratos = new List<ContratoProposta>
            {
                new ContratoProposta
                {
                    id = "contrato1",
                    proposta = new Proposta { id = "proposta1" },
                    dataVigenciaInicio = DateOnly.FromDateTime(DateTime.Now),
                    dataVigenciaFim = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
                    dataAtualizacao = DateTime.UtcNow
                },
                new ContratoProposta
                {
                    id = "contrato2",
                    proposta = new Proposta { id = "proposta2" },
                    dataVigenciaInicio = DateOnly.FromDateTime(DateTime.Now),
                    dataVigenciaFim = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
                    dataAtualizacao = DateTime.UtcNow
                }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(expectedContratos);

            // Act
            var result = await _useCase.GetAllContratoPropostaAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("contrato1", result.First().id);
            Assert.AreEqual("contrato2", result.Last().id);
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetAllContratoPropostaAsync_EmptyRepository_ReturnsEmptyList()
        {
            // Arrange
            var expectedContratos = new List<ContratoProposta>();
            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(expectedContratos);

            // Act
            var result = await _useCase.GetAllContratoPropostaAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task AddContratoPropostaAsync_ValidContrato_ReturnsSuccess()
        {
            // Arrange
            var contrato = new ContratoProposta
            {
                id = "contrato123",
                proposta = new Proposta { id = "proposta123" },
                dataVigenciaInicio = DateOnly.FromDateTime(DateTime.Now),
                dataVigenciaFim = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
                dataAtualizacao = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.AddAsync(contrato))
                          .ReturnsAsync(1);

            // Act
            var result = await _useCase.AddContratoPropostaAsync(contrato);

            // Assert
            Assert.AreEqual(1, result);
            _mockRepository.Verify(r => r.AddAsync(contrato), Times.Once);
        }

        [TestMethod]
        public async Task AddContratoPropostaAsync_RepositoryFailure_ReturnsZero()
        {
            // Arrange
            var contrato = new ContratoProposta
            {
                id = "contrato123",
                proposta = new Proposta { id = "proposta123" },
                dataVigenciaInicio = DateOnly.FromDateTime(DateTime.Now),
                dataVigenciaFim = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
                dataAtualizacao = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.AddAsync(contrato))
                          .ReturnsAsync(0);

            // Act
            var result = await _useCase.AddContratoPropostaAsync(contrato);

            // Assert
            Assert.AreEqual(0, result);
            _mockRepository.Verify(r => r.AddAsync(contrato), Times.Once);
        }

        [TestMethod]
        public async Task UpdateContratoPropostaAsync_ValidContrato_ReturnsSuccess()
        {
            // Arrange
            var contrato = new ContratoProposta
            {
                id = "contrato123",
                proposta = new Proposta { id = "proposta123" },
                dataVigenciaInicio = DateOnly.FromDateTime(DateTime.Now),
                dataVigenciaFim = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
                dataAtualizacao = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.UpdateAsync(contrato))
                          .ReturnsAsync(1);

            // Act
            var result = await _useCase.UpdateContratoPropostaAsync(contrato);

            // Assert
            Assert.AreEqual(1, result);
            _mockRepository.Verify(r => r.UpdateAsync(contrato), Times.Once);
        }

        [TestMethod]
        public async Task UpdateContratoPropostaAsync_NonExistentContrato_ReturnsZero()
        {
            // Arrange
            var contrato = new ContratoProposta
            {
                id = "nonexistent123",
                proposta = new Proposta { id = "proposta123" },
                dataVigenciaInicio = DateOnly.FromDateTime(DateTime.Now),
                dataVigenciaFim = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
                dataAtualizacao = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.UpdateAsync(contrato))
                          .ReturnsAsync(0);

            // Act
            var result = await _useCase.UpdateContratoPropostaAsync(contrato);

            // Assert
            Assert.AreEqual(0, result);
            _mockRepository.Verify(r => r.UpdateAsync(contrato), Times.Once);
        }

        [TestMethod]
        public async Task DeleteContratoPropostaAsync_ValidId_ReturnsSuccess()
        {
            // Arrange
            var contratoId = "contrato123";
            _mockRepository.Setup(r => r.DeleteAsync(contratoId))
                          .ReturnsAsync(1);

            // Act
            var result = await _useCase.DeleteContratoPropostaAsync(contratoId);

            // Assert
            Assert.AreEqual(1, result);
            _mockRepository.Verify(r => r.DeleteAsync(contratoId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteContratoPropostaAsync_NonExistentId_ReturnsZero()
        {
            // Arrange
            var contratoId = "nonexistent123";
            _mockRepository.Setup(r => r.DeleteAsync(contratoId))
                          .ReturnsAsync(0);

            // Act
            var result = await _useCase.DeleteContratoPropostaAsync(contratoId);

            // Assert
            Assert.AreEqual(0, result);
            _mockRepository.Verify(r => r.DeleteAsync(contratoId), Times.Once);
        }
    }
}
