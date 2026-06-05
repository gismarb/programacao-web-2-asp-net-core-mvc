# Projeto1_IF - ASP.NET Core MVC com Entity Framework

Projeto desenvolvido na disciplina de Programação Web II, utilizando ASP.NET Core MVC, Entity Framework Core, Identity e SQL Server.

O projeto segue a estrutura apresentada na Aula 01, com adaptação do ambiente para Linux, utilizando SQL Server em container Docker e ferramentas de linha de comando do .NET.

## Tecnologias utilizadas

- C#
- ASP.NET Core MVC
- .NET 8
- Entity Framework Core
- ASP.NET Core Identity
- SQL Server 2022
- Docker
- Visual Studio Code
- Git e GitHub

## Banco de dados

O banco de dados utilizado é o `db_IF`, criado a partir do script fornecido na disciplina.

O projeto segue a abordagem **Database First**, ou seja, as classes Models e o DbContext foram gerados a partir da estrutura existente do banco de dados.

## Estrutura principal

- `Controllers/`: controladores MVC da aplicação.
- `Models/`: classes geradas a partir das tabelas do banco.
- `Data/`: contextos do Entity Framework.
- `Views/`: páginas Razor utilizadas pelos controllers.
- `Areas/Identity/`: estrutura relacionada ao ASP.NET Core Identity.

## Entregas

### Entrega 01 - Tarefa de acompanhamento 1

Atividade solicitada: seguir todas as etapas da Aula 01 e adicionar um controlador de um model à escolha.

Model escolhido:

- `TbSuplemento`

Arquivos principais criados:

- `Controllers/TbSuplementosController.cs`
- `Views/TbSuplementos/`

> Observação: conforme solicitado na atividade, meu nome (Gismar Pereira Barbosa) foi inserido acima do namespace no controller criado.
> Acesso direto à [Controllers/TbSuplementosController.cs](Controllers/TbSuplementosController.cs)

### Entrega 02

A ser detalhada futuramente.

### Entrega 03

A ser detalhada futuramente.

## Observações

- Este projeto foi desenvolvido em ambiente Linux, reproduzindo o fluxo da aula originalmente apresentada em ambiente Windows com Visual Studio.

- Foram utilizadas ferramentas CLI do .NET para substituir os recursos visuais do Visual Studio, como geração de Models, DbContext, Controllers e Views.