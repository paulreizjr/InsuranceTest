using InsuranceCoreBusiness.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCoreBusiness.Application.Ports.Inbound
{
    public interface IPropostaService
    {
        decimal CalculaValorTotalProposta(Proposta proposta);
    }
}
