using InsuranceCoreBusiness.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceContratacaoService.DTOs
{
    public class PropostaStatusResponseDto
    {
        public string PropostaId { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;
        public EStatusProposta StatusProposta { get; set; }
        public bool PropostaFound { get; set; }
    }
}
