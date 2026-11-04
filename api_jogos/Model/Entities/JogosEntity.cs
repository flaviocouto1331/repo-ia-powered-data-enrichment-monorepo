namespace api_jogos.Model.Entities
{
    public class JogosEntity : BaseEntity
    {
        public JogosEntity() { }
        public JogosEntity(
            Guid id,
            DateTime dataCadastro,
            bool statusCadastro,
            string nomeJogo,
            string generoJogo,
            string plataformaJogo,
            string descricaoJogo
            ) : base(id) 
        {
            DataCadstro = dataCadastro;
            StatusCadastro = statusCadastro;
            NomeJogo = nomeJogo;
            GeneroJogo = generoJogo;
            PlataformaJogo = plataformaJogo;
            DescricaoJogo = descricaoJogo;
        }           

        public DateTime DataCadstro { get; private set; }
        public bool StatusCadastro { get; private set; }
        public string NomeJogo { get; private set; } = null!;
        public string GeneroJogo { get; private set; } = null!;
        public string PlataformaJogo { get; private set; } = null!;
        public string DescricaoJogo { get; private set; } = null!;
    }
}
