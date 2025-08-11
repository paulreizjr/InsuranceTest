using InsuranceCoreBusiness.Application.Ports.Inbound;
using InsuranceCoreBusiness.Application.Ports.Outbound;
using InsuranceCoreBusiness.Application.UseCases;
using InsuranceCoreBusiness.Domain.Entities;
using InsuranceCoreBusiness.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceCoreBusinessTest.Application.UseCases
{
    [TestClass]
    public class CrudPropostaUCTest
    {
        private Mock<IPropostaRepository> _mockPropostaRepository;
        private Mock<IPropostaService> _mockPropostaService;
        private CrudPropostaUC _useCase;

        [TestInitialize]
        public void Setup()
        {
            _mockPropostaRepository = new Mock<IPropostaRepository>();
            _mockPropostaService = new Mock<IPropostaService>();
            _useCase = new CrudPropostaUC(_mockPropostaRepository.Object, _mockPropostaService.Object);
        }

        [TestMethod]
        public async Task GetPropostaByIdAsync_ValidId_ReturnsProposta()
        {
            // Arrange
            var propostaId = "proposta123";
            var expectedProposta = new Proposta
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
                                  .ReturnsAsync(expectedProposta);

            // Act
            var result = await _useCase.GetPropostaByIdAsync(propostaId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(propostaId, result.id);
            Assert.AreEqual("Toyota Corolla", result.automovel);
            Assert.AreEqual(EStatusProposta.EmAnalise, result.statusProposta);
            _mockPropostaRepository.Verify(r => r.GetByIdAsync(propostaId), Times.Once);
        }

        [TestMethod]
        public async Task GetPropostaByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var propostaId = "invalid123";
            _mockPropostaRepository.Setup(r => r.GetByIdAsync(propostaId))
                                  .ReturnsAsync((Proposta)null);

            // Act
            var result = await _useCase.GetPropostaByIdAsync(propostaId);

            // Assert
            Assert.IsNull(result);
            _mockPropostaRepository.Verify(r => r.GetByIdAsync(propostaId), Times.Once);
        }

        [TestMethod]
        public async Task GetAllPropostaAsync_ValidCall_ReturnsAllPropostas()
        {
            // Arrange
            var expectedPropostas = new List<Proposta>
            {
                new Proposta
                {
                    id = "proposta1",
                    automovel = "Toyota Corolla",
                    valorAutomovel = 50000m,
                    fatorPeso = 1.2m,
                    condutor = "João Silva",
                    statusProposta = EStatusProposta.EmAnalise,
                    valorProposta = 2500m,
                    dataAtualizacao = DateTime.UtcNow
                },
                new Proposta
                {
                    id = "proposta2",
                    automovel = "Honda Civic",
                    valorAutomovel = 60000m,
                    fatorPeso = 1.1m,
                    condutor = "Maria Santos",
                    statusProposta = EStatusProposta.Aprovada,
                    valorProposta = 3000m,
                    dataAtualizacao = DateTime.UtcNow
                }
            };

            _mockPropostaRepository.Setup(r => r.GetAllAsync())
                                  .ReturnsAsync(expectedPropostas);

            // Act
            var result = await _useCase.GetAllPropostaAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("proposta1", result.First().id);
            Assert.AreEqual("proposta2", result.Last().id);
            _mockPropostaRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetAllPropostaAsync_EmptyRepository_ReturnsEmptyList()
        {
            // Arrange
            var expectedPropostas = new List<Proposta>();
            _mockPropostaRepository.Setup(r => r.GetAllAsync())
                                  .ReturnsAsync(expectedPropostas);

            // Act
            var result = await _useCase.GetAllPropostaAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockPropostaRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task AddPropostaAsync_ValidProposta_CalculatesValueAndReturnsSuccess()
        {
            // Arrange
            var proposta = new Proposta
            {
                id = "proposta123",
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.EmAnalise,
                valorProposta = 0m, // Will be calculated
                dataAtualizacao = DateTime.UtcNow
            };

            var calculatedValue = 2500m;

            _mockPropostaService.Setup(s => s.CalculaValorTotalProposta(proposta))
                               .Returns(calculatedValue);

            _mockPropostaRepository.Setup(r => r.AddAsync(It.IsAny<Proposta>()))
                                  .ReturnsAsync(1);

            // Act
            var result = await _useCase.AddPropostaAsync(proposta);

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(calculatedValue, proposta.valorProposta);
            _mockPropostaService.Verify(s => s.CalculaValorTotalProposta(proposta), Times.Once);
            _mockPropostaRepository.Verify(r => r.AddAsync(proposta), Times.Once);
        }

        [TestMethod]
        public async Task UpdatePropostaAsync_ValidProposta_CalculatesValueAndReturnsSuccess()
        {
            // Arrange
            var proposta = new Proposta
            {
                id = "proposta123",
                automovel = "Toyota Corolla",
                valorAutomovel = 55000m,
                fatorPeso = 1.3m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.EmAnalise,
                valorProposta = 2500m, // Will be recalculated
                dataAtualizacao = DateTime.UtcNow
            };

            var calculatedValue = 3000m;

            _mockPropostaService.Setup(s => s.CalculaValorTotalProposta(proposta))
                               .Returns(calculatedValue);

            _mockPropostaRepository.Setup(r => r.UpdateAsync(It.IsAny<Proposta>()))
                                  .ReturnsAsync(1);

            // Act
            var result = await _useCase.UpdatePropostaAsync(proposta);

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(calculatedValue, proposta.valorProposta);
            _mockPropostaService.Verify(s => s.CalculaValorTotalProposta(proposta), Times.Once);
            _mockPropostaRepository.Verify(r => r.UpdateAsync(proposta), Times.Once);
        }

        [TestMethod]
        public async Task UpdatePropostaAsync_NonExistentProposta_ReturnsZero()
        {
            // Arrange
            var proposta = new Proposta
            {
                id = "nonexistent123",
                automovel = "Toyota Corolla",
                valorAutomovel = 50000m,
                fatorPeso = 1.2m,
                condutor = "João Silva",
                statusProposta = EStatusProposta.EmAnalise,
                valorProposta = 2500m,
                dataAtualizacao = DateTime.UtcNow
            };

            var calculatedValue = 2500m;

            _mockPropostaService.Setup(s => s.CalculaValorTotalProposta(proposta))
                               .Returns(calculatedValue);

            _mockPropostaRepository.Setup(r => r.UpdateAsync(It.IsAny<Proposta>()))
                                  .ReturnsAsync(0);

            // Act
            var result = await _useCase.UpdatePropostaAsync(proposta);

            // Assert
            Assert.AreEqual(0, result);
            Assert.AreEqual(calculatedValue, proposta.valorProposta);
            _mockPropostaService.Verify(s => s.CalculaValorTotalProposta(proposta), Times.Once);
            _mockPropostaRepository.Verify(r => r.UpdateAsync(proposta), Times.Once);
        }

        [TestMethod]
        public async Task DeletePropostaAsync_ValidId_ReturnsSuccess()
        {
            // Arrange
            var propostaId = "proposta123";
            _mockPropostaRepository.Setup(r => r.DeleteAsync(propostaId))
                                  .ReturnsAsync(1);

            // Act
            var result = await _useCase.DeletePropostaAsync(propostaId);

            // Assert
            Assert.AreEqual(1, result);
            _mockPropostaRepository.Verify(r => r.DeleteAsync(propostaId), Times.Once);
        }

        [TestMethod]
        public async Task DeletePropostaAsync_NonExistentId_ReturnsZero()
        {
            // Arrange
            var propostaId = "nonexistent123";
            _mockPropostaRepository.Setup(r => r.DeleteAsync(propostaId))
                                  .ReturnsAsync(0);

            // Act
            var result = await _useCase.DeletePropostaAsync(propostaId);

            // Assert
            Assert.AreEqual(0, result);
            _mockPropostaRepository.Verify(r => r.DeleteAsync(propostaId), Times.Once);
        }

        [TestMethod]
        public async Task GetPropostasByClienteIdAsync_ValidClienteId_ReturnsPropostas()
        {
            // Arrange
            var clienteId = "cliente123";
            var expectedPropostas = new List<Proposta>
            {
                new Proposta
                {
                    id = "proposta1",
                    automovel = "Toyota Corolla",
                    valorAutomovel = 50000m,
                    fatorPeso = 1.2m,
                    condutor = clienteId,
                    statusProposta = EStatusProposta.EmAnalise,
                    valorProposta = 2500m,
                    dataAtualizacao = DateTime.UtcNow
                },
                new Proposta
                {
                    id = "proposta2",
                    automovel = "Honda Civic",
                    valorAutomovel = 60000m,
                    fatorPeso = 1.1m,
                    condutor = clienteId,
                    statusProposta = EStatusProposta.Aprovada,
                    valorProposta = 3000m,
                    dataAtualizacao = DateTime.UtcNow
                }
            };

            _mockPropostaRepository.Setup(r => r.GetByClienteIdAsync(clienteId))
                                  .ReturnsAsync(expectedPropostas);

            // Act
            var result = await _useCase.GetPropostasByClienteIdAsync(clienteId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(p => p.condutor == clienteId));
            _mockPropostaRepository.Verify(r => r.GetByClienteIdAsync(clienteId), Times.Once);
        }

        [TestMethod]
        public async Task GetPropostasByClienteIdAsync_InvalidClienteId_ReturnsEmptyList()
        {
            // Arrange
            var clienteId = "invalid123";
            var expectedPropostas = new List<Proposta>();

            _mockPropostaRepository.Setup(r => r.GetByClienteIdAsync(clienteId))
                                  .ReturnsAsync(expectedPropostas);

            // Act
            var result = await _useCase.GetPropostasByClienteIdAsync(clienteId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockPropostaRepository.Verify(r => r.GetByClienteIdAsync(clienteId), Times.Once);
        }

        [TestMethod]
        public async Task GetPropostasByStatusAsync_ValidStatusId_ReturnsPropostas()
        {
            // Arrange
            var statusId = "1"; // Aprovada
            var expectedPropostas = new List<Proposta>
            {
                new Proposta
                {
                    id = "proposta1",
                    automovel = "Toyota Corolla",
                    valorAutomovel = 50000m,
                    fatorPeso = 1.2m,
                    condutor = "cliente1",
                    statusProposta = EStatusProposta.Aprovada,
                    valorProposta = 2500m,
                    dataAtualizacao = DateTime.UtcNow
                },
                new Proposta
                {
                    id = "proposta2",
                    automovel = "Honda Civic",
                    valorAutomovel = 60000m,
                    fatorPeso = 1.1m,
                    condutor = "cliente2",
                    statusProposta = EStatusProposta.Aprovada,
                    valorProposta = 3000m,
                    dataAtualizacao = DateTime.UtcNow
                }
            };

            _mockPropostaRepository.Setup(r => r.GetByStatusAsync(statusId))
                                  .ReturnsAsync(expectedPropostas);

            // Act
            var result = await _useCase.GetPropostasByStatusAsync(statusId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(p => p.statusProposta == EStatusProposta.Aprovada));
            _mockPropostaRepository.Verify(r => r.GetByStatusAsync(statusId), Times.Once);
        }

        [TestMethod]
        public async Task GetPropostasByStatusAsync_InvalidStatusId_ReturnsEmptyList()
        {
            // Arrange
            var statusId = "999"; // Invalid status
            var expectedPropostas = new List<Proposta>();

            _mockPropostaRepository.Setup(r => r.GetByStatusAsync(statusId))
                                  .ReturnsAsync(expectedPropostas);

            // Act
            var result = await _useCase.GetPropostasByStatusAsync(statusId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockPropostaRepository.Verify(r => r.GetByStatusAsync(statusId), Times.Once);
        }
    }
}
