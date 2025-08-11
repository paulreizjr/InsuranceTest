using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsurancePropostaService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proposta",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    automovel = table.Column<string>(type: "text", nullable: true),
                    valorAutomovel = table.Column<decimal>(type: "numeric", nullable: false),
                    fatorPeso = table.Column<decimal>(type: "numeric", nullable: false),
                    condutor = table.Column<string>(type: "text", nullable: true),
                    statusProposta = table.Column<int>(type: "integer", nullable: false),
                    valorProposta = table.Column<decimal>(type: "numeric", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposta", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ContratoProposta",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    idProposta = table.Column<string>(type: "text", nullable: true),
                    dataVigenciaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    dataVigenciaFim = table.Column<DateOnly>(type: "date", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratoProposta", x => x.id);
                    table.ForeignKey(
                        name: "FK_ContratoProposta_Proposta_idProposta",
                        column: x => x.idProposta,
                        principalTable: "Proposta",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContratoProposta_idProposta",
                table: "ContratoProposta",
                column: "idProposta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContratoProposta");

            migrationBuilder.DropTable(
                name: "Proposta");
        }
    }
}
