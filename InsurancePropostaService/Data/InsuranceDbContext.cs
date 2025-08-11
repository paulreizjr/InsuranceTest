using InsurancePropostaService.Models;
using Microsoft.EntityFrameworkCore;

namespace InsurancePropostaService.Data
{
    public class InsuranceDbContext : DbContext
    {
        public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options) : base(options)
        {
        }

        public DbSet<PropostaEFModel> Propostas { get; set; }
        public DbSet<ContratoPropostaEFModel> ContratoPropostas { get; set; }

    }
}
