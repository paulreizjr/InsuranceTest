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
    public class OperacoesContratoUC : IOperacoesContratoUC
    {
        private readonly IContratoPropostaRepository _contratoRepository;
        private readonly IPropostaRepository _propostaRepository;

        public OperacoesContratoUC(
            IContratoPropostaRepository contratoRepository, 
            IPropostaRepository propostaRepository)
        {
            _contratoRepository = contratoRepository;
            _propostaRepository = propostaRepository;
        }

        public async Task<ContratoProposta> GeraContratoProposta(string propostaId, string userId, DateOnly dataVigenciaInicio, DateOnly dataVigenciaFim)
        {
            var proposta = await _propostaRepository.GetByIdAsync(propostaId);
            if (proposta == null)
            {
                throw new ArgumentException("Proposta não encontrada.");
            }

            if (proposta.statusProposta != Domain.Enums.EStatusProposta.Aprovada)
            {
                throw new StatusPropostaInvalidoException("Proposta não está aprovada.");
            }

            if(dataVigenciaInicio >= dataVigenciaFim)
            {
                throw new ArgumentException("Data de início da vigência deve ser anterior à data de fim.");
            }

            var novoContrato = new ContratoProposta
            {
                id = Guid.NewGuid().ToString(),
                proposta = new Proposta { id = proposta.id },
                dataVigenciaInicio = dataVigenciaInicio,
                dataVigenciaFim = dataVigenciaFim,
                dataAtualizacao = DateTime.UtcNow
            };

            try
            {
                _contratoRepository.BeginTransaction();
                int retorno = await _contratoRepository.AddAsync(novoContrato);

                if (retorno <= 0)
                {
                    throw new Exception("Erro ao gerar contrato proposta.");
                }

                proposta.statusProposta = Domain.Enums.EStatusProposta.Contratada;
                proposta.dataAtualizacao = DateTime.UtcNow;
                await _propostaRepository.UpdateAsync(proposta);
                _contratoRepository.CommitTransaction();
            }
            catch (Exception)
            {
                _contratoRepository.RollbackTransaction();
                throw;
            }

            return novoContrato;

        }
    }
}
