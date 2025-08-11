using InsuranceCoreBusiness.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCoreBusiness.Application.Ports.Outbound
{
    public interface IContratoPropostaRepository
    {
        Task<ContratoProposta> GetByIdAsync(string id);
        Task<IEnumerable<ContratoProposta>> GetAllAsync();
        Task<int> AddAsync(ContratoProposta contrato);
        Task<int> UpdateAsync(ContratoProposta contrato);
        Task<int> UpdatePropostaAsync(Proposta contrato);
        Task<int> DeleteAsync(string id);
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}
