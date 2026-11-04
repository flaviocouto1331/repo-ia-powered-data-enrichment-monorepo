using System.Data;
using Microsoft.Data.SqlClient;

namespace api_jogos.Data.Dapper
{
    public class DapperContext
    {
        private readonly IConfiguration _configuracao;
        private readonly string _conexao;

        public DapperContext(IConfiguration configuracao) 
        {
            _configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao), "Configuração inválida.");
            _conexao = _configuracao.GetConnectionString("conexao") ?? throw new InvalidOperationException("Conexão inválida.");
        }

        public IDbConnection DapperConnection() => new SqlConnection(_conexao);
    }
}
