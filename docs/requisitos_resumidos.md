# Requisitos Resumidos — Trabalho Final ASP.NET Core MVC

**Projeto:** Programação Web II — ASP.NET Core MVC  
**Aluno:** Gismar Pereira Barbosa  
**Contexto:** aplicação ASP.NET Core MVC com Identity, Entity Framework Core, SQL Server em Docker e banco `db_IF` fornecido pela disciplina.

---

## 1. Objetivo do documento

Este documento resume os requisitos funcionais, regras de autorização e decisões técnicas implementadas no projeto.

A finalidade é servir como checklist de atendimento ao enunciado do Trabalho Final, relacionando cada requisito com a solução construída no código.

---

## 2. Base técnica utilizada

| Item | Situação no projeto |
|---|---|
| ASP.NET Core MVC | Utilizado como estrutura principal da aplicação |
| ASP.NET Core Identity | Utilizado para autenticação, usuários e Roles |
| Entity Framework Core | Utilizado em abordagem database-first/scaffold |
| SQL Server | Utilizado como banco da aplicação |
| Docker | Utilizado para executar o SQL Server em ambiente Linux |
| `ApplicationDbContext` | Contexto do Identity: `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles` |
| `db_IFContext` | Contexto das tabelas da disciplina: `tbProfissional`, `tbPaciente`, `tbContrato`, etc. |
| Migrations | Não utilizadas, pois a estrutura do banco da disciplina não foi alterada |
| Scripts SQL | Utilizados apenas para seed de dados mínimos e associação de Roles |

---

## 3. Requisitos principais do Trabalho Final

### RF001 — Cadastro do próprio profissional

**Requisito:**  
O sistema deve permitir que o próprio profissional se cadastre.

**Implementação realizada:**  
Foram criadas rotas específicas para cadastro de profissional:

- `/Profissionais/RegistrarMedico`
- `/Profissionais/RegistrarNutricionista`

Essas rotas usam a mesma View `Registrar.cshtml`, mas recebem do Controller o tipo profissional correto.

**Arquivos principais:**

- `Controllers/ProfissionaisController.cs`
- `ViewModels/RegistroProfissionalViewModel.cs`
- `Views/Profissionais/Registrar.cshtml`

**Status:** Atendido.

---

### RF002 — Duas opções de registro: Médico e Nutricionista

**Requisito:**  
O cadastro deve oferecer duas opções: Médico e Nutricionista.

**Implementação realizada:**  
Foram criadas duas actions GET distintas:

- `RegistrarMedico()`
- `RegistrarNutricionista()`

Cada uma define internamente o tipo profissional:

- `IdTipoProfissional = 1` para Médico;
- `IdTipoProfissional = 2` para Nutricionista.

A escolha não fica livre no formulário. O tipo é definido pelo fluxo acessado.

**Status:** Atendido.

---

### RF003 — Uso de Roles diferentes para Médico e Nutricionista

**Requisito:**  
Médico e Nutricionista devem utilizar Roles diferentes.

**Implementação realizada:**  
Foram criadas as Roles:

- `Medico`
- `Nutricionista`

No cadastro, o usuário recebe automaticamente a Role correspondente ao fluxo escolhido.

**Ajuste necessário no projeto:**  
O `Program.cs` foi ajustado com:

```csharp
.AddRoles<IdentityRole>()
```

**Arquivos principais:**

- `Program.cs`
- `Controllers/ProfissionaisController.cs`
- `docs/sql/seed_parte_3.sql`

**Status:** Atendido.

---

### RF004 — Criação de usuário, contrato e profissional no cadastro

**Requisito implícito:**  
Para o cadastro profissional funcionar, não basta criar apenas o usuário do Identity. Também é necessário criar contrato e registro em `tbProfissional`.

**Implementação realizada:**  
O POST `Registrar` cria:

1. usuário em `AspNetUsers`;
2. contrato em `tbContrato`;
3. profissional em `tbProfissional`;
4. vínculo do usuário à Role correta em `AspNetUserRoles`.

**Observação:**  
A senha é criada pelo `UserManager<IdentityUser>`, garantindo o hash correto do ASP.NET Core Identity.

**Status:** Atendido.

---

### RF005 — Profissional só pode visualizar seus próprios dados

