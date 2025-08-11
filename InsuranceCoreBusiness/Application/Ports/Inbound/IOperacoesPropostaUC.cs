using InsuranceCoreBusiness.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCoreBusiness.Application.Ports.Inbound
{
    public interface IOperacoesPropostaUC
    {
        Task<int> AprovaPropostaPorId(string propostaId);
        Task<int> ReprovaPropostaPorId(string propostaId);

    }
}
