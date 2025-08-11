using InsuranceCoreBusiness.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCoreBusiness.Application.Ports.Outbound
{
    public interface IPropostaRepository
    {
        Task<Proposta> GetByIdAsync(string id);
        Task<IEnumerable<Proposta>> GetAllAsync();
        Task<int> AddAsync(Proposta proposta);
        Task<int> UpdateAsync(Proposta proposta);
        Task<int> DeleteAsync(string id);
        Task<IEnumerable<Proposta>> GetByClienteIdAsync(string clienteId);
        Task<IEnumerable<Proposta>> GetByStatusAsync(string statusId);
    }
}
