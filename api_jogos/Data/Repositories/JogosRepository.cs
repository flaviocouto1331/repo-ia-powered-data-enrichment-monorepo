using api_jogos.Data.Dapper;
using api_jogos.Model.Entities;
using Dapper;
using System.Data;
using System.Text;

namespace api_jogos.Data.Repositories
{
    public class JogosRepository : IJogosRepository
    {
        private readonly DapperContext _dapper;
        public JogosRepository(DapperContext dapper) => _dapper = dapper ?? throw new ArgumentNullException(nameof(dapper), "Dapper inválido.");

        public async Task<JogosEntity> CriarJogo(JogosCriarEntity jogo)
        {
            StringBuilder query = new();
            query.AppendLine(" insert into Jogos (NomeJogo, GeneroJogo, PlataformaJogo, DescricaoJogo)");
            query.AppendLine(" output inserted.Id, inserted.DataCadstro, inserted.StatusCadastro, inserted.NomeJogo, inserted.GeneroJogo, inserted.PlataformaJogo, inserted.DescricaoJogo");
            query.AppendLine(" values (@NomeJogo, @GeneroJogo, @PlataformaJogo, @DescricaoJogo)");
            using var _ctx = _dapper.DapperConnection();
            var jogoResponso = await _ctx.QuerySingleAsync<JogosEntity>(
                query.ToString(),
                jogo,
                commandType: CommandType.Text
                );
            return jogoResponso;
        }

        public async Task<List<JogosEntity>> PegarTodosJogos()
        {
            StringBuilder query = new();
            query.Append(" select Id, DataCadstro, StatusCadastro, NomeJogo, GeneroJogo, PlataformaJogo, DescricaoJogo from Jogos order by DataCadstro desc");
            using var _ctx = _dapper.DapperConnection();
            var jogos = await _ctx.QueryAsync<JogosEntity>(
                query.ToString(),
                commandType: CommandType.Text
                );
            return [.. jogos];
        }
    }
}
