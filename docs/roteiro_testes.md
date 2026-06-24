# Roteiro de Testes Funcionais — Trabalho Final ASP.NET Core MVC

**Autor:** Gismar Pereira Barbosa  
**Projeto:** Programação Web II — ASP.NET Core MVC + Identity + Entity Framework Core  
**Ambiente base:** Linux/Pop!_OS, .NET 8, SQL Server em Docker, banco `db_IF`

---

## 1. Objetivo

Este documento apresenta um roteiro de testes funcionais para validar se o projeto atende aos requisitos do Trabalho Final de ASP.NET Core MVC.

A validação está organizada por requisito e por ação executada no sistema. O foco é confirmar:

- cadastro de profissionais como Médico ou Nutricionista;
- uso de Roles no ASP.NET Core Identity;
- acesso gerencial por perfil;
- visualização e edição dos próprios dados pelo profissional;
- bloqueio de CPF na edição do próprio profissional;
- permissão gerencial para edição de CPF;
- bloqueio de acesso indevido por URL;
- CRUD de pacientes vinculado ao profissional logado;
- bloqueio de exclusão de profissional com paciente vinculado;
- mensagens, navegação e página de acesso negado.

---

## 2. Pré-condições gerais

Antes de iniciar os testes, confirmar os pontos abaixo.

### 2.1. Docker e SQL Server

O SQL Server deve estar ativo no Docker.

```bash
sudo docker ps
```

Caso o container esteja parado, iniciar o container usado no projeto:

```bash
sudo docker start sqlserver-if
```

O projeto utiliza o banco:

```text
db_IF
```

### 2.2. Aplicação ASP.NET Core

Na raiz do projeto:

```bash
cd /home/gismar/Workspace/DOTNET/programacao-web-2-asp-net-core-mvc/Projeto1_IF
```

Executar build:

```bash
dotnet build
```

Executar aplicação:

```bash
dotnet run
```

### 2.3. Dados mínimos esperados no banco

As Roles devem existir em `AspNetRoles`:

```text
Medico
Nutricionista
GerenteMedico
GerenteNutricionista
GerenteGeral
```

As tabelas de apoio devem estar preenchidas:

```text
tbTipoProfissional
tbPlano
tbTipoAcesso
```

Os usuários gerenciais devem existir e estar vinculados às respectivas Roles:

```text
gerente.medico@if.com         -> GerenteMedico
gerente.nutricionista@if.com  -> GerenteNutricionista
gerente.geral@if.com          -> GerenteGeral
```

Senha usada nos testes:

```text
Gerente@123456
```

---

## 3. Consultas SQL auxiliares

### 3.1. Conferir profissionais cadastrados

```sql
USE db_IF;
GO

SELECT 
    p.IdProfissional,
    p.Nome,
    p.CPF,
    p.IdTipoProfissional,
    p.IdTipoAcesso,
    p.IdContrato,
    p.IdUser,
    u.Email
FROM tbProfissional p
INNER JOIN AspNetUsers u ON u.Id = p.IdUser
ORDER BY p.IdProfissional;
GO
```

### 3.2. Conferir profissionais, Roles e planos

```sql
USE db_IF;
GO

SELECT 
    p.IdProfissional,
    p.Nome,
    p.CPF,
    p.IdTipoProfissional,
    u.Email,
    r.Name AS Role,
    c.IdContrato,
    pl.Nome AS Plano
FROM tbProfissional p
INNER JOIN AspNetUsers u ON u.Id = p.IdUser
LEFT JOIN AspNetUserRoles ur ON ur.UserId = u.Id
LEFT JOIN AspNetRoles r ON r.Id = ur.RoleId
INNER JOIN tbContrato c ON c.IdContrato = p.IdContrato
INNER JOIN tbPlano pl ON pl.IdPlano = c.IdPlano
ORDER BY p.IdProfissional;
GO
```

### 3.3. Conferir vínculos entre profissionais e pacientes

```sql
USE db_IF;
GO

SELECT
    mp.IdMedico_Paciente,
    mp.IdPaciente,
    pac.Nome AS NomePaciente,
    mp.IdProfissional,
    prof.Nome AS NomeProfissional,
    u.Email AS EmailProfissional,
    mp.InformacaoResumida
FROM tbMedico_Paciente mp
INNER JOIN tbPaciente pac ON pac.IdPaciente = mp.IdPaciente
INNER JOIN tbProfissional prof ON prof.IdProfissional = mp.IdProfissional
INNER JOIN AspNetUsers u ON u.Id = prof.IdUser
ORDER BY mp.IdMedico_Paciente;
GO
```

---

## 4. Testes de cadastro de profissional

### TF001 — Cadastro de Médico

**Requisito validado:** o próprio profissional deve conseguir se cadastrar como Médico.

**Usuário sugerido:**

```text
medico.final@if.com
Senha: Medico@123456
```

**Passos:**

1. Acessar `/Profissionais/RegistrarMedico`.
2. Preencher dados de acesso, dados profissionais, plano, cidade e contato.
3. Selecionar um plano de Médico.
4. Enviar o formulário.
5. Verificar redirecionamento para Login.
6. Conferir no banco se usuário, profissional, contrato e Role foram criados.

**Resultado esperado:**

- usuário criado em `AspNetUsers`;
- profissional criado em `tbProfissional`;
- contrato criado em `tbContrato`;
- Role `Medico` associada;
- plano de Médico vinculado;
- sistema redireciona para Login.

---

### TF002 — Cadastro de Nutricionista

**Requisito validado:** o próprio profissional deve conseguir se cadastrar como Nutricionista.

**Usuário sugerido:**

```text
nutri.final@if.com
Senha: Nutri@123456
```

**Passos:**

1. Acessar `/Profissionais/RegistrarNutricionista`.
2. Preencher dados de acesso, dados profissionais, plano, cidade e contato.
3. Selecionar um plano de Nutricionista.
4. Enviar o formulário.
5. Verificar redirecionamento para Login.
6. Conferir no banco se usuário, profissional, contrato e Role foram criados.

**Resultado esperado:**

- usuário criado em `AspNetUsers`;
- profissional criado em `tbProfissional`;
- contrato criado em `tbContrato`;
- Role `Nutricionista` associada;
- plano de Nutricionista vinculado;
- sistema redireciona para Login.

---

## 5. Testes de área do profissional

### TF003 — Visualizar próprios dados

**Requisito validado:** profissional só pode visualizar seus próprios dados.

**Perfil:** Médico ou Nutricionista.

**Passos:**

1. Fazer login como profissional.
2. Acessar `/Profissionais/MeusDados`.
3. Conferir os dados exibidos.

**Resultado esperado:**

- tela abre somente com os dados do usuário logado;
- não existe seleção por `IdProfissional` na URL;
- a consulta é baseada no `IdUser` do usuário autenticado.

---

### TF004 — Editar próprios dados com CPF bloqueado

**Requisito validado:** profissional pode editar seus dados, mas não pode alterar CPF.

**Perfil:** Médico ou Nutricionista.

**Passos:**

1. Fazer login como profissional.
2. Acessar `/Profissionais/EditarMeusDados`.
3. Alterar especialidade, telefone, endereço ou bairro.
4. Verificar se o CPF aparece somente para leitura.
5. Salvar.
6. Conferir retorno para `/Profissionais/MeusDados`.

**Resultado esperado:**

- dados permitidos são atualizados;
- CPF não pode ser editado pela tela;
- CPF não é atualizado no Controller;
- alteração aparece em `MeusDados`.

---

## 6. Testes de área gerencial

### TF005 — Gerente Médico lista somente médicos

**Usuário:**

```text
gerente.medico@if.com
Senha: Gerente@123456
```

**Passos:**

1. Fazer login como Gerente Médico.
2. Acessar `/Profissionais/Gerenciar`.
3. Verificar a listagem.

**Resultado esperado:**

- somente profissionais com `IdTipoProfissional = 1` aparecem;
- nutricionistas não aparecem.

---

### TF006 — Gerente Nutricionista lista somente nutricionistas

**Usuário:**

```text
gerente.nutricionista@if.com
Senha: Gerente@123456
```

**Passos:**

1. Fazer login como Gerente Nutricionista.
2. Acessar `/Profissionais/Gerenciar`.
3. Verificar a listagem.

**Resultado esperado:**

- somente profissionais com `IdTipoProfissional = 2` aparecem;
- médicos não aparecem.

---

### TF007 — Gerente Geral lista todos os profissionais

**Usuário:**

```text
gerente.geral@if.com
Senha: Gerente@123456
```

**Passos:**

1. Fazer login como Gerente Geral.
2. Acessar `/Profissionais/Gerenciar`.
3. Verificar a listagem.

**Resultado esperado:**

- médicos aparecem;
- nutricionistas aparecem;
- gerente consegue abrir detalhes e edição de ambos.

---

### TF008 — Bloqueio de acesso gerencial indevido

**Requisito validado:** gerente não pode acessar profissional fora do seu perfil.

**Passos:**

1. Fazer login como `gerente.medico@if.com`.
2. Tentar acessar manualmente detalhes ou edição de uma nutricionista.
3. Repetir o inverso com `gerente.nutricionista@if.com` tentando acessar médico.

Exemplos:

```text
/Profissionais/GerenciarDetails/{idNutricionista}
/Profissionais/GerenciarEdit/{idNutricionista}
/Profissionais/GerenciarDetails/{idMedico}
/Profissionais/GerenciarEdit/{idMedico}
```

**Resultado esperado:**

- sistema bloqueia com página `Acesso negado`;
- mensagem aparece em português;
- botão de voltar para página inicial funciona.

---

### TF009 — Edição gerencial com alteração de CPF

**Requisito validado:** gerentes podem editar profissionais e alterar CPF.

**Perfil sugerido:** Gerente Geral.

**Passos:**

1. Fazer login como `gerente.geral@if.com`.
2. Acessar `/Profissionais/Gerenciar`.
3. Clicar em Editar em um profissional.
4. Alterar CPF e outro campo simples.
5. Salvar.
6. Conferir detalhes do profissional.

**Resultado esperado:**

- CPF é alterado;
- dados salvos aparecem nos detalhes;
- campos estruturais permanecem preservados: `IdUser`, `IdContrato`, `IdTipoProfissional`, `IdTipoAcesso`.

---

### TF010 — Exclusão gerencial de profissional sem pacientes

**Requisito validado:** gerente pode excluir profissional sem pacientes vinculados.

**Passos:**

1. Fazer login como gerente autorizado.
2. Acessar `/Profissionais/Gerenciar`.
3. Escolher profissional sem vínculo em `tbMedico_Paciente`.
4. Clicar em Excluir.
5. Confirmar exclusão.

**Resultado esperado:**

- tela de confirmação abre;
- botão Confirmar exclusão aparece;
- profissional é removido;
- contrato vinculado é removido;
- profissional deixa de aparecer na listagem.

---

### TF011 — Bloqueio de exclusão gerencial com pacientes vinculados

**Requisito validado:** gerente não pode excluir profissional com paciente vinculado.

**Passos:**

1. Garantir que existe vínculo em `tbMedico_Paciente` para o profissional.
2. Fazer login como gerente autorizado.
3. Acessar `/Profissionais/Gerenciar`.
4. Clicar em Excluir no profissional com vínculo.

**Resultado esperado:**

- tela de confirmação abre;
- mensagem de bloqueio aparece;
- botão Confirmar exclusão não aparece;
- profissional permanece no banco.

---

## 7. Testes de pacientes do profissional

### TF012 — Listar somente pacientes vinculados

**Requisito validado:** profissional só visualiza seus próprios pacientes.

**Passos:**

1. Fazer login como Médico ou Nutricionista.
2. Acessar `/Profissionais/MeusPacientes`.
3. Conferir lista.
4. Fazer login com outro profissional e repetir o teste.

**Resultado esperado:**

- cada profissional vê apenas pacientes vinculados ao seu `IdProfissional`;
- pacientes de outros profissionais não aparecem.

---

### TF013 — Criar paciente e vínculo automático

**Requisito validado:** profissional pode cadastrar paciente e criar vínculo.

**Passos:**

1. Fazer login como profissional.
2. Acessar `/Profissionais/MeusPacientes`.
3. Clicar em `Cadastrar novo paciente`.
4. Preencher dados obrigatórios e opcionais.
5. Marcar/desmarcar Atleta e Gestante.
6. Informar `InformacaoResumida`.
7. Salvar.

**Resultado esperado:**

- registro criado em `tbPaciente`;
- vínculo criado em `tbMedico_Paciente`;
- paciente aparece em `/Profissionais/MeusPacientes`;
- checkboxes funcionam corretamente.

