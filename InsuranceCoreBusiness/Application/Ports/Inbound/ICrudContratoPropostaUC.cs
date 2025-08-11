using InsuranceCoreBusiness.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCoreBusiness.Application.Ports.Inbound
{
    public interface ICrudContratoPropostaUC
    {
        Task<ContratoProposta> GetContratoPropostaByIdAsync(string id);
        Task<IEnumerable<ContratoProposta>> GetAllContratoPropostaAsync();
        Task<int> AddContratoPropostaAsync(ContratoProposta contrato);
        Task<int> UpdateContratoPropostaAsync(ContratoProposta contrato);
        Task<int> DeleteContratoPropostaAsync(string id);
    }
}
