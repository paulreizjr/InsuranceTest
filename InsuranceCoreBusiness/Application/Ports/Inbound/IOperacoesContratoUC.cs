using InsuranceCoreBusiness.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCoreBusiness.Application.Ports.Inbound
{
    public interface IOperacoesContratoUC
    {
        Task<ContratoProposta> GeraContratoProposta(string propostaId, string userId, DateOnly dataVigenciaInicio, DateOnly dataVigenciaFim);
    }
}