**Requisito:**  
Depois de cadastrado, o profissional deve visualizar apenas seus próprios dados.

**Implementação realizada:**  
Foi criada a rota:

- `/Profissionais/MeusDados`

A consulta localiza o profissional pelo `IdUser` do usuário autenticado:

```csharp
p.IdUser == idUsuarioLogado
```

Não é utilizado `IdProfissional` vindo da URL.

**Arquivos principais:**

- `Controllers/ProfissionaisController.cs`
- `Views/Profissionais/MeusDados.cshtml`

**Status:** Atendido.

---

### RF006 — Profissional pode editar seus próprios dados

**Requisito:**  
O profissional deve poder editar seus próprios dados.

**Implementação realizada:**  
Foi criada a rota:

- `/Profissionais/EditarMeusDados`

A edição busca o registro pelo `IdUser` autenticado e atualiza apenas campos permitidos.

**Arquivos principais:**

- `ViewModels/EditarMeusDadosProfissionalViewModel.cs`
- `Controllers/ProfissionaisController.cs`
- `Views/Profissionais/EditarMeusDados.cshtml`

**Status:** Atendido.

---

### RF007 — Profissional não pode alterar CPF

**Requisito:**  
Na edição feita pelo próprio profissional, o CPF não pode ser alterado.

**Implementação realizada:**  
Na tela `EditarMeusDados`, o CPF é exibido como somente leitura. Além disso, o Controller não atualiza o CPF no POST.

**Observação técnica:**  
Mesmo que o usuário manipule o HTML, o Controller não copia o CPF do ViewModel para o model `TbProfissional`.

**Status:** Atendido.

---

### RF008 — Criação de três usuários gerenciais

**Requisito:**  
Devem existir três usuários especiais/gerentes:

- Gerente Médico;
- Gerente Nutricionista;
- Gerente Geral.

**Implementação realizada:**  
Foram criadas as Roles:

- `GerenteMedico`
- `GerenteNutricionista`
- `GerenteGeral`

Os usuários gerenciais foram criados pela tela Register e associados às Roles por script SQL.

**Usuários definidos no ambiente de teste:**

- `gerente.medico@if.com`
- `gerente.nutricionista@if.com`
- `gerente.geral@if.com`

**Arquivos principais:**

- `docs/sql/seed_parte_3.sql`
- `docs/sql/seed_gerentes_parte_3.sql`

**Status:** Atendido.

---

### RF009 — Gerente Médico acessa somente médicos

**Requisito:**  
O Gerente Médico deve acessar somente profissionais médicos.

**Implementação realizada:**  
Na action `Gerenciar`, quando o usuário possui Role `GerenteMedico`, é aplicado filtro:

```csharp
p.IdTipoProfissional == 1
```

A mesma regra é reutilizada nas actions gerenciais de detalhes, edição e exclusão.

**Status:** Atendido.

---

### RF010 — Gerente Nutricionista acessa somente nutricionistas

**Requisito:**  
O Gerente Nutricionista deve acessar somente profissionais nutricionistas.

**Implementação realizada:**  
Na action `Gerenciar`, quando o usuário possui Role `GerenteNutricionista`, é aplicado filtro:

```csharp
p.IdTipoProfissional == 2
```

A mesma regra é validada nas actions gerenciais de detalhes, edição e exclusão.

**Status:** Atendido.

---

### RF011 — Gerente Geral acessa todos os profissionais

**Requisito:**  
O Gerente Geral deve acessar médicos e nutricionistas.

**Implementação realizada:**  
Usuários com Role `GerenteGeral` não recebem filtro por tipo profissional na listagem gerencial.

O método auxiliar `UsuarioGerencialPodeAcessarProfissional` também permite acesso total ao Gerente Geral.

**Status:** Atendido.

---

### RF012 — Gerentes podem visualizar, editar e excluir profissionais

**Requisito:**  
Gerentes podem executar Details, Edit e Delete sobre profissionais conforme seu perfil.

**Implementação realizada:**  
Foram criadas as rotas:

- `/Profissionais/Gerenciar`
- `/Profissionais/GerenciarDetails/{id}`
- `/Profissionais/GerenciarEdit/{id}`
- `/Profissionais/GerenciarDelete/{id}`

