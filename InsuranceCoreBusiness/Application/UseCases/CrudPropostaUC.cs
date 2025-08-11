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
    public class CrudPropostaUC : ICrudPropostaUC
    {
        private readonly IPropostaService _propostaService;
        private readonly IPropostaRepository _propostaRepository;
        public CrudPropostaUC(IPropostaRepository propostaRepository, IPropostaService propostaService)
        {
            _propostaRepository = propostaRepository;
            _propostaService = propostaService;
        }
        public async Task<Proposta> GetPropostaByIdAsync(string id)
        {
            return await _propostaRepository.GetByIdAsync(id);
        }
        public async Task<IEnumerable<Proposta>> GetAllPropostaAsync()
        {
            return await _propostaRepository.GetAllAsync();
        }
        public async Task<int> AddPropostaAsync(Proposta proposta)
        {
            proposta.valorProposta = _propostaService.CalculaValorTotalProposta(proposta);
            return await _propostaRepository.AddAsync(proposta);
        }
        public async Task<int> UpdatePropostaAsync(Proposta proposta)
        {
            proposta.valorProposta = _propostaService.CalculaValorTotalProposta(proposta);
            return await _propostaRepository.UpdateAsync(proposta);
        }
        public async Task<int> DeletePropostaAsync(string id)
        {
            return await _propostaRepository.DeleteAsync(id);
        }
        public async Task<IEnumerable<Proposta>> GetPropostasByClienteIdAsync(string clienteId)
        {
            return await _propostaRepository.GetByClienteIdAsync(clienteId);
        }
        public async Task<IEnumerable<Proposta>> GetPropostasByStatusAsync(string statusId)
        {
            return await _propostaRepository.GetByStatusAsync(statusId);
        }
    }
}
