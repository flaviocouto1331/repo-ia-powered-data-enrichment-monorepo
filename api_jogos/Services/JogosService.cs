using api_jogos.Data.Repositories;
using api_jogos.Model.Entities;
using api_jogos.Model.Request;
using api_jogos.Model.Response;

namespace api_jogos.Services
{
    public class JogosService : IJogosService
    {
        private readonly IJogosRepository _repository;
        public JogosService(IJogosRepository repository) => _repository = repository;

        public async Task<JogosResponse> CriarJogo(JogosRequest jogo, JogosAgenteResponse complementoJogo)
        {
            var mapJogosCriarEntity = new JogosCriarEntity(
                jogo.NomeJogo,
                complementoJogo.GeneroJogo,
                complementoJogo.PlataformaJogo,
                complementoJogo.DescricaoJogo
                ){ };

            var jogosEntity = await _repository.CriarJogo(mapJogosCriarEntity);

            var mapJogosResponse = new JogosResponse()
            {
                Id = jogosEntity.Id,
                DataCadstro = jogosEntity.DataCadstro,
                StatusCadastro = jogosEntity.StatusCadastro,
                NomeJogo = jogosEntity.NomeJogo,
                GeneroJogo = jogosEntity.GeneroJogo,
                PlataformaJogo = jogosEntity.PlataformaJogo,
                DescricaoJogo = jogosEntity.DescricaoJogo
            };

            return mapJogosResponse;
        }

        public async Task<List<JogosResponse>> PegarTodosJogos()
        {
            var jogosEntity = await _repository.PegarTodosJogos();

            if (jogosEntity.Count == 0) return [];

            var mapJogosResponse = jogosEntity.Select(i => new JogosResponse()
            {
                Id = i.Id,
                DataCadstro = i.DataCadstro,
                StatusCadastro = i.StatusCadastro,
                NomeJogo = i.NomeJogo,
                GeneroJogo = i.GeneroJogo,
                PlataformaJogo = i.PlataformaJogo,
                DescricaoJogo = i.DescricaoJogo
            }).ToList();

            return mapJogosResponse;
        }
    }
}
