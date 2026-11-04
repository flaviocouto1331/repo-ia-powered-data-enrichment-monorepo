using api_jogos.Model.Request;
using api_jogos.Model.Response;

namespace api_jogos.Services
{
    public interface IJogosService
    {
        Task<List<JogosResponse>> PegarTodosJogos();
        Task<JogosResponse> CriarJogo(JogosRequest jogo, JogosAgenteResponse complementoJogo);
    }
}
