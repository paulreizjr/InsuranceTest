using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurancePropostaService.DTOs
{
    public class GeraContratoRequestDto
    {
        public string PropostaId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateOnly DataVigenciaInicio { get; set; }
        public DateOnly DataVigenciaFim { get; set; }
        public string RequestId { get; set; } = string.Empty;
    }
}
