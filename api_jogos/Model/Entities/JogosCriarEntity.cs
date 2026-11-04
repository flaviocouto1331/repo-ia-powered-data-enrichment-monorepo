namespace api_jogos.Model.Entities
{
    public class JogosCriarEntity
    {
        public JogosCriarEntity() { }
        public JogosCriarEntity(
            string nomeJogo,
            string generoJogo,
            string plataformaJogo,
            string descricaoJogo
            ) 
        {
            NomeJogo = nomeJogo.Trim().ToUpper();
            GeneroJogo = generoJogo;
            PlataformaJogo = plataformaJogo;
            DescricaoJogo = descricaoJogo;
        }           

        public string NomeJogo { get; private set; } = null!;
        public string GeneroJogo { get; private set; } = null!;
        public string PlataformaJogo { get; private set; } = null!;
        public string DescricaoJogo { get; private set; } = null!;
    }
}
