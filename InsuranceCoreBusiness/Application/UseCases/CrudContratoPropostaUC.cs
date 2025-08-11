using InsuranceCoreBusiness.Application.Ports.Inbound;
using InsuranceCoreBusiness.Application.Ports.Outbound;
using InsuranceCoreBusiness.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCoreBusiness.Application.UseCases
{
    public class CrudContratoPropostaUC : ICrudContratoPropostaUC
    {
        private readonly IContratoPropostaRepository _repository;
        public CrudContratoPropostaUC(IContratoPropostaRepository repository)
        {
            _repository = repository;
        }
        public async Task<ContratoProposta> GetContratoPropostaByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task<IEnumerable<ContratoProposta>> GetAllContratoPropostaAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<int> AddContratoPropostaAsync(ContratoProposta contrato)
        {
            return await _repository.AddAsync(contrato);
        }
        public async Task<int> UpdateContratoPropostaAsync(ContratoProposta contrato)
        {
            return await _repository.UpdateAsync(contrato);
        }
        public async Task<int> DeleteContratoPropostaAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
