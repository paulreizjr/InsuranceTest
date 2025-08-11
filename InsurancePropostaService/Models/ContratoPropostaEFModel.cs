using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurancePropostaService.Models
{
    [Table("ContratoProposta")]
    public class ContratoPropostaEFModel
    {
        [Key]
        public string? id { get; set; }

        public string? idProposta { get; set; }

        [ForeignKey("idProposta")]
        public PropostaEFModel proposta { get; set; } = new PropostaEFModel();
        
        public DateOnly dataVigenciaInicio { get; set; }
        
        public DateOnly dataVigenciaFim { get; set; }
        
        public DateTime dataAtualizacao { get; set; }
    }
}
