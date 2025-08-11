namespace InsuranceContratacaoService.DTOs
{
    public class GerarContratoRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public DateOnly DataVigenciaInicio { get; set; }
        public DateOnly DataVigenciaFim { get; set; }
    }
}
