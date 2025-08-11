using InsuranceCoreBusiness.Application.Ports.Inbound;
using InsuranceCoreBusiness.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCoreBusiness.Application.Services
{
    public class PropostaService : IPropostaService
    {
        public decimal CalculaValorTotalProposta(Proposta proposta)
        {
            decimal valorProposta = proposta.valorAutomovel * proposta.fatorPeso / 100 * proposta.fatorPeso / 100;
            return Math.Round(valorProposta, 2);
        }
    }
}
