using api_jogos.Model.Request;
using api_jogos.Model.Response;
using api_jogos.Services;

namespace api_jogos.Endpoint
{
    public static class JogosEndpoint
    {
        public static void AddJogosEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/jogos", async (IJogosService _service) => {
                try
                {
                    var jogos = await _service.PegarTodosJogos();

                    if (jogos.Count == 0) return Results.Ok(new { status = true, mensagem = "Nenhum registro encontrado.", Jogos = new List<JogosResponse>() });

                    return Results.Ok(new { status = true, mensagem = "Registro encontrado.", Jogos = jogos });
                }
                catch (ArgumentException ex) 
                {
                    return Results.BadRequest(new { status = false, mensagem = "Requisição inválida.", detalhe = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Json(new { status = false, mensagem = "Erro na Api.", detalhe = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
                }
            }).WithTags("Jogos");

            app.MapPost("/api/jogos-azure-function", async (JogosRequest _jogo, IConfiguration _configuracao, HttpClient _http, IJogosService _service) => {
                try
                {
                    if (_jogo is null || string.IsNullOrWhiteSpace(_jogo.NomeJogo)) throw new ArgumentException("Nome do jogo obrigatório.", nameof(_jogo));

                    #region Azure Function da Plataforma Ollama com LLM llama3 IA

                    var url = _configuracao["AzureFunctionOllamaUrl"] ?? throw new InvalidOperationException("Configuração AzureFunctionOllamaUrl inválida.");
                    
                    var response = await _http.PostAsJsonAsync(url, _jogo);

                    if (!response.IsSuccessStatusCode)
                    {
                        var erro = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Erro na Function {erro}");
                    }

                    var complementoJogo = await response.Content.ReadFromJsonAsync<JogosAgenteResponse>() ?? throw new InvalidOperationException("Function retornou inválida.");

                    #endregion

                    var jogo = await _service.CriarJogo(_jogo, complementoJogo);

                    return Results.Ok(new { status = true, mensagem = "Registro inserido com sucesso.", Jogos = jogo });
                }
                catch (ArgumentException ex) 
                {
                    return Results.BadRequest(new { status = false, mensagem = "Requisição inválida.", detalhe = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Json(new { status = false, mensagem = "Erro na Api", detalhe = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithTags("Jogos")
            .WithDescription("Plataforma Ollama\n\nLLM llama3\n\nIA Powered Data Enrichment\n\nPasso 1 - enviar o nome do jogo\n\nPasso 2 - IA vai completar o cadastro inserindo os dados restantes sobre o jogo");

            app.MapPost("/api/jogos-worker-service", async (JogosRequest _jogo, IConfiguration _configuracao, HttpClient _http, IJogosService _service) => {
                try
                {
                    if (_jogo is null || string.IsNullOrWhiteSpace(_jogo.NomeJogo)) throw new ArgumentException("Nome do jogo obrigatório.", nameof(_jogo));

                    #region Azure Function da Plataforma Ollama com LLM llama3 IA

                    var url = _configuracao["WorkerServiceOllamaUrl"] ?? throw new InvalidOperationException("Configuração WorkerServiceOllamaUrl inválida.");

                    var response = await _http.PostAsJsonAsync(url, _jogo);

                    if (!response.IsSuccessStatusCode)
                    {
                        var erro = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Erro na Function {erro}");
                    }

                    var complementoJogo = await response.Content.ReadFromJsonAsync<JogosAgenteResponse>() ?? throw new InvalidOperationException("Function retornou inválida.");

                    #endregion

                    var jogo = await _service.CriarJogo(_jogo, complementoJogo);

                    return Results.Ok(new { status = true, mensagem = "Registro inserido com sucesso.", Jogos = jogo });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { status = false, mensagem = "Requisição inválida.", detalhe = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Json(new { status = false, mensagem = "Erro na Api", detalhe = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithTags("Jogos")
            .WithDescription("Plataforma Ollama\n\nLLM llama3\n\nIA Powered Data Enrichment\n\nPasso 1 - enviar o nome do jogo\n\nPasso 2 - IA vai completar o cadastro inserindo os dados restantes sobre o jogo");
        }
    }
}
