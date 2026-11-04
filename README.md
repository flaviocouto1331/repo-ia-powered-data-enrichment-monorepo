# 🎮 repo-ia-powered-data-enrichment-monorepo

## 1. AI-Powered Data Enrichment

Este repositório contém uma solução em monorepo voltada para o **enriquecimento automatizado de dados de jogos** utilizando Inteligência Artificial (LLM - Llama 3 via Ollama).

O fluxo principal consiste em receber um cadastro básico de um jogo e delegar à IA a tarefa de completar inteligentemente os campos faltantes (como gênero, plataformas e descrição detalhada).

---

## 2. Estrutura do Monorepo

A solução está dividida nos seguintes módulos e pastas:

```text
repo-ia-powered-data-enrichment-monorepo/
├── api_jogos/              # API Web ASP.NET Core (Ponto de entrada para requisições)
├── fn_jogos_llm_llama3/    # Azure Function para processamento orientado a eventos
├── wks_jogos_llm_llama3/   # Serviço Web/Background híbrido para processamento e endpoints HTTP
├── docker/                 # Configurações e ficheiros do Docker Compose
├── ollama/                 # Configurações e modelfiles do Llama 3
└── scripts/                # Scripts de automação e utilitários do SQL
```

---

## 3. Tutorial

* **Passo 1:** Clonar o projeto para sua máquina local.
* **Passo 2:** Executar o docker compose referente na pasta do projeto `/docker/docker-compose...yml`.
* **Passo 3:** Executar o comando para baixar o modelo no container do Ollama (consulte a referência em `/ollama/ollama-docker.txt`):
  ```bash
  docker exec -it container-03112026-ollama ollama pull llama3
  ```
* **Passo 4:** Utilizando uma ferramenta de gerenciamento e visualização de banco de dados, executar o script da pasta do projeto `/scripts/scripts.sql`.
* **Passo 5:** No navegador utilize a seguinte URL do Swagger:  
  `http://localhost:1000/swagger/index.html`
* **Passo 6:** Siga o procedimento abaixo.

---

### Procedimento de Teste

Execute o endpoint **POST** `/api/jogos-azure-function`

**Payload de Requisição:**
```json
{
  "nomeJogo": "Sonic the Hedgehog"
}
```

Aguarde o retorno da API que está utilizando Inteligência Artificial (LLM - Llama 3 via Ollama) para completar o cadastro:

**Resposta Esperada:**
```json
{
  "sucesso": true,
  "mensagem": "Jogo enriquecido com sucesso via Llama 3",
  "dados": {
    "nomeJogo": "Sonic the Hedgehog",
    "generoJogo": "Ação/Esportes",
    "plataformaJogo": "PC, PlayStation, Xbox, Nintendo Switch",
    "descricaoJogo": "Jogo de plataforma criado por Yuji Naka e desenvolvido pela Sega..."
  }
}
```
