using InsuranceCoreBusiness.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCoreBusiness.Application.Ports.Inbound
{
    public interface ICrudPropostaUC
    {
        Task<Proposta> GetPropostaByIdAsync(string id);
        Task<IEnumerable<Proposta>> GetAllPropostaAsync();
        Task<int> AddPropostaAsync(Proposta proposta);
        Task<int> UpdatePropostaAsync(Proposta proposta);
        Task<int> DeletePropostaAsync(string id);
        Task<IEnumerable<Proposta>> GetPropostasByClienteIdAsync(string clienteId);
        Task<IEnumerable<Proposta>> GetPropostasByStatusAsync(string statusId);
    }
}
