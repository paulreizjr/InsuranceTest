using InsuranceCoreBusiness.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceContratacaoService.DTOs
{
    public class GeraContratoResponseDto
    {
        public string PropostaId { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public ContratoProposta? Contrato { get; set; }
        public string? ContratoId { get; set; }
    }
}