Todas aplicam autorização por Role e validação do tipo profissional.

**Status:** Atendido.

---

### RF013 — Gerentes não podem criar profissionais

**Requisito:**  
Gerentes não podem criar profissionais.

**Implementação realizada:**  
Não foi criada action gerencial de Create para profissionais. O cadastro de profissional fica restrito aos fluxos públicos de registro:

- `RegistrarMedico`
- `RegistrarNutricionista`

A área gerencial possui apenas listagem, detalhes, edição e exclusão.

**Status:** Atendido.

---

### RF014 — Gerentes podem alterar CPF

**Requisito:**  
Gerentes podem editar o CPF do profissional.

**Implementação realizada:**  
A ViewModel `EditarProfissionalGerencialViewModel` possui CPF editável. O POST `GerenciarEdit` atualiza o campo `Cpf`.

**Observação:**  
Essa regra é diferente da edição feita pelo próprio profissional, onde o CPF permanece protegido.

**Status:** Atendido.

---

### RF015 — Excluir profissional somente se não houver paciente vinculado

**Requisito:**  
Só deve ser permitida exclusão de profissionais sem pacientes cadastrados.

**Implementação realizada:**  
Antes da exclusão, o sistema verifica a existência de vínculo em `tbMedico_Paciente`:

```csharp
mp.IdProfissional == profissional.IdProfissional
```

Se houver vínculo, a tela mostra bloqueio e o POST também impede a exclusão.

**Status:** Atendido.

---

### RF016 — Profissional deve ter pacientes próprios

**Requisito:**  
Cada profissional deve possuir lista de pacientes cadastrados/vinculados a ele.

**Implementação realizada:**  
Foi criada a rota:

- `/Profissionais/MeusPacientes`

A listagem parte do usuário autenticado, localiza o `IdProfissional` e busca somente vínculos em `tbMedico_Paciente` daquele profissional.

**Status:** Atendido.

---

### RF017 — Profissional pode cadastrar paciente

**Requisito:**  
O profissional deve conseguir cadastrar pacientes.

**Implementação realizada:**  
Foi criada a rota:

- `/Profissionais/CriarMeuPaciente`

O cadastro cria:

1. registro em `tbPaciente`;
2. vínculo em `tbMedico_Paciente` com o profissional logado.

**Arquivos principais:**

- `ViewModels/CriarMeuPacienteViewModel.cs`
- `Controllers/ProfissionaisController.cs`
- `Views/Profissionais/CriarMeuPaciente.cshtml`

**Status:** Atendido.

---

### RF018 — Profissional pode visualizar detalhes de seus pacientes

**Requisito:**  
O profissional deve acessar detalhes de pacientes sob sua responsabilidade.

**Implementação realizada:**  
Foi criada a rota:

- `/Profissionais/MeuPacienteDetails/{id}`

A action só retorna o paciente se existir vínculo entre o paciente solicitado e o profissional logado.

**Status:** Atendido.

---

### RF019 — Profissional pode editar seus pacientes

**Requisito:**  
O profissional deve poder editar pacientes sob sua responsabilidade.

**Implementação realizada:**  
Foi criada a rota:

- `/Profissionais/EditarMeuPaciente/{id}`

A edição altera dados em `tbPaciente` e também a `InformacaoResumida` do vínculo em `tbMedico_Paciente`.

**Status:** Atendido.

---

### RF020 — Profissional pode excluir paciente da sua lista

**Requisito:**  
O profissional deve possuir operação de Delete para seus pacientes.

**Implementação realizada:**  
Foi criada a rota:

- `/Profissionais/ExcluirMeuPaciente/{id}`

A operação remove somente o vínculo em `tbMedico_Paciente` e preserva o cadastro principal em `tbPaciente`.

**Justificativa técnica:**  
Essa decisão evita apagar fisicamente um paciente que possa possuir outros vínculos ou históricos no banco.

**Status:** Atendido como remoção de vínculo do paciente com o profissional logado.

---

### RF021 — Segurança no Controller, não apenas na View

**Requisito explícito do enunciado:**  
A autorização deve ser aplicada no Controller. Esconder links não é suficiente.

**Implementação realizada:**  
Foram usados:

