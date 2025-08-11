using InsuranceCoreBusiness.Application.Ports.Inbound;
using InsuranceCoreBusiness.Application.Ports.Outbound;
using InsuranceCoreBusiness.Domain.Entities;
using InsuranceCoreBusiness.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCoreBusiness.Application.UseCases
{
    public class OperacoesPropostaUC : IOperacoesPropostaUC
    {
        private readonly IPropostaRepository _propostaRepository;

        public OperacoesPropostaUC(IPropostaRepository propostaRepository)
        {
            _propostaRepository = propostaRepository;
        }

        public async Task<int> AprovaPropostaPorId(string propostaId)
        {
            Proposta proposta = await BuscaValidaProposta(propostaId);
            proposta.statusProposta = Domain.Enums.EStatusProposta.Aprovada;
            proposta.dataAtualizacao = DateTime.UtcNow;
            return await _propostaRepository.UpdateAsync(proposta);
        }

        public async Task<int> ReprovaPropostaPorId(string propostaId)
        {
            Proposta proposta = await BuscaValidaProposta(propostaId);
            proposta.statusProposta = Domain.Enums.EStatusProposta.Rejeitada;
            proposta.dataAtualizacao = DateTime.UtcNow;
            return await _propostaRepository.UpdateAsync(proposta);
        }

        private async Task<Proposta> BuscaValidaProposta(string propostaId)
        {
            var proposta = await _propostaRepository.GetByIdAsync(propostaId);
            if (proposta == null)
            {
                throw new ArgumentException("Proposta não encontrada.");
            }

            if (proposta.statusProposta != Domain.Enums.EStatusProposta.EmAnalise)
            {
                throw new StatusPropostaInvalidoException("Proposta não está em análise");
            }

            return proposta;
        }
    }
}
