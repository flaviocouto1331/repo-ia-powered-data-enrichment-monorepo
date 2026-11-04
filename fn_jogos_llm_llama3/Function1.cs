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

    // TODO - verificar com tipo Function e nao Anonymous
    [Function("function-ia")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        _logger.LogInformation("function-ia - iniciando processo.");

        try
        {
            #region Requisição HTTP 

            _logger.LogInformation("iniciando leitura json.");

            var requisicao = await req.ReadFromJsonAsync<JogosRequest>();

            if (requisicao is null) 
            {
                _logger.LogError("json inválido.");
                throw new ArgumentException("json inválido.");
            }

            _logger.LogInformation("nome do jogo {NomeJogo}", requisicao.NomeJogo);
            _logger.LogInformation("finalizando leitura json");

            #endregion

            #region Configuração Ollama          

            _logger.LogInformation("iniciando configuração ollama.");

            var ollamaHost = _configuracao["OLLAMA_HOST"] ?? throw new InvalidOperationException("OLLAMA_HOST inválido.");
           
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

            _logger.LogInformation("finalizando configuração ollama.");

            #endregion

            #region Requisição Ollama

            _logger.LogInformation("iniciando requisição ollama.");

            var ollamaApi = await _http.PostAsJsonAsync(ollamaUrl, ollamaPayload);

            if (!ollamaApi.IsSuccessStatusCode)
            {
                var erro = await ollamaApi.Content.ReadAsStringAsync();
                _logger.LogError("erro requisição ollama. {StatusCode} - {errorBody}", ollamaApi.StatusCode, erro);
                throw new HttpRequestException($"erro requisição ollama. {ollamaApi.StatusCode} - {erro}");
            }

            // 1 - Ler o conteúdo da resposta como string
            /*             
            {
              "model": "llama3",
              "created_at": "2026-03-30T10:00:00Z",
              "response": "```json\n{\n  \"GeneroJogo\": \"RPG de Ação\",\n  \"PlataformaJogo\": \"PC, PlayStation 4, Xbox One, Nintendo Switch\",\n  \"DescricaoJogo\": \"The Witcher 3: Wild Hunt é um jogo eletrônico de RPG de ação desenvolvido...\"\n}\n```",
              "done": true,
              "total_duration": 1845000000,
              "load_duration": 120000000,
              "prompt_eval_count": 142,
              "eval_count": 85,
              "eval_duration": 1640000000
            }
            */
            var ollamaApiResponse = await ollamaApi.Content.ReadAsStringAsync();

            _logger.LogInformation("retorno completo ollama: {ollamaApiResponse}", ollamaApiResponse);            

            // 2 - Desserializar a resposta completa para obter apenas a propriedade "response"
            var ollamaApiSomenteCampoResponse = JsonSerializer.Deserialize<OllamaApiResponse>(
                ollamaApiResponse, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _logger.LogInformation("retorno desserializado ollama: {ollamaApiSomenteCampoResponse}", ollamaApiSomenteCampoResponse);

            // 3 - Obter apenas o campo "response" da resposta desserializada
            var campoResponse = ollamaApiSomenteCampoResponse?.Response ?? throw new InvalidOperationException("ollama nulo ou vazio.");

            _logger.LogInformation("retorno ollama OllamaApiResponse Response: {campoResponse}", campoResponse);

            // 3.1 - Remove blocos de markdown caso o Llama insira ```json ... ```
            campoResponse = campoResponse.Replace("```json", "").Replace("```", "").Trim();

            _logger.LogInformation("retorno ollama sem blocos de markdown: {campoResponse}", campoResponse);

            // 3.2 - Pega somente a parte JSON válido (procura a primeira chave '{' e a última '}')
            var startIndex = campoResponse.IndexOf('{');
            var endIndex = campoResponse.LastIndexOf('}');
            if (startIndex >= 0 && endIndex > startIndex)
            {
                campoResponse = campoResponse.Substring(startIndex, (endIndex - startIndex) + 1);
            }

            _logger.LogInformation("retorno ollama somente a parte JSON válido: {campoResponse}", campoResponse);

            // 4 - Desserializar o campo "response" para a classe JogosAgenteResponse
            var jogosAgenteResponse = JsonSerializer.Deserialize<JogosAgenteResponse>(
                campoResponse,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true 
                });

            if (jogosAgenteResponse is null)
            {
                _logger.LogError("O ollama JogosAgenteResponse inválido.");
                throw new InvalidOperationException("ollama JogosAgenteResponse inválido.");
            }

            _logger.LogInformation("JSON convertido com sucesso para JogosAgenteResponse. {GeneroJogo} {PlataformaJogo} {DescricaoJogo}", 
                jogosAgenteResponse.GeneroJogo, 
                jogosAgenteResponse.PlataformaJogo, 
                jogosAgenteResponse.DescricaoJogo);

            #endregion

            var okResponse = req.CreateResponse(HttpStatusCode.OK);
            _logger.LogInformation("function-ia requisição finalizada com sucesso.");
            await okResponse.WriteAsJsonAsync(jogosAgenteResponse);           
            return okResponse;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Requisição inválida.");
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteAsJsonAsync(new { status = false, mensagem = "Requisição inválida.", detalhe = ex.Message, stack = ex.StackTrace });
            return badResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na Function.");
            var serverError = req.CreateResponse(HttpStatusCode.InternalServerError);
            await serverError.WriteAsJsonAsync(new { status = false, mensagem = "Erro na Function.", detalhe = ex.Message, stack = ex.StackTrace });
            return serverError;
        }
    }

    public class OllamaApiResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;
    }
}