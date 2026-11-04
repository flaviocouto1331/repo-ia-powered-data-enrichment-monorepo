# repo-ia-powered-data-enrichment-monorepo
repo-ia-powered-data-enrichment-monorepo

1 - AI-Powered Data Enrichment

Este repositório contém uma solução em monorepo voltada para o **enriquecimento automatizado de dados de jogos** utilizando Inteligência Artificial (LLM - Llama 3 via Ollama). 

O fluxo principal consiste em receber um cadastro básico de um jogo e delegar à IA a tarefa de completar inteligentemente os campos faltantes (como género, plataformas e descrição detalhada).

2-  Estrutura do Monorepo

A solução está dividida nos seguintes módulos e pastas:

repo-ia-powered-data-enrichment-monorepo/
├── api_jogos/              # API Web ASP.NET Core (Ponto de entrada para requisições)
├── fn_jogos_llm_llama3/    # Azure Function para processamento orientado a eventos
├── wks_jogos_llm_llama3/   # Serviço Web/Background híbrido para processamento e endpoints HTTP
├── docker/                 # Configurações e ficheiros do Docker Compose
├── ollama/                 # Configurações e modelfiles do Llama 3
└── scripts/                # Scripts de automação e utilitários do SQL

3- U+1F3AE
