# Manual de Execução

**Projeto:** Programação Web II — ASP.NET Core MVC  
**Aluno:** Gismar Pereira Barbosa  
**Banco:** SQL Server / `db_IF`  
**Ambiente utilizado no desenvolvimento:** Linux com .NET 8, Docker e VS Code

---

## 1. Objetivo

Este documento apresenta as instruções mínimas para preparar, abrir, compilar e executar o projeto ASP.NET Core MVC.

O projeto foi desenvolvido em Linux, mas também pode ser aberto em uma IDE compatível com projetos .NET, como Visual Studio, JetBrains Rider ou Visual Studio Code.

---

## 2. Pré-requisitos

Para executar o projeto, é necessário ter instalado:

```text
- Git
- .NET SDK 8
- Docker
- SQL Server em container Docker ou uma instância SQL Server acessível
- Visual Studio Code, Visual Studio, Rider ou outra IDE compatível
```

Verificar instalação do .NET:

```bash
dotnet --version
```

Verificar instalação do Docker:

```bash
docker --version
```

---

## 3. Banco de dados

O projeto utiliza o banco:

```text
db_IF
```

Durante o desenvolvimento, o SQL Server foi executado em container Docker.

Container usado no ambiente Linux:

```text
sqlserver-if
```

Antes de executar o projeto, conferir se o container está ativo:

```bash
docker ps
```

Se o container existir, mas estiver parado:

```bash
docker start sqlserver-if
```

Para listar todos os containers:

```bash
docker ps -a
```

---

## 4. Preparação inicial do banco

A base do banco foi criada a partir do script fornecido na disciplina.

Além do script principal do banco, existem scripts auxiliares em:

```text
docs/sql/
```

Scripts usados no contexto do Trabalho Final:

```text
docs/sql/seed_parte_3.sql
docs/sql/seed_gerentes_parte_3.sql
docs/sql/consultas_apoio_testes_parte_3.sql
docs/sql/opcional_criar_vinculo_teste_paciente_profissional.sql
docs/sql/opcional_remover_vinculo_teste_paciente_profissional.sql
```

### 4.1. Script `seed_parte_3.sql`

Executar este script para inserir dados mínimos necessários à Parte 3:

```text
- Roles do Identity;
- tipos profissionais;
- planos;
- tipos de acesso.
```

Roles criadas:

```text
Medico
Nutricionista
GerenteMedico
GerenteNutricionista
GerenteGeral
```

### 4.2. Script `seed_gerentes_parte_3.sql`

Este script associa os usuários gerenciais às suas respectivas Roles.

Antes de executá-lo, os usuários gerentes devem existir em `AspNetUsers`.

Usuários gerenciais usados:

```text
gerente.medico@if.com
gerente.nutricionista@if.com
gerente.geral@if.com
```

Associações esperadas:

```text
gerente.medico@if.com         -> GerenteMedico
gerente.nutricionista@if.com  -> GerenteNutricionista
gerente.geral@if.com          -> GerenteGeral
```

### 4.3. Scripts de apoio aos testes

O arquivo abaixo contém consultas de conferência para apoiar os testes funcionais:

```text
docs/sql/consultas_apoio_testes_parte_3.sql
```

Os arquivos abaixo são opcionais e, servem para criar e/ou remover um vínculo de teste entre profissional e paciente:

```text
docs/sql/opcional_criar_vinculo_teste_paciente_profissional.sql
docs/sql/opcional_remover_vinculo_teste_paciente_profissional.sql
```

---

## 5. Configuração da conexão

O projeto precisa apontar para o SQL Server onde está o banco `db_IF`.

A configuração fica no arquivo:

```text
appsettings.json
```

Exemplo de connection string usada no ambiente local:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=db_IF;User Id=app_projeto_if;Password=<SENHA_APP>;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Neste projeto, a chave utilizada é `DefaultConnection`. Ela aponta para o banco `db_IF` no SQL Server local.

A senha real não deve ser publicada no repositório (por esse motivo o uso de `<SENHA_APP>`).

---

## 6. Clonar o projeto

Clonar o repositório:

```bash
git clone https://github.com/gismarb/programacao-web-2-asp-net-core-mvc.git
```

Entrar na pasta do projeto:

```bash
cd programacao-web-2-asp-net-core-mvc/Projeto1_IF
```

---

## 7. Abrir em uma IDE

### 7.1. Visual Studio Code

Na pasta do projeto:

```bash
code .
```

Executar pelo terminal integrado:

```bash
dotnet build
dotnet run
```

### 7.2. Visual Studio ou Rider

Abrir o arquivo de projeto:

```text
Projeto1_IF.csproj
```

ou abrir a pasta/solução do projeto, conforme a IDE utilizada.

Depois, conferir a connection string e executar o projeto pela própria IDE.

---

## 8. Restaurar pacotes e compilar

Na raiz do projeto `Projeto1_IF`:

```bash
dotnet restore
dotnet build
```

Resultado esperado:

```text
Build succeeded.
```

---

## 9. Executar o projeto

Na raiz do projeto:

```bash
dotnet run
```

O terminal exibirá a URL local da aplicação, por exemplo:

```text
https://localhost:<porta>
http://localhost:<porta>
```

Acessar a URL no navegador.

---

## 10. Usuários de teste

### 10.1. Gerentes

```text
E-mail: gerente.medico@if.com
Senha: Gerente@123456
Perfil: GerenteMedico
```

```text
E-mail: gerente.nutricionista@if.com
Senha: Gerente@123456
Perfil: GerenteNutricionista
```

```text
E-mail: gerente.geral@if.com
Senha: Gerente@123456
Perfil: GerenteGeral
```

### 10.2. Profissionais

Os profissionais podem ser cadastrados pela própria aplicação:

```text
/Profissionais/RegistrarMedico
/Profissionais/RegistrarNutricionista
```

Exemplos usados nos testes:

```text
medico2.teste@if.com
nutri.teste@if.com
```

---

## 11. Rotas principais para conferência

### Cadastro de profissional

```text
/Profissionais/RegistrarMedico
/Profissionais/RegistrarNutricionista
```

### Área do profissional

```text
/Profissionais/MeusDados
/Profissionais/EditarMeusDados
/Profissionais/MeusPacientes
```

### Área gerencial

```text
/Profissionais/Gerenciar
```

### Acesso negado

```text
/Identity/Account/AccessDenied
```

---

## 12. Validação inicial após subir o projeto

Depois de executar o projeto, validar:

```text
1. A aplicação abre no navegador.
2. O menu aparece corretamente.
3. Usuário deslogado visualiza opções de cadastro de Médico e Nutricionista.
4. Login de gerente funciona.
5. Login de profissional funciona.
6. A conexão com o banco está funcionando.
```

Para uma validação completa, usar o documento:

[Roteiro de Testes Funcionais](docs/roteiro_testes.md "roteiro_testes.md")

---

## 13. Observações

Este manual descreve apenas a execução e preparação mínima do projeto.

O mapeamento dos requisitos está em:

[Requisitos Funcionais explícitos e implícitos](docs/requisitos_resumidos.md "requisitos_resumidos.md")

O roteiro de validação funcional está em:

[Roteiro de Testes Funcionais](docs/roteiro_testes.md "roteiro_testes.md")

Consultas auxiliares ou de apoio estão em:

[Na pasta `docs/sql` podem ser encontradas mais consultas](docs/sql/ "docs/sql")