- `[Authorize(Roles = "...")]`;
- filtros LINQ por `IdUser`;
- filtros LINQ por `IdTipoProfissional`;
- validação de vínculo em `tbMedico_Paciente`;
- retorno `Forbid()` em acessos indevidos.

**Status:** Atendido.

---

## 4. Requisitos implícitos (baseados na minha percepção) atendidos

### RI001 — Preparação do banco com dados mínimos

**Necessidade:**  
As tabelas `tbTipoProfissional`, `tbPlano` e `tbTipoAcesso` estavam vazias. Sem esses dados, o cadastro profissional não funcionaria adequadamente.

**Implementação realizada:**  
Foi criado script idempotente:

- `docs/sql/seed_parte_3.sql`

O script cria:

- Roles;
- tipos profissionais;
- planos;
- tipos de acesso.

**Status:** Atendido.

---

### RI002 — Associação dos gerentes às Roles

**Necessidade:**  
Os usuários gerenciais precisam estar vinculados às Roles corretas.

**Implementação realizada:**  
Foi criado script:

- `docs/sql/seed_gerentes_parte_3.sql`

**Status:** Atendido.

---

### RI003 — Separação entre os contextos EF Core

**Necessidade:**  
O projeto possui um contexto para Identity e outro para o banco da disciplina.

**Implementação realizada:**  
O Controller usa:

- `ApplicationDbContext` para dados do Identity;
- `db_IFContext` para tabelas da disciplina.

Na listagem gerencial, as consultas são executadas separadamente para evitar erro de múltiplos contextos na mesma query.

**Status:** Atendido.

---

### RI004 — Navegação por perfil

**Necessidade:**  
A aplicação precisa oferecer links coerentes com o perfil do usuário.

**Implementação realizada:**  
O arquivo `_Layout.cshtml` foi ajustado para exibir links por Role:

- visitante: registrar médico/nutricionista;
- profissional: meus dados/meus pacientes;
- gerente: gerenciar profissionais.

**Status:** Atendido.

---

### RI005 — Mensagens de feedback

**Necessidade:**  
Operações de cadastro, edição e exclusão precisam exibir retorno ao usuário.

**Implementação realizada:**  
Foi criado bloco centralizado em `_Layout.cshtml` para exibir `TempData["MensagemSucesso"]` e `TempData["MensagemErro"]`.

**Status:** Atendido.

---

### RI006 — Página de acesso negado em português

**Necessidade:**  
A mensagem padrão `Access Denied` estava em inglês.

**Implementação realizada:**  
Foi criada página customizada:

- `Areas/Identity/Pages/Account/AccessDenied.cshtml`
- `Areas/Identity/Pages/Account/AccessDenied.cshtml.cs`

**Status:** Atendido.

---

## 5. Extras implementados

| Extra | Descrição | Status |
|---|---|---|
| Planos separados por tipo profissional | Médico visualiza planos iniciados por “Médico”; Nutricionista visualiza planos iniciados por “Nutricionista” | Implementado |
| Página Access Denied em português | Melhora de usabilidade e apresentação | Implementado |
| Navbar por Role | Melhora de navegação | Implementado |
| Mensagens Bootstrap centralizadas | Feedback visual após operações | Implementado |
| Remoção segura de vínculo de paciente | Delete do paciente no contexto do profissional sem apagar `tbPaciente` | Implementado |
| Documentação SQL de seed | Scripts guardados em `docs/sql` | Implementado |

---

## 6. Pontos não implementados por decisão técnica

### Exclusão em cascata opcional de profissional e pacientes

De acordo com o documento da tarefa, o enunciado citava a possibilidade de ponto extra para deleção em cascata do profissional e todos os seus pacientes caso ele seja o único médico.

Essa funcionalidade não foi implementada. A solução adotada foi mais conservadora: bloquear exclusão de profissional com pacientes vinculados, conforme requisito obrigatório.

**Status:** Não implementado, por ser opcional.

---

## 7. Resumo

A implementação atende aos requisitos centrais do Trabalho Final:

- cadastro de médico e nutricionista;
- uso de Roles;
- usuários gerenciais;
- regras de acesso por perfil;
- edição com restrições diferentes para profissional e gerente;
- bloqueio de exclusão de profissional com paciente;
- gestão de pacientes por profissional;
- segurança aplicada no Controller e nas consultas LINQ.
