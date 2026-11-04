namespace api_jogos.Model.Response
{
    public class JogosResponse
    {
        public Guid Id { get; set; }
        public DateTime DataCadstro { get; set; }
        public bool StatusCadastro { get; set; }
        public string NomeJogo { get; set; } = null!;
        public string GeneroJogo { get; set; } = null!;
        public string PlataformaJogo { get; set; } = null!;
        public string DescricaoJogo { get; set; } = null!;
    }
}
