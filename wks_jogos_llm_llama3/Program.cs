using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using wks_jogos_llm_llama3.Model.Request;
using wks_jogos_llm_llama3.Model.Response;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient(string.Empty, client =>
{
    client.Timeout = TimeSpan.FromMinutes(3);
});

var app = builder.Build();

app.MapPost("/api/worker-service-ia", async ([FromBody] JogosRequest? requisicao, IConfiguration _configuracao, HttpClient _http, ILogger<Program> _logger) =>
{
    _logger.LogInformation("worker-service-ia - iniciando processo.");

    try
    {
        #region Validação

        if (requisicao is null || string.IsNullOrWhiteSpace(requisicao.NomeJogo))
        {
            throw new ArgumentException("Nome do jogo obrigatório.", nameof(requisicao));
        }

        #endregion

        #region Configuração Ollama

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

        #endregion

        #region Requisição Ollama

        var ollamaApi = await _http.PostAsJsonAsync(ollamaUrl, ollamaPayload);

        if (!ollamaApi.IsSuccessStatusCode)
        {
            var erro = await ollamaApi.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Erro ao comunicar com o Ollama: {ollamaApi.StatusCode} - {erro}");
        }

        var ollamaApiResponse = await ollamaApi.Content.ReadAsStringAsync();

        var ollamaApiSomenteCampoResponse = JsonSerializer.Deserialize<OllamaApiResponse>(
            ollamaApiResponse,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var campoResponse = ollamaApiSomenteCampoResponse?.Response ?? throw new InvalidOperationException("Ollama retornou resposta nula.");

        campoResponse = campoResponse.Replace("```json", "").Replace("```", "").Trim();
        var startIndex = campoResponse.IndexOf('{');
        var endIndex = campoResponse.LastIndexOf('}');
        if (startIndex >= 0 && endIndex > startIndex)
        {
            campoResponse = campoResponse.Substring(startIndex, (endIndex - startIndex) + 1);
        }

        var jogosAgenteResponse = JsonSerializer.Deserialize<JogosAgenteResponse>(
            campoResponse,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidOperationException("Falha ao desserializar a resposta da IA.");

        #endregion

        return Results.Ok(jogosAgenteResponse);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { status = false, mensagem = "Requisição inválida.", detalhe = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Json(new { status = false, mensagem = "Erro no Worker Service.", detalhe = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.Run();

public class OllamaApiResponse
{
    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;
}