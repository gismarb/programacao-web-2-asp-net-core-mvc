# Projeto1_IF - ASP.NET Core MVC com Entity Framework

Projeto desenvolvido na disciplina de Programação Web II, utilizando ASP.NET Core MVC, Entity Framework Core, Identity e SQL Server.

O projeto segue a estrutura apresentada nas aulas da disciplina, com adaptação do ambiente para Linux, utilizando SQL Server em container Docker e ferramentas de linha de comando do .NET.

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

Não foram utilizadas migrations para alterar a estrutura do banco da disciplina, respeitando a orientação de manter a base compatível com o script fornecido pelo professor.

## Estrutura principal

- `Controllers/`: controladores MVC da aplicação.
- `Models/`: classes geradas a partir das tabelas do banco.
- `Data/`: contextos do Entity Framework.
- `Views/`: páginas Razor utilizadas pelos controllers.
- `Areas/Identity/`: estrutura relacionada ao ASP.NET Core Identity.
- `ViewModels/`: classes auxiliares usadas para estruturar dados específicos de telas e fluxos de negócio.
- `wwwroot/js/`: arquivos JavaScript utilizados pela aplicação.
- `docs/`: documentação complementar do projeto.
- `docs/sql/`: scripts SQL de preparação, apoio e conferência.

## Entregas

### Entrega 01 - Tarefa de acompanhamento 1

Atividade solicitada: seguir todas as etapas da Aula 01 e adicionar um controlador de um model à escolha.

Model escolhido:

- `TbSuplemento`

Arquivos principais criados:

- `Controllers/TbSuplementosController.cs`
- `Views/TbSuplementos/`

> Observação: conforme solicitado na atividade, meu nome (Gismar Pereira Barbosa) foi inserido acima do namespace no controller criado.
> Acesso direto à [Controllers/TbSuplementosController.cs](Controllers/TbSuplementosController.cs).

### Entrega 02 - Tarefa de acompanhamento 2

Atividade solicitada: construir um controlador para o model `TbPaciente`, alterar as actions conforme visto nas aulas e construir o arquivo `jquery.validate.pt-br.js`.

Model utilizado:

- `TbPaciente`

Controller criado e ajustado:

- `Controllers/TbPacientesController.cs`

Views geradas e ajustadas:

- `Views/TbPacientes/Index.cshtml`
- `Views/TbPacientes/Create.cshtml`
- `Views/TbPacientes/Edit.cshtml`
- `Views/TbPacientes/Details.cshtml`
- `Views/TbPacientes/Delete.cshtml`

Arquivo JavaScript criado:

- `wwwroot/js/jquery.validate.pt-br.js`

Arquivo de validação referenciado em:

- `Views/Shared/_ValidationScriptsPartial.cshtml`

Actions trabalhadas no controller:

- `Create` com uso de `Bind`
- `Details` com consulta somente leitura usando `AsNoTracking()`
- `Edit` GET
- `EditPost` com `TryUpdateModelAsync`
- `Delete` com tratamento para erro de exclusão
- `DeleteConfirmed` com `try/catch` para `DbUpdateException`

Ajustes adicionais realizados:

- Campo de cidade configurado para exibir o nome da cidade no formulário, mantendo o `IdCidade` como valor persistido.
- Views de `Index`, `Details` e `Delete` ajustadas para exibir `NomeCidade` e o nome da cidade, em vez do código numérico.
- Arquivo `jquery.validate.pt-br.js` criado para adequar validações de data e número ao formato brasileiro.
- Nome do estudante mantido como comentário no controller e no arquivo JavaScript, conforme solicitado na atividade.

> Acesso direto à [Controllers/TbPacientesController.cs](Controllers/TbPacientesController.cs).
> Acesso direto à [wwwroot/js/jquery.validate.pt-br.js](wwwroot/js/jquery.validate.pt-br.js).

### Entrega 03 - Trabalho Final ASP.NET Core MVC

Atividade solicitada: evoluir o projeto para contemplar regras de autenticação, autorização por perfis, cadastro de profissionais, área gerencial e gestão de pacientes por profissional.

Resumo da entrega:

- Cadastro do próprio profissional como Médico ou Nutricionista.
- Uso de ASP.NET Core Identity com Roles específicas.
- Criação de três perfis gerenciais: Gerente Médico, Gerente Nutricionista e Gerente Geral.
- Restrição de acesso por perfil usando `[Authorize]` e filtros no controller.
- Área do profissional para visualização e edição dos próprios dados.
- Bloqueio de alteração do CPF pelo próprio profissional.
- Área gerencial para consulta, edição e exclusão de profissionais conforme perfil do gerente.
- Permissão para gerente alterar CPF de profissional.
- Bloqueio de exclusão de profissional com paciente vinculado.
- CRUD funcional de pacientes por profissional, usando vínculo em `tbMedico_Paciente`.
- Ajustes de navegação por perfil no layout principal.
- Página de acesso negado customizada em português.
- Mensagens globais de sucesso e erro centralizadas no layout.

Arquivos principais trabalhados:

- `Program.cs`
- `Controllers/ProfissionaisController.cs`
- `ViewModels/`
- `Views/Profissionais/`
- `Views/Shared/_Layout.cshtml`
- `Areas/Identity/Pages/Account/AccessDenied.cshtml`
- `Areas/Identity/Pages/Account/AccessDenied.cshtml.cs`
- `docs/`
- `docs/sql/`

Documentação complementar da entrega:

- [Manual de execução](docs/manual_execucao.md)
- [Requisitos resumidos](docs/requisitos_resumidos.md)
- [Roteiro de testes funcionais](docs/roteiro_testes.md)
- [Consultas SQL de apoio aos testes](docs/sql/consultas_apoio_testes_parte_3.sql)
- [Seed da Parte 3](docs/sql/seed_parte_3.sql)
- [Seed dos usuários gerenciais](docs/sql/seed_gerentes_parte_3.sql)
- [Script opcional para remover vínculo de teste](docs/sql/opcional_remover_vinculo_teste_paciente_profissional.sql)

> A Entrega 03 foi documentada de forma separada em `docs/` para facilitar a conferência dos requisitos, a execução do projeto e a validação funcional.

## Referências

### Ambiente base e .NET no Linux

- [Instalar o .NET no Linux - Microsoft Learn](https://learn.microsoft.com/dotnet/core/install/linux)
- [Instalar o .NET no Ubuntu - Microsoft Learn](https://learn.microsoft.com/dotnet/core/install/linux-ubuntu)
- [Ferramentas CLI do .NET - Microsoft Learn](https://learn.microsoft.com/dotnet/core/tools/)
- [Imagens oficiais do .NET para Docker - Microsoft Learn](https://learn.microsoft.com/dotnet/architecture/microservices/net-core-net-framework-containers/official-net-docker-images)

### SQL Server e banco de dados

- [Executar SQL Server em container Docker - Microsoft Learn](https://learn.microsoft.com/sql/linux/quickstart-install-connect-docker)
- [Instalar SQL Server no Ubuntu - Microsoft Learn](https://learn.microsoft.com/sql/linux/quickstart-install-connect-ubuntu)
- [Entity Framework Core - Microsoft Learn](https://learn.microsoft.com/ef/core/)
- [Scaffolding / Reverse Engineering com EF Core - Microsoft Learn](https://learn.microsoft.com/ef/core/managing-schemas/scaffolding/)
- [Ferramentas de linha de comando do EF Core - Microsoft Learn](https://learn.microsoft.com/ef/core/cli/dotnet)

### ASP.NET Core MVC, Identity e Scaffolding

- [ASP.NET Core MVC - Microsoft Learn](https://learn.microsoft.com/aspnet/core/mvc/overview)
- [ASP.NET Core Identity - Microsoft Learn](https://learn.microsoft.com/aspnet/core/security/authentication/identity)
- [Scaffold Identity em projetos ASP.NET Core - Microsoft Learn](https://learn.microsoft.com/aspnet/core/security/authentication/scaffold-identity)
- [dotnet aspnet-codegenerator - Microsoft Learn](https://learn.microsoft.com/aspnet/core/fundamentals/tools/dotnet-aspnet-codegenerator)

### CRUD básico com ASP.NET Core MVC e EF Core

- [Tutorial: Implementar funcionalidade CRUD básica - ASP.NET MVC com EF Core](https://learn.microsoft.com/aspnet/core/data/ef-mvc/crud?view=aspnetcore-8.0)

### Estratégias de programação utilizadas na Entrega 03

- [Views em ASP.NET Core MVC - Microsoft Learn](https://learn.microsoft.com/aspnet/core/mvc/views/overview)
- [Layouts em ASP.NET Core - Microsoft Learn](https://learn.microsoft.com/aspnet/core/mvc/views/layout)
- [Tag Helpers em ASP.NET Core - Microsoft Learn](https://learn.microsoft.com/aspnet/core/mvc/views/tag-helpers/intro)
- [Trabalhar com formulários em ASP.NET Core - Microsoft Learn](https://learn.microsoft.com/aspnet/core/mvc/views/working-with-forms)
- [Partial Views em ASP.NET Core - Microsoft Learn](https://learn.microsoft.com/aspnet/core/mvc/views/partial)
- [Model Binding em ASP.NET Core - Microsoft Learn](https://learn.microsoft.com/aspnet/core/mvc/models/model-binding)
- [Validação de Models em ASP.NET Core MVC - Microsoft Learn](https://learn.microsoft.com/aspnet/core/mvc/models/validation)
- [ViewModels e Views fortemente tipadas em ASP.NET MVC - Microsoft Learn](https://learn.microsoft.com/aspnet/mvc/overview/older-versions-1/nerddinner/use-viewdata-and-implement-viewmodel-classes)
- [Autorização baseada em Roles no ASP.NET Core - Microsoft Learn](https://learn.microsoft.com/aspnet/core/security/authorization/roles)
- [Introdução à autorização no ASP.NET Core - Microsoft Learn](https://learn.microsoft.com/aspnet/core/security/authorization/introduction)
- [Injeção de dependência em Controllers ASP.NET Core - Microsoft Learn](https://learn.microsoft.com/aspnet/core/mvc/controllers/dependency-injection)
- [Consultas com LINQ no Entity Framework Core - Microsoft Learn](https://learn.microsoft.com/ef/core/querying/)
- [Consultas Tracking e No-Tracking no Entity Framework Core - Microsoft Learn](https://learn.microsoft.com/ef/core/querying/tracking)
- [Operadores de consulta LINQ em C# - Microsoft Learn](https://learn.microsoft.com/dotnet/csharp/linq/standard-query-operators/)
- [Gerenciamento de estado e TempData no ASP.NET Core - Microsoft Learn](https://learn.microsoft.com/aspnet/core/fundamentals/app-state)

## Observações

- Este projeto foi desenvolvido em ambiente Linux, reproduzindo o fluxo das aulas originalmente apresentadas em ambiente Windows com Visual Studio.
- Foram utilizadas ferramentas CLI do .NET para substituir os recursos visuais do Visual Studio, como geração de Models, DbContext, Controllers e Views.
- O SQL Server foi executado em container Docker com volume persistente.
- O banco foi tratado com abordagem Database First, evitando alterações estruturais por migrations.
