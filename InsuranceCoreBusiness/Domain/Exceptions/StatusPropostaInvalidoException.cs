using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCoreBusiness.Domain.Exceptions
{
    public class StatusPropostaInvalidoException : Exception
    {
        public StatusPropostaInvalidoException()
            : base("Status da proposta inválido.")
        {
        }
        public StatusPropostaInvalidoException(string message)
            : base(message)
        {
        }
        public StatusPropostaInvalidoException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
