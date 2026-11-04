use master

CREATE DATABASE db_jogos
COLLATE Latin1_General_CI_AI;
GO

use db_jogos

create table Jogos
(
	Id uniqueidentifier not null default newid(),
	DataCadstro datetime not null default getdate(),
	StatusCadastro bit not null default 1,
	NomeJogo nvarchar(200) NOT NULL,
	GeneroJogo nvarchar(200) not null,
	PlataformaJogo nvarchar(2000) not null,
	DescricaoJogo nvarchar(max) not null
)


insert into Jogos 
(NomeJogo, GeneroJogo, PlataformaJogo, DescricaoJogo)
values ('Final Fight', 'Beat em up', 'Arcade, Super Nintendo Entertainment System', 'O Final Fight é um clássico jogo eletrônico do gênero beat em up (briga de rua) desenvolvido e publicado pela Capcom em 1989 para os fliperamas (Arcade).')


select * from Jogos