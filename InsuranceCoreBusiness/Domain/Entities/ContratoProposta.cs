using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCoreBusiness.Domain.Entities
{
    public class ContratoProposta
    {
        public string? id { get; set; }
        public Proposta proposta { get; set; } = new Proposta();
        public DateOnly dataVigenciaInicio { get; set; }
        public DateOnly dataVigenciaFim { get; set; }
        public DateTime dataAtualizacao { get; set; }
    }
}