---

### TF014 — Detalhes de paciente vinculado

**Requisito validado:** profissional só abre detalhes de paciente vinculado.

**Passos:**

1. Fazer login como profissional.
2. Acessar `/Profissionais/MeusPacientes`.
3. Clicar em Detalhes de um paciente vinculado.
4. Tentar acessar manualmente um paciente de outro profissional.

Exemplo:

```text
/Profissionais/MeuPacienteDetails/{idPacienteDeOutroProfissional}
```

**Resultado esperado:**

- paciente vinculado abre corretamente;
- paciente não vinculado gera acesso negado.

---

### TF015 — Editar paciente vinculado

**Requisito validado:** profissional edita apenas pacientes vinculados.

**Passos:**

1. Fazer login como profissional.
2. Acessar `/Profissionais/MeusPacientes`.
3. Clicar em Editar.
4. Alterar dados do paciente.
5. Alterar `InformacaoResumida`.
6. Salvar.
7. Tentar editar por URL um paciente de outro profissional.

**Resultado esperado:**

- dados de `tbPaciente` são atualizados;
- `InformacaoResumida` de `tbMedico_Paciente` é atualizada;
- paciente não vinculado gera acesso negado.

---

### TF016 — Remover paciente da lista do profissional

**Requisito validado:** profissional pode remover vínculo com paciente preservando o cadastro principal.

**Passos:**

1. Fazer login como profissional.
2. Acessar `/Profissionais/MeusPacientes`.
3. Clicar em Excluir.
4. Confirmar remoção do vínculo.
5. Conferir listagem.
6. Conferir no banco se o paciente permanece em `tbPaciente`.

**Resultado esperado:**

- vínculo em `tbMedico_Paciente` é removido;
- paciente deixa de aparecer na lista do profissional;
- registro em `tbPaciente` permanece.

---

## 8. Testes de navegação e mensagens

### TF017 — Navbar por perfil

**Passos:**

1. Acessar o sistema sem login.
2. Fazer login como Médico/Nutricionista.
3. Fazer login como gerente.

**Resultado esperado:**

Usuário não logado:

```text
Home
Privacidade
Registrar Médico
Registrar Nutricionista
```

Profissional:

```text
Home
Privacidade
Meus Dados
Meus Pacientes
```

Gerente:

```text
Home
Privacidade
Gerenciar Profissionais
```

---

### TF018 — Acesso negado em português

**Passos:**

1. Fazer login como Médico ou Nutricionista.
2. Acessar `/Profissionais/Gerenciar`.

**Resultado esperado:**

- tela `Acesso negado` aparece em português;
- botão `Voltar para a página inicial` funciona.

---

### TF019 — Mensagens TempData centralizadas

**Passos:**

1. Executar operação que gere mensagem de sucesso.
2. Verificar exibição no topo da página.
3. Fechar o alerta.
4. Garantir que a mensagem não aparece duplicada.

**Resultado esperado:**

- mensagem aparece uma única vez;
- alerta Bootstrap aparece no topo do conteúdo;
- botão fechar funciona.

---

## 9. Resultado geral

| Código | Descrição resumida | Status |
|---|---|---|
| TF001 | Cadastro de Médico | OK |
| TF002 | Cadastro de Nutricionista | OK |
| TF003 | Visualizar próprios dados | OK |
| TF004 | Editar próprios dados com CPF bloqueado | OK |
| TF005 | Gerente Médico lista médicos | OK |
| TF006 | Gerente Nutricionista lista nutricionistas | OK |
| TF007 | Gerente Geral lista todos | OK |
| TF008 | Bloqueio gerencial indevido | OK |
| TF009 | Edição gerencial com CPF | OK |
| TF010 | Exclusão de profissional sem pacientes | OK |
| TF011 | Bloqueio de exclusão com pacientes | OK |
| TF012 | Listar próprios pacientes | OK |
| TF013 | Criar paciente e vínculo | OK |
| TF014 | Detalhes de paciente vinculado | OK |
| TF015 | Editar paciente vinculado | OK |
| TF016 | Remover vínculo do paciente | OK |
| TF017 | Navbar por perfil | OK |
| TF018 | Acesso negado em português | OK |
| TF019 | Mensagens centralizadas | OK |
