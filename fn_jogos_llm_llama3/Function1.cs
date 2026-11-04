using fn_jogos_llm_llama3.Model.Request;
using fn_jogos_llm_llama3.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace azure_function_ia_ollama_worker;

public class Function1
{
    private readonly ILogger<Function1> _logger;
    private readonly IConfiguration _configuracao;
    private readonly HttpClient _http;

    public Function1(ILogger<Function1> logger, IConfiguration configuracao, HttpClient http)
    {
        _logger = logger;
        _configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao), "Configuração inválida.");
        _http = http ?? throw new ArgumentNullException(nameof(_http), "Configuração Http inválida");
    }

    // TODO - verificar com tipo Function
    [Function("azure-function-ia")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        _logger.LogInformation("function-ia - iniciando processo.");

        try
        {
            #region Passo 1 - Requisição HTTP 

            _logger.LogInformation("Passo 1 - iniciando leitura json.");

            var requisicao = await req.ReadFromJsonAsync<JogosRequest>() ?? throw new ArgumentException("Passo 1 - json inválido."); 

            _logger.LogInformation("Passo 1 - nome do jogo {NomeJogo}", requisicao.NomeJogo);
            _logger.LogInformation("Passo 1 - finalizando leitura json");

            #endregion

            #region Passo 2 - Configuração Ollama          

            _logger.LogInformation("Passo 2 - iniciando configuração ollama.");

            var ollamaHost = _configuracao["OLLAMA_HOST"] ?? throw new InvalidOperationException("Passo 2 - configuração OLLAMA_HOST inválida.");
           
            var ollamaUrl = $"{ollamaHost}/api/generate";

            var ollamaPrompt = $" Atue estritamente como um assistente de cadastro de jogos." +
                               $" Com base no jogo '{requisicao.NomeJogo}', preencha os dados solicitados." +
                               $" Retorne APENAS um JSON válido contendo exatamente estas três chaves, sem inventar outras:" +
                               $" \"GeneroJogo\" (string), \"PlataformaJogo\" (string) e \"DescricaoJogo\" (string)." +
                               $" Importante limitar os caracteres GeneroJogo 200 PlataformaJogo 2000 e DescricaoJogo 20000." +
                               $" Exemplo de formato esperado: {{\"GeneroJogo\": \"Ação\", \"PlataformaJogo\": \"PC\", \"DescricaoJogo\": \"jogo criado em...\"}}";

            var ollamaPayload = new
            {
                model = "llama3",
                prompt = ollamaPrompt,
                format = "json",
                stream = false
            };

            _logger.LogInformation("Passo 2 - finalizando configuração ollama.");

            #endregion

            #region Passo 3 - Requisição Ollama

            _logger.LogInformation("Passo 3 - iniciando requisição ollama com llm llama3.");

            var ollamaApi = await _http.PostAsJsonAsync(ollamaUrl, ollamaPayload);

            if (!ollamaApi.IsSuccessStatusCode)
            {
                var erro = await ollamaApi.Content.ReadAsStringAsync();
                _logger.LogError("Passo 3 - erro requisição ollama. {StatusCode} - {errorBody}", ollamaApi.StatusCode, erro);
                throw new HttpRequestException($"Passo 3 - erro requisição ollama. {ollamaApi.StatusCode} - {erro}");
            }

            // 1 - Ler o conteúdo da resposta como string
            #region retorno completo do ollama com llm llama3

            /*             
            
                {
                  "model": "llama3",
                  "created_at": "2026-11-04T14:41:11.638499536Z",
                  "response": "{\"GeneroJogo\": \"Ação/Esportes\", \"PlataformaJogo\": \"PC, PlayStation, Xbox, Nintendo Switch\", \"DescricaoJogo\": \"jogo de plataforma criado por Yuji Naka e desenvolvido pela Sega, lançado originalmente em 1991 para Sega Genesis. O jogo segue as aventuras de Sonic, um hedgehog azul, que deve evitar os seguidores de Dr. Eggman e coletar Chaos Emeralds para ganhar poderes especiais. O jogo é conhecido por sua jogabilidade rápida e sua trama simplista, mas divertida.\"}",
                  "done": true,
                  "done_reason": "stop",
                  "context": [
                    128006, 882, 128007, 271, 2468, 361, 30607, 275, 9157, 8112, 4543, 7945,
                    6960, 409, 19973, 23834, 409, 31225, 437, 13, 1219, 2385, 912, 67234, 364,
                    942, 292, 518, 864, 268, 6583, 2709, 29045, 56246, 5670, 13, 10608, 17334,
                    10314, 965, 1950, 4543, 4823, 348, 5809, 75, 5362, 687, 8862, 506, 266,
                    9157, 48591, 490, 52063, 523, 4798, 11, 5347, 17459, 277, 74802, 25, 330,
                    5648, 78, 41, 24404, 1, 320, 928, 705, 330, 2169, 460, 48671, 41, 24404, 1,
                    320, 928, 8, 384, 330, 93358, 41, 24404, 1, 320, 928, 570, 13516, 5048,
                    4017, 277, 2709, 60355, 2672, 78, 41, 24404, 220, 1049, 1856, 460, 48671,
                    41, 24404, 220, 1049, 15, 384, 32285, 28773, 41, 24404, 220, 1049, 410,
                    13, 1398, 31310, 409, 55956, 31157, 2172, 25, 5324, 5648, 78, 41, 24404,
                    794, 330, 32, 10178, 78, 498, 330, 2169, 460, 48671, 41, 24404, 794, 330,
                    4977, 498, 330, 93358, 41, 24404, 794, 330, 73, 24404, 28658, 2172, 991,
                    1131, 9388, 128009, 128006, 78191, 128007, 271, 5018, 5648, 78, 41, 24404,
                    794, 330, 32, 6027, 14, 17812, 403, 288, 498, 330, 2169, 460, 48671, 41,
                    24404, 794, 330, 4977, 11, 32365, 11, 21222, 11, 23643, 15958, 498, 330,
                    93358, 41, 24404, 794, 330, 73, 24404, 409, 92308, 28658, 2172, 4247,
                    28372, 7910, 452, 13637, 384, 61344, 337, 77643, 33823, 80949, 11, 97069,
                    2172, 4113, 12826, 991, 220, 2550, 16, 3429, 80949, 41888, 13, 507, 67234,
                    37870, 439, 91806, 19879, 409, 44317, 11, 4543, 42766, 61722, 12657, 360,
                    11, 1744, 37244, 71908, 2709, 4915, 2480, 4692, 409, 2999, 13, 42313,
                    1543, 384, 1080, 1169, 277, 44205, 64196, 82, 3429, 25893, 13279, 29638,
                    288, 43938, 57567, 13, 507, 67234, 4046, 71583, 5362, 4247, 19906, 31225,
                    81016, 70838, 4849, 384, 19906, 490, 3105, 15858, 9265, 11, 9427, 37098,
                    4849, 1210, 92
                  ],
                  "total_duration": 36164950344,
                  "load_duration": 13630286992,
                  "prompt_eval_count": 167,
                  "prompt_eval_cached_count": 0,
                  "prompt_eval_duration": 5292739000,
                  "eval_count": 133,
                  "eval_duration": 17237791000
                }

            */

            #endregion
            var ollamaApiResponse = await ollamaApi.Content.ReadAsStringAsync();

            _logger.LogInformation("Passo 3 - retorno ollama {ollamaApiResponse}", ollamaApiResponse);            

            // 2 - Desserializar a resposta completa para obter apenas a propriedade "response"
            var ollamaApiSomenteCampoResponse = JsonSerializer.Deserialize<OllamaApiResponse>(
                ollamaApiResponse, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _logger.LogInformation("Passo 3 - retorno desserializado somente campo response do ollama: {ollamaApiSomenteCampoResponse}", ollamaApiSomenteCampoResponse);

            // 3 - Obter apenas o campo "response" da resposta desserializada
            var campoResponse = ollamaApiSomenteCampoResponse?.Response ?? throw new InvalidOperationException("Passo 3 - campo response inválido.");

            _logger.LogInformation("Passo 3 - retorno campo response do ollama {campoResponse}", campoResponse);

            // 3.1 - Remove blocos de markdown caso o Llama insira ```json ... ```
            campoResponse = campoResponse.Replace("```json", "").Replace("```", "").Trim();

            _logger.LogInformation("Passo 3 - retorno campo response do ollama sem blocos de markdown: {campoResponse}", campoResponse);

            // 3.2 - Pega somente a parte JSON válido (procura a primeira chave '{' e a última '}')
            var startIndex = campoResponse.IndexOf('{');
            var endIndex = campoResponse.LastIndexOf('}');
            if (startIndex >= 0 && endIndex > startIndex)
            {
                campoResponse = campoResponse.Substring(startIndex, (endIndex - startIndex) + 1);
            }

            _logger.LogInformation("Passo 3 - retorno campo response do ollama somente a parte JSON válido {campoResponse}", campoResponse);

            // 4 - Desserializar o campo "response" para a classe JogosAgenteResponse
            var jogosAgenteResponse = JsonSerializer.Deserialize<JogosAgenteResponse>(
                campoResponse,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true 
                });

            if (jogosAgenteResponse is null) throw new InvalidOperationException("Passo 3 - ollama JogosAgenteResponse inválido.");

            _logger.LogInformation("Passo 3 - ollama json para JogosAgenteResponse. {GeneroJogo} {PlataformaJogo} {DescricaoJogo}", 
                jogosAgenteResponse.GeneroJogo, 
                jogosAgenteResponse.PlataformaJogo, 
                jogosAgenteResponse.DescricaoJogo);

            _logger.LogInformation("Passo 3 - finalizando requisição ollama com llm llama3.");

            #endregion

            var okResponse = req.CreateResponse(HttpStatusCode.OK);
            _logger.LogInformation("function-ia requisição finalizada com sucesso.");
            await okResponse.WriteAsJsonAsync(jogosAgenteResponse);           
            return okResponse;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Requisição inválida.");
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteAsJsonAsync(new { status = false, mensagem = "Requisição inválida.", detalhe = ex.Message, stack = ex.StackTrace });
            return badResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na Function.");
            var serverError = req.CreateResponse(HttpStatusCode.InternalServerError);
            await serverError.WriteAsJsonAsync(new { status = false, mensagem = "Erro na Azure Function.", detalhe = ex.Message, stack = ex.StackTrace });
            return serverError;
        }
    }

    public class OllamaApiResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;
    }
}