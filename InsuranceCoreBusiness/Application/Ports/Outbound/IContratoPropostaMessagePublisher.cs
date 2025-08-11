
using InsuranceCoreBusiness.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCoreBusiness.Application.Ports.Outbound
{
    public interface IContratoPropostaMessagePublisher
    {
        Task<EStatusProposta> getStatusPropostaFromService(string propostaId);
        Task<string> geraContratoFromService(string propostaId, string userId, DateOnly dataVigenciaInicio, DateOnly dataVigenciaFim);
    }
}
