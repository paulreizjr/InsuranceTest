using InsuranceCoreBusiness.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurancePropostaService.Models
{
    [Table("Proposta")]
    public class PropostaEFModel
    {
        [Key]
        public string? id { get; set; }
        public string? automovel { get; set; }
        public decimal valorAutomovel { get; set; }
        public decimal fatorPeso { get; set; }
        public string? condutor { get; set; }
        public EStatusProposta statusProposta { get; set; }
        public decimal valorProposta { get; set; }
        public DateTime dataAtualizacao { get; set; }
    }
}
