using InsuranceCoreBusiness.Application.Ports.Outbound;
using InsuranceCoreBusiness.Domain.Entities;
using InsurancePropostaService.Data;
using InsurancePropostaService.Models;
using Microsoft.EntityFrameworkCore;

namespace InsurancePropostaService.Repositories
{
    public class ContratoPropostaRepository : IContratoPropostaRepository
    {
        private readonly InsuranceDbContext _context;
        private readonly IPropostaRepository _propostaRepository;
        private Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? _currentTransaction;

        public ContratoPropostaRepository(InsuranceDbContext context, IPropostaRepository propostaRepository)
        {
            _context = context;
            _propostaRepository = propostaRepository;
        }

        public async Task<ContratoProposta> GetByIdAsync(string id)
        {
            var contratoModel = await _context.ContratoPropostas
                .Include(c => c.proposta)
                .FirstOrDefaultAsync(c => c.id == id);
            
            if (contratoModel == null)
                return null;

            return MapToDomainEntity(contratoModel);
        }

        public async Task<IEnumerable<ContratoProposta>> GetAllAsync()
        {
            var contratosModel = await _context.ContratoPropostas
                .Include(c => c.proposta)
                .ToListAsync();
            
            return contratosModel.Select(MapToDomainEntity);
        }

        public async Task<int> AddAsync(ContratoProposta contrato)
        {
            var contratoModel = MapToModel(contrato);
            contratoModel.id = Guid.NewGuid().ToString();
            _context.ContratoPropostas.Add(contratoModel);
            await _context.SaveChangesAsync();
            return 1; // Return success indicator
        }

        public async Task<int> UpdateAsync(ContratoProposta contrato)
        {
            var existingContrato = await _context.ContratoPropostas.FindAsync(contrato.id);
            if (existingContrato == null)
                return 0;

            existingContrato.idProposta = contrato.proposta?.id;
            existingContrato.dataVigenciaInicio = contrato.dataVigenciaInicio;
            existingContrato.dataVigenciaFim = contrato.dataVigenciaFim;
            existingContrato.dataAtualizacao = contrato.dataAtualizacao;

            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(string id)
        {
            var contrato = await _context.ContratoPropostas.FindAsync(id);
            if (contrato == null)
                return 0;

            _context.ContratoPropostas.Remove(contrato);
            return await _context.SaveChangesAsync();
        }

        private ContratoProposta MapToDomainEntity(ContratoPropostaEFModel model)
        {
            var domainEntity = new ContratoProposta
            {
                id = model.id,
                dataVigenciaInicio = model.dataVigenciaInicio,
                dataVigenciaFim = model.dataVigenciaFim,
                dataAtualizacao = model.dataAtualizacao
            };

            if (model.proposta != null)
            {
                domainEntity.proposta = new Proposta
                {
                    id = model.proposta.id,
                    automovel = model.proposta.automovel,
                    valorAutomovel = model.proposta.valorAutomovel,
                    fatorPeso = model.proposta.fatorPeso,
                    condutor = model.proposta.condutor,
                    statusProposta = model.proposta.statusProposta,
                    valorProposta = model.proposta.valorProposta,
                    dataAtualizacao = model.proposta.dataAtualizacao
                };
            }

            return domainEntity;
        }

        private ContratoPropostaEFModel MapToModel(ContratoProposta entity)
        {
            var model = new ContratoPropostaEFModel
            {
                id = entity.id,
                proposta = null!,
                idProposta = entity.proposta?.id,
                dataVigenciaInicio = entity.dataVigenciaInicio,
                dataVigenciaFim = entity.dataVigenciaFim,
                dataAtualizacao = entity.dataAtualizacao
            };
            
            return model;
        }

        public async Task<int> UpdatePropostaAsync(Proposta proposta)
        {
            return await _propostaRepository.UpdateAsync(proposta);
        }

        public void BeginTransaction()
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }
            
            _currentTransaction = _context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No transaction in progress.");
            }

            try
            {
                _currentTransaction.Commit();
            }
            finally
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }

        public void RollbackTransaction()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No transaction in progress.");
            }

            try
            {
                _currentTransaction.Rollback();
            }
            finally
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
