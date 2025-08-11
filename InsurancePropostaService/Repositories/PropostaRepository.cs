using InsuranceCoreBusiness.Application.Ports.Outbound;
using InsuranceCoreBusiness.Domain.Entities;
using InsuranceCoreBusiness.Domain.Enums;
using InsurancePropostaService.Data;
using InsurancePropostaService.Models;
using Microsoft.EntityFrameworkCore;

namespace InsurancePropostaService.Repositories
{
    public class PropostaRepository : IPropostaRepository
    {
        private readonly InsuranceDbContext _context;

        public PropostaRepository(InsuranceDbContext context)
        {
            _context = context;
        }

        public async Task<Proposta> GetByIdAsync(string id)
        {
            var propostaModel = await _context.Propostas.FindAsync(id);
            if (propostaModel == null)
                return null;

            return MapToDomainEntity(propostaModel);
        }

        public async Task<IEnumerable<Proposta>> GetAllAsync()
        {
            var propostasModel = await _context.Propostas.ToListAsync();
            return propostasModel.Select(MapToDomainEntity);
        }

        public async Task<int> AddAsync(Proposta proposta)
        {
            var propostaModel = MapToModel(proposta);
            _context.Propostas.Add(propostaModel);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Proposta proposta)
        {
            var existingProposta = await _context.Propostas.FindAsync(proposta.id);
            if (existingProposta == null)
                return 0;

            existingProposta.automovel = proposta.automovel;
            existingProposta.valorAutomovel = proposta.valorAutomovel;
            existingProposta.fatorPeso = proposta.fatorPeso;
            existingProposta.condutor = proposta.condutor;
            existingProposta.statusProposta = proposta.statusProposta;
            existingProposta.valorProposta = proposta.valorProposta;
            existingProposta.dataAtualizacao = proposta.dataAtualizacao;

            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(string id)
        {
            var proposta = await _context.Propostas.FindAsync(id);
            if (proposta == null)
                return 0;

            _context.Propostas.Remove(proposta);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Proposta>> GetByClienteIdAsync(string clienteId)
        {
            var propostasModel = await _context.Propostas
                .Where(p => p.condutor == clienteId)
                .ToListAsync();
            
            return propostasModel.Select(MapToDomainEntity);
        }

        public async Task<IEnumerable<Proposta>> GetByStatusAsync(string statusId)
        {
            if (Enum.TryParse<EStatusProposta>(statusId, out var status))
            {
                var propostasModel = await _context.Propostas
                    .Where(p => p.statusProposta == status)
                    .ToListAsync();
                
                return propostasModel.Select(MapToDomainEntity);
            }
            
            return new List<Proposta>();
        }

        private Proposta MapToDomainEntity(PropostaEFModel model)
        {
            return new Proposta
            {
                id = model.id,
                automovel = model.automovel,
                valorAutomovel = model.valorAutomovel,
                fatorPeso = model.fatorPeso,
                condutor = model.condutor,
                statusProposta = model.statusProposta,
                valorProposta = model.valorProposta,
                dataAtualizacao = model.dataAtualizacao
            };
        }

        private PropostaEFModel MapToModel(Proposta entity)
        {
            return new PropostaEFModel
            {
                id = entity.id,
                automovel = entity.automovel,
                valorAutomovel = entity.valorAutomovel,
                fatorPeso = entity.fatorPeso,
                condutor = entity.condutor,
                statusProposta = entity.statusProposta,
                valorProposta = entity.valorProposta,
                dataAtualizacao = entity.dataAtualizacao
            };
        }
    }
}
