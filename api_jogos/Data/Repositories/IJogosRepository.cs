using api_jogos.Model.Entities;

namespace api_jogos.Data.Repositories
{
    public interface IJogosRepository
    {
        Task<List<JogosEntity>> PegarTodosJogos();
        Task<JogosEntity> CriarJogo(JogosCriarEntity jogo);
    }
}
