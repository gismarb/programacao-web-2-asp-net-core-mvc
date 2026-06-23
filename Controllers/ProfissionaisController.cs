using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projeto1_IF.Data;
using Projeto1_IF.Models;
using Projeto1_IF.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Projeto1_IF.Controllers;

// Gismar Pereira Barbosa
//
// Controller criado para atender ao Trabalho Final de Programação Web II.
//
// Responsabilidade principal:
// controlar o fluxo de cadastro e manutenção de profissionais
// do sistema, separando o comportamento de Médico, Nutricionista
// e usuários gerenciais.
//
// Observação arquitetural:
// Este controller não foi gerado por scaffolding porque o registro
// de profissional não é um CRUD simples de uma única tabela.
// O fluxo envolve ASP.NET Core Identity, Roles, tbContrato,
// tbProfissional, tbPlano, tbCidade e regras de autorização.
public class ProfissionaisController : Controller
{
    private readonly db_IFContext _context;
    private readonly ApplicationDbContext _identityContext;
    private readonly UserManager<IdentityUser> _userManager;

    // Gismar Pereira Barbosa
    //
    // Injeção de dependências usada pelo ASP.NET Core.
    //
    // db_IFContext:
    // acesso às tabelas do banco da disciplina, como tbProfissional,
    // tbContrato, tbPlano e tbCidade.
    //
    // ApplicationDbContext:
    // acesso às tabelas do ASP.NET Core Identity, como AspNetUsers,
    // AspNetRoles e AspNetUserRoles.
    //
    // UserManager<IdentityUser>:
    // serviço do ASP.NET Core Identity usado para criar usuários,
    // consultar usuário logado e associar Roles.
    public ProfissionaisController(
        db_IFContext context,
        ApplicationDbContext identityContext,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _identityContext = identityContext;
        _userManager = userManager;
    }

    // Gismar Pereira Barbosa
    //
    // Action GET para cadastro de Médico.
    //
    // Esta action prepara o formulário de registro já fixando:
    // IdTipoProfissional = 1, equivalente a Médico na tabela tbTipoProfissional.
    //
    // O usuário não escolhe livremente o tipo profissional no formulário.
    // Isso evita que alguém altere o tipo profissional por manipulação da tela.
    [HttpGet]
    public async Task<IActionResult> RegistrarMedico()
    {
        var viewModel = await MontarViewModelRegistroAsync(
            idTipoProfissional: 1,
            tipoProfissionalNome: "Médico");

        return View("Registrar", viewModel);
    }

    // Gismar Pereira Barbosa
    //
    // Action GET para cadastro de Nutricionista.
    //
    // Esta action prepara o formulário de registro já fixando:
    // IdTipoProfissional = 2, equivalente a Nutricionista na tabela tbTipoProfissional.
    //
    // Assim como no fluxo de Médico, o tipo profissional vem da rota/action,
    // e não de uma escolha livre feita manualmente pelo usuário.
    [HttpGet]
    public async Task<IActionResult> RegistrarNutricionista()
    {
        var viewModel = await MontarViewModelRegistroAsync(
            idTipoProfissional: 2,
            tipoProfissionalNome: "Nutricionista");

        return View("Registrar", viewModel);
    }

    // Gismar Pereira Barbosa
    //
    // Action POST responsável pelo registro completo do profissional.
    //
    // Esta action coordena a criação de quatro partes do cadastro:
    //
    // 1. Usuário do ASP.NET Core Identity em AspNetUsers;
    // 2. Contrato do profissional em tbContrato;
    // 3. Dados profissionais em tbProfissional;
    // 4. Associação do usuário à Role Medico ou Nutricionista.
    //
    // Justificativa técnica:
    // Não é usado scaffolding aqui porque o cadastro envolve múltiplas tabelas
    // e regras de negócio que não pertencem a um CRUD simples.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(RegistroProfissionalViewModel viewModel)
    {
        // Gismar Pereira Barbosa
        //
        // Validação defensiva do tipo profissional.
        // Mesmo que a View envie o IdTipoProfissional em campo hidden,
        // não se pode confiar cegamente no valor vindo do navegador.
        if (viewModel.IdTipoProfissional != 1 && viewModel.IdTipoProfissional != 2)
        {
            ModelState.AddModelError(
                string.Empty,
                "Tipo de profissional inválido.");
        }

        var tipoProfissionalNome = viewModel.IdTipoProfissional == 1
            ? "Médico"
            : "Nutricionista";

        var roleProfissional = viewModel.IdTipoProfissional == 1
            ? "Medico"
            : "Nutricionista";

        viewModel.TipoProfissionalNome = tipoProfissionalNome;

        // Gismar Pereira Barbosa
        //
        // Verifica se o plano selecionado pertence ao tipo profissional.
        // Exemplo:
        // - Médico só pode selecionar plano iniciado por "Médico";
        // - Nutricionista só pode selecionar plano iniciado por "Nutricionista".
        //
        // Isso evita manipulação manual do IdPlano no navegador.
        var plano = await _context.TbPlanos
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
                p.IdPlano == viewModel.IdPlano &&
                p.Nome.StartsWith(tipoProfissionalNome));

        if (plano == null)
        {
            ModelState.AddModelError(
                nameof(viewModel.IdPlano),
                "Plano inválido para o tipo profissional selecionado.");
        }

        var cidade = await _context.TbCidades
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdCidade == viewModel.IdCidade);

        if (cidade == null)
        {
            ModelState.AddModelError(
                nameof(viewModel.IdCidade),
                "Cidade inválida.");
        }

        // Gismar Pereira Barbosa
        //
        // Se houver erro de validação, é necessário recarregar os combos
        // de cidades e planos antes de devolver a View.
        //
        // Caso contrário, a View seria renderizada sem os itens dos selects.
        if (!ModelState.IsValid)
        {
            await PreencherCombosRegistroAsync(viewModel);
            return View("Registrar", viewModel);
        }

        // Gismar Pereira Barbosa
        //
        // Criação do usuário Identity.
        // A senha não é gravada manualmente no banco.
        // O UserManager cria o hash correto para o ASP.NET Core Identity.
        var usuario = new IdentityUser
        {
            UserName = viewModel.Email,
            Email = viewModel.Email,
            EmailConfirmed = true
        };

        var resultadoUsuario = await _userManager.CreateAsync(usuario, viewModel.Password);

        if (!resultadoUsuario.Succeeded)
        {
            foreach (var erro in resultadoUsuario.Errors)
            {
                ModelState.AddModelError(string.Empty, erro.Description);
            }

            await PreencherCombosRegistroAsync(viewModel);
            return View("Registrar", viewModel);
        }

        try
        {
            // Gismar Pereira Barbosa
            //
            // Criação do contrato.
            // A DataFim é calculada usando a validade do plano escolhido.
            var contrato = new TbContrato
            {
                IdPlano = viewModel.IdPlano,
                DataInicio = DateTime.Now,
                DataFim = DateTime.Now.AddDays(plano!.Validade)
            };

            _context.TbContratos.Add(contrato);
            await _context.SaveChangesAsync();

            // Gismar Pereira Barbosa
            //
            // Criação do cadastro profissional.
            //
            // IdUser recebe o Id do usuário criado no Identity,
            // formando o vínculo entre login e profissional.
            //
            // IdTipoAcesso = 1 representa o acesso "Profissional",
            // conforme seed inicial executado no banco.
            var profissional = new TbProfissional
            {
                IdTipoProfissional = viewModel.IdTipoProfissional,
                IdContrato = contrato.IdContrato,
                IdTipoAcesso = 1,
                IdCidade = viewModel.IdCidade,
                IdUser = usuario.Id,
                Nome = viewModel.Nome,
                Cpf = viewModel.Cpf,
                CrmCrn = viewModel.CrmCrn,
                Especialidade = viewModel.Especialidade,
                Logradouro = viewModel.Logradouro,
                Numero = viewModel.Numero,
                Bairro = viewModel.Bairro,
                Cep = viewModel.Cep,
                Cidade = cidade!.Nome,
                Ddd1 = viewModel.Ddd1,
                Telefone1 = viewModel.Telefone1,
                Ddd2 = viewModel.Ddd2,
                Telefone2 = viewModel.Telefone2
            };

            _context.TbProfissionals.Add(profissional);
            await _context.SaveChangesAsync();

            // Gismar Pereira Barbosa
            //
            // Associação do usuário à Role correta.
            //
            // Médico recebe Role "Medico".
            // Nutricionista recebe Role "Nutricionista".
            //
            // Essa Role será usada futuramente nos atributos [Authorize]
            // e nas regras de exibição da navegação.
            var resultadoRole = await _userManager.AddToRoleAsync(usuario, roleProfissional);

            if (!resultadoRole.Succeeded)
            {
                foreach (var erro in resultadoRole.Errors)
                {
                    ModelState.AddModelError(string.Empty, erro.Description);
                }

                await PreencherCombosRegistroAsync(viewModel);
                return View("Registrar", viewModel);
            }

            TempData["MensagemSucesso"] =
                $"Cadastro de {tipoProfissionalNome} realizado com sucesso. Faça login para acessar o sistema.";

            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        catch (DbUpdateException)
        {
            // Gismar Pereira Barbosa
            //
            // Tratamento de erro de gravação no banco.
            //
            // Como o usuário Identity já pode ter sido criado antes do erro
            // nas tabelas de domínio, foi removido esse usuário para evitar
            // cadastro incompleto.
            await _userManager.DeleteAsync(usuario);

            ModelState.AddModelError(
                string.Empty,
                "Não foi possível concluir o cadastro profissional. Verifique os dados informados e tente novamente.");

            await PreencherCombosRegistroAsync(viewModel);
            return View("Registrar", viewModel);
        }
    }

    // Gismar Pereira Barbosa
    //
    // Action responsável por exibir os dados do profissional logado.
    //
    // Regra de segurança aplicada:
    // Somente usuários autenticados com Role Medico ou Nutricionista
    // podem acessar esta tela.
    //
    // Além disso, a consulta LINQ filtra o cadastro pelo IdUser do usuário
    // logado. Isso garante que um profissional não consiga visualizar dados
    // de outro profissional apenas alterando parâmetros na URL.
    [Authorize(Roles = "Medico,Nutricionista")]
    [HttpGet]
    public async Task<IActionResult> MeusDados()
    {
        var idUsuarioLogado = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(idUsuarioLogado))
        {
            return Challenge();
        }

        var profissional = await _context.TbProfissionals
            .AsNoTracking()
            .Include(p => p.IdCidadeNavigation)
            .Include(p => p.IdContratoNavigation)
                .ThenInclude(c => c.IdPlanoNavigation)
            .FirstOrDefaultAsync(p => p.IdUser == idUsuarioLogado);

        if (profissional == null)
        {
            return NotFound("Cadastro profissional não encontrado para o usuário logado.");
        }

        return View(profissional);
    }

        // Gismar Pereira Barbosa
    //
    // Action GET responsável por carregar a tela de edição dos dados
    // do próprio profissional logado.
    //
    // Regra de segurança:
    // A busca é feita pelo IdUser do usuário autenticado no Identity,
    // e não por IdProfissional recebido pela URL.
    //
    // Isso impede que um profissional tente editar dados de outro
    // profissional manipulando parâmetros de rota.
    [Authorize(Roles = "Medico,Nutricionista")]
    [HttpGet]
    public async Task<IActionResult> EditarMeusDados()
    {
        var idUsuarioLogado = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(idUsuarioLogado))
        {
            return Challenge();
        }

        var profissional = await _context.TbProfissionals
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdUser == idUsuarioLogado);

        if (profissional == null)
        {
            return NotFound("Cadastro profissional não encontrado para o usuário logado.");
        }

        var viewModel = new EditarMeusDadosProfissionalViewModel
        {
            IdProfissional = profissional.IdProfissional,
            Cpf = profissional.Cpf,
            Nome = profissional.Nome,
            CrmCrn = profissional.CrmCrn,
            Especialidade = profissional.Especialidade,
            Logradouro = profissional.Logradouro,
            Numero = profissional.Numero,
            Bairro = profissional.Bairro,
            Cep = profissional.Cep,
            IdCidade = profissional.IdCidade,
            Ddd1 = profissional.Ddd1,
            Telefone1 = profissional.Telefone1,
            Ddd2 = profissional.Ddd2,
            Telefone2 = profissional.Telefone2
        };

        await PreencherCidadesEdicaoAsync(viewModel);

        return View(viewModel);
    }

    // Gismar Pereira Barbosa
    //
    // Action POST responsável por salvar a edição dos dados do próprio
    // profissional logado.
    //
    // Regra de segurança:
    // Mesmo que o formulário envie IdProfissional, o Controller busca o
    // registro pelo IdUser do usuário autenticado.
    //
    // Assim, o usuário não consegue editar outro profissional alterando
    // o IdProfissional no HTML da página.
    //
    // Regra de negócio:
    // O CPF não é atualizado nesta action. Ele aparece na tela apenas
    // para consulta, atendendo ao requisito de que o profissional não
    // pode alterar CPF após o cadastro.
    [Authorize(Roles = "Medico,Nutricionista")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarMeusDados(
        EditarMeusDadosProfissionalViewModel viewModel)
    {
        var idUsuarioLogado = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(idUsuarioLogado))
        {
            return Challenge();
        }

        var cidade = await _context.TbCidades
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdCidade == viewModel.IdCidade);

        if (cidade == null)
        {
            ModelState.AddModelError(
                nameof(viewModel.IdCidade),
                "Cidade inválida.");
        }

        if (!ModelState.IsValid)
        {
            await PreencherCidadesEdicaoAsync(viewModel);
            return View(viewModel);
        }

        var profissional = await _context.TbProfissionals
            .FirstOrDefaultAsync(p => p.IdUser == idUsuarioLogado);

        if (profissional == null)
        {
            return NotFound("Cadastro profissional não encontrado para o usuário logado.");
        }

        // Gismar Pereira Barbosa
        //
        // Atualização controlada dos campos permitidos.
        //
        // Não atualiza:
        // - Cpf
        // - IdUser
        // - IdContrato
        // - IdTipoProfissional
        // - IdTipoAcesso
        //
        // Isso evita alteração indevida de identidade, vínculo,
        // contrato, papel profissional ou CPF.
        profissional.Nome = viewModel.Nome;
        profissional.CrmCrn = viewModel.CrmCrn;
        profissional.Especialidade = viewModel.Especialidade;
        profissional.Logradouro = viewModel.Logradouro;
        profissional.Numero = viewModel.Numero;
        profissional.Bairro = viewModel.Bairro;
        profissional.Cep = viewModel.Cep;
        profissional.IdCidade = viewModel.IdCidade;
        profissional.Cidade = cidade!.Nome;
        profissional.Ddd1 = viewModel.Ddd1;
        profissional.Telefone1 = viewModel.Telefone1;
        profissional.Ddd2 = viewModel.Ddd2;
        profissional.Telefone2 = viewModel.Telefone2;

        try
        {
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] =
                "Seus dados profissionais foram atualizados com sucesso.";

            return RedirectToAction(nameof(MeusDados));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Não foi possível salvar as alterações. Verifique os dados informados e tente novamente.");

            await PreencherCidadesEdicaoAsync(viewModel);
            return View(viewModel);
        }
    }

    // Gismar Pereira Barbosa
    //
    // Action responsável pela listagem gerencial de profissionais.
    //
    // Regra de autorização:
    // Somente usuários com perfil gerencial podem acessar esta tela.
    //
    // Regras de filtro aplicadas no Controller:
    // - GerenteMedico visualiza apenas profissionais do tipo Médico;
    // - GerenteNutricionista visualiza apenas profissionais do tipo Nutricionista;
    // - GerenteGeral visualiza todos os profissionais.
    //
    // A filtragem é feita na consulta LINQ antes dos dados chegarem à View.
    // A segurança não depende apenas de esconder links ou botões.
    [Authorize(Roles = "GerenteMedico,GerenteNutricionista,GerenteGeral")]
    [HttpGet]
    public async Task<IActionResult> Gerenciar()
    {
        var query = _context.TbProfissionals
            .AsNoTracking()
            .Include(p => p.IdContratoNavigation)
                .ThenInclude(c => c.IdPlanoNavigation)
            .AsQueryable();

        if (User.IsInRole("GerenteMedico"))
        {
            query = query.Where(p => p.IdTipoProfissional == 1);
            ViewData["TituloGerencial"] = "Gerenciamento de Médicos";
        }
        else if (User.IsInRole("GerenteNutricionista"))
        {
            query = query.Where(p => p.IdTipoProfissional == 2);
            ViewData["TituloGerencial"] = "Gerenciamento de Nutricionistas";
        }
        else
        {
            ViewData["TituloGerencial"] = "Gerenciamento de Profissionais";
        }

        // Gismar Pereira Barbosa
        //
        // Primeira consulta:
        // carrega os profissionais, contratos e planos usando db_IFContext.
        //
        // Esta consulta não acessa tabelas do Identity para evitar misturar
        // dois contextos diferentes dentro da mesma execução LINQ.
        var profissionaisBanco = await query
            .OrderBy(p => p.Nome)
            .ToListAsync();

        var idsUsuarios = profissionaisBanco
            .Select(p => p.IdUser)
            .Distinct()
            .ToList();

        // Gismar Pereira Barbosa
        //
        // Segunda consulta:
        // carrega os usuários vinculados usando ApplicationDbContext,
        // que é o contexto responsável pelas tabelas do ASP.NET Core Identity.
        //
        // O resultado é convertido para Dictionary para facilitar a busca
        // do e-mail pelo IdUser durante a montagem do ViewModel.
        var emailsUsuarios = await _identityContext.Users
            .AsNoTracking()
            .Where(u => idsUsuarios.Contains(u.Id))
            .Select(u => new
            {
                u.Id,
                Email = u.Email ?? string.Empty
            })
            .ToDictionaryAsync(u => u.Id, u => u.Email);

        // Gismar Pereira Barbosa
        //
        // Montagem do ViewModel em memória.
        //
        // Neste ponto, as duas consultas ao banco já foram executadas
        // separadamente. Por isso, é seguro combinar os dados de profissional
        // e usuário sem gerar erro de múltiplos contextos no Entity Framework.
        var profissionais = profissionaisBanco
            .Select(p => new ProfissionalGerencialViewModel
            {
                IdProfissional = p.IdProfissional,
                Nome = p.Nome,
                Cpf = p.Cpf,
                IdTipoProfissional = p.IdTipoProfissional,
                TipoProfissional = p.IdTipoProfissional == 1
                    ? "Médico"
                    : p.IdTipoProfissional == 2
                        ? "Nutricionista"
                        : "Não identificado",
                CrmCrn = p.CrmCrn,
                Especialidade = p.Especialidade,
                Cidade = p.Cidade,
                EmailUsuario = emailsUsuarios.ContainsKey(p.IdUser)
                    ? emailsUsuarios[p.IdUser]
                    : string.Empty,
                Plano = p.IdContratoNavigation?.IdPlanoNavigation?.Nome ?? string.Empty
            })
            .ToList();

        return View(profissionais);
    }

    // Gismar Pereira Barbosa
    //
    // Action responsável por exibir os detalhes de um profissional
    // na área gerencial.
    //
    // A action recebe IdProfissional pela rota, mas aplica validação
    // de autorização no Controller antes de retornar os dados para a View.
    //
    // Regras:
    // - GerenteMedico só pode visualizar profissionais médicos;
    // - GerenteNutricionista só pode visualizar profissionais nutricionistas;
    // - GerenteGeral pode visualizar qualquer profissional.
    [Authorize(Roles = "GerenteMedico,GerenteNutricionista,GerenteGeral")]
    [HttpGet]
    public async Task<IActionResult> GerenciarDetails(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var profissional = await _context.TbProfissionals
            .AsNoTracking()
            .Include(p => p.IdCidadeNavigation)
            .Include(p => p.IdContratoNavigation)
                .ThenInclude(c => c.IdPlanoNavigation)
            .FirstOrDefaultAsync(p => p.IdProfissional == id);

        if (profissional == null)
        {
            return NotFound();
        }

        if (!UsuarioGerencialPodeAcessarProfissional(profissional.IdTipoProfissional))
        {
            return Forbid();
        }

        return View(profissional);
    }

    // Gismar Pereira Barbosa
    //
    // Action GET responsável por carregar a tela de edição gerencial
    // de um profissional.
    //
    // A action recebe o IdProfissional pela rota, mas antes de exibir
    // os dados aplica a validação de autorização gerencial.
    //
    // Regras:
    // - GerenteMedico só pode editar profissionais médicos;
    // - GerenteNutricionista só pode editar profissionais nutricionistas;
    // - GerenteGeral pode editar qualquer profissional.
    [Authorize(Roles = "GerenteMedico,GerenteNutricionista,GerenteGeral")]
    [HttpGet]
    public async Task<IActionResult> GerenciarEdit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var profissional = await _context.TbProfissionals
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdProfissional == id);

        if (profissional == null)
        {
            return NotFound();
        }

        if (!UsuarioGerencialPodeAcessarProfissional(profissional.IdTipoProfissional))
        {
            return Forbid();
        }

        var viewModel = new EditarProfissionalGerencialViewModel
        {
            IdProfissional = profissional.IdProfissional,
            IdTipoProfissional = profissional.IdTipoProfissional,
            TipoProfissionalNome = ObterNomeTipoProfissional(profissional.IdTipoProfissional),
            Nome = profissional.Nome,
            Cpf = profissional.Cpf,
            CrmCrn = profissional.CrmCrn,
            Especialidade = profissional.Especialidade,
            Logradouro = profissional.Logradouro,
            Numero = profissional.Numero,
            Bairro = profissional.Bairro,
            Cep = profissional.Cep,
            IdCidade = profissional.IdCidade,
            Ddd1 = profissional.Ddd1,
            Telefone1 = profissional.Telefone1,
            Ddd2 = profissional.Ddd2,
            Telefone2 = profissional.Telefone2
        };

        await PreencherCidadesGerencialAsync(viewModel);

        return View(viewModel);
    }

    // Gismar Pereira Barbosa
    //
    // Action POST responsável por salvar a edição gerencial de um profissional.
    //
    // Regra de segurança:
    // O Controller recarrega o profissional pelo IdProfissional e valida
    // se o gerente logado pode acessar aquele tipo de profissional.
    //
    // Regra de negócio:
    // Diferente da edição feita pelo próprio profissional, nesta edição
    // gerencial o CPF pode ser alterado.
    //
    // Campos estruturais não são alterados:
    // - IdUser;
    // - IdContrato;
    // - IdTipoProfissional;
    // - IdTipoAcesso.
    [Authorize(Roles = "GerenteMedico,GerenteNutricionista,GerenteGeral")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GerenciarEdit(
        int id,
        EditarProfissionalGerencialViewModel viewModel)
    {
        if (id != viewModel.IdProfissional)
        {
            return NotFound();
        }

        var profissional = await _context.TbProfissionals
            .FirstOrDefaultAsync(p => p.IdProfissional == id);

        if (profissional == null)
        {
            return NotFound();
        }

        if (!UsuarioGerencialPodeAcessarProfissional(profissional.IdTipoProfissional))
        {
            return Forbid();
        }

        var cidade = await _context.TbCidades
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdCidade == viewModel.IdCidade);

        if (cidade == null)
        {
            ModelState.AddModelError(
                nameof(viewModel.IdCidade),
                "Cidade inválida.");
        }

        if (!ModelState.IsValid)
        {
            viewModel.IdTipoProfissional = profissional.IdTipoProfissional;
            viewModel.TipoProfissionalNome = ObterNomeTipoProfissional(profissional.IdTipoProfissional);

            await PreencherCidadesGerencialAsync(viewModel);
            return View(viewModel);
        }

        // Gismar Pereira Barbosa
        //
        // Atualização controlada dos campos permitidos na edição gerencial.
        //
        // Nesta tela, o CPF pode ser alterado pelo gerente.
        // Os campos de vínculo e estrutura permanecem preservados.
        profissional.Nome = viewModel.Nome;
        profissional.Cpf = viewModel.Cpf;
        profissional.CrmCrn = viewModel.CrmCrn;
        profissional.Especialidade = viewModel.Especialidade;
        profissional.Logradouro = viewModel.Logradouro;
        profissional.Numero = viewModel.Numero;
        profissional.Bairro = viewModel.Bairro;
        profissional.Cep = viewModel.Cep;
        profissional.IdCidade = viewModel.IdCidade;
        profissional.Cidade = cidade!.Nome;
        profissional.Ddd1 = viewModel.Ddd1;
        profissional.Telefone1 = viewModel.Telefone1;
        profissional.Ddd2 = viewModel.Ddd2;
        profissional.Telefone2 = viewModel.Telefone2;

        try
        {
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] =
                "Cadastro profissional atualizado com sucesso.";

            return RedirectToAction(nameof(GerenciarDetails), new { id = profissional.IdProfissional });
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Não foi possível salvar as alterações. Verifique os dados informados e tente novamente.");

            viewModel.IdTipoProfissional = profissional.IdTipoProfissional;
            viewModel.TipoProfissionalNome = ObterNomeTipoProfissional(profissional.IdTipoProfissional);

            await PreencherCidadesGerencialAsync(viewModel);
            return View(viewModel);
        }
    }

    // Gismar Pereira Barbosa
    //
    // Action GET responsável por carregar a tela de confirmação
    // de exclusão gerencial de um profissional.
    //
    // A action valida se o gerente logado pode acessar o tipo profissional
    // selecionado e também verifica se existem pacientes vinculados.
    //
    // Regra de negócio:
    // O profissional só pode ser excluído se não possuir pacientes
    // cadastrados em tbMedico_Paciente.
    [Authorize(Roles = "GerenteMedico,GerenteNutricionista,GerenteGeral")]
    [HttpGet]
    public async Task<IActionResult> GerenciarDelete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var profissional = await _context.TbProfissionals
            .AsNoTracking()
            .Include(p => p.IdCidadeNavigation)
            .Include(p => p.IdContratoNavigation)
                .ThenInclude(c => c.IdPlanoNavigation)
            .FirstOrDefaultAsync(p => p.IdProfissional == id);

        if (profissional == null)
        {
            return NotFound();
        }

        if (!UsuarioGerencialPodeAcessarProfissional(profissional.IdTipoProfissional))
        {
            return Forbid();
        }

        var possuiPacientes = await _context.TbMedicoPacientes
            .AsNoTracking()
            .AnyAsync(mp => mp.IdProfissional == profissional.IdProfissional);

        ViewData["PossuiPacientes"] = possuiPacientes;

        if (possuiPacientes)
        {
            ViewData["MensagemBloqueio"] =
                "Este profissional possui pacientes vinculados e não pode ser excluído.";
        }

        return View(profissional);
    }

    // Gismar Pereira Barbosa
    //
    // Action POST responsável por executar a exclusão gerencial
    // de um profissional.
    //
    // A validação de pacientes vinculados é repetida no POST.
    // Isso é necessário porque a regra de segurança não pode depender
    // apenas da tela de confirmação exibida no GET.
    //
    // Se houver qualquer registro em tbMedico_Paciente para o profissional,
    // a exclusão é bloqueada.
    [Authorize(Roles = "GerenteMedico,GerenteNutricionista,GerenteGeral")]
    [HttpPost, ActionName("GerenciarDelete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GerenciarDeleteConfirmed(int id)
    {
        var profissional = await _context.TbProfissionals
            .Include(p => p.IdContratoNavigation)
            .FirstOrDefaultAsync(p => p.IdProfissional == id);

        if (profissional == null)
        {
            return NotFound();
        }

        if (!UsuarioGerencialPodeAcessarProfissional(profissional.IdTipoProfissional))
        {
            return Forbid();
        }

        var possuiPacientes = await _context.TbMedicoPacientes
            .AsNoTracking()
            .AnyAsync(mp => mp.IdProfissional == profissional.IdProfissional);

        if (possuiPacientes)
        {
            TempData["MensagemErro"] =
                "Não foi possível excluir o profissional, pois existem pacientes vinculados ao cadastro.";

            return RedirectToAction(nameof(GerenciarDelete), new { id = profissional.IdProfissional });
        }

        try
        {
            var contrato = profissional.IdContratoNavigation;

            _context.TbProfissionals.Remove(profissional);

            if (contrato != null)
            {
                _context.TbContratos.Remove(contrato);
            }

            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] =
                "Profissional excluído com sucesso.";

            return RedirectToAction(nameof(Gerenciar));
        }
        catch (DbUpdateException)
        {
            TempData["MensagemErro"] =
                "Não foi possível excluir o profissional. Verifique se existem vínculos no banco de dados.";

            return RedirectToAction(nameof(GerenciarDelete), new { id = profissional.IdProfissional });
        }
    }

    // Gismar Pereira Barbosa
    //
    // Action responsável por listar os pacientes vinculados ao profissional logado.
    //
    // Regra de segurança:
    // A consulta parte do IdUser do usuário autenticado no ASP.NET Core Identity.
    // A partir dele, é localizado o IdProfissional correspondente em tbProfissional.
    //
    // Somente depois disso são consultados os vínculos existentes em
    // tbMedico_Paciente. Dessa forma, a tela não depende de parâmetros de URL
    // para decidir quais pacientes serão exibidos.
    [Authorize(Roles = "Medico,Nutricionista")]
    [HttpGet]
    public async Task<IActionResult> MeusPacientes()
    {
        var idUsuarioLogado = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(idUsuarioLogado))
        {
            return Challenge();
        }

        var profissional = await _context.TbProfissionals
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdUser == idUsuarioLogado);

        if (profissional == null)
        {
            return NotFound("Cadastro profissional não encontrado para o usuário logado.");
        }

        var pacientes = await _context.TbMedicoPacientes
            .AsNoTracking()
            .Include(mp => mp.IdPacienteNavigation)
                .ThenInclude(p => p.IdCidadeNavigation)
            .Where(mp => mp.IdProfissional == profissional.IdProfissional)
            .OrderBy(mp => mp.IdPacienteNavigation.Nome)
            .Select(mp => new PacienteProfissionalViewModel
            {
                IdPaciente = mp.IdPaciente,
                IdMedicoPaciente = mp.IdMedicoPaciente,
                Nome = mp.IdPacienteNavigation.Nome,
                Cpf = mp.IdPacienteNavigation.Cpf,
                Rg = mp.IdPacienteNavigation.Rg,
                DataNascimento = mp.IdPacienteNavigation.DataNascimento,
                Sexo = mp.IdPacienteNavigation.Sexo,
                Cidade = mp.IdPacienteNavigation.IdCidadeNavigation != null
                    ? mp.IdPacienteNavigation.IdCidadeNavigation.Nome
                    : string.Empty,
                TelCelular = mp.IdPacienteNavigation.TelCelular,
                InformacaoResumida = mp.InformacaoResumida
            })
            .ToListAsync();

        return View(pacientes);
    }

    // Gismar Pereira Barbosa
    //
    // Action responsável por exibir os detalhes de um paciente
    // vinculado ao profissional logado.
    //
    // Regra de segurança:
    // A action recebe o IdPaciente pela rota, mas não consulta o paciente
    // diretamente em TbPacientes.
    //
    // Primeiro é localizado o profissional correspondente ao usuário logado.
    // Depois é verificado se existe vínculo em tbMedico_Paciente entre
    // esse profissional e o paciente solicitado.
    //
    // Isso impede que um Médico ou Nutricionista acesse detalhes de pacientes
    // de outro profissional apenas alterando o id na URL.
    [Authorize(Roles = "Medico,Nutricionista")]
    [HttpGet]
    public async Task<IActionResult> MeuPacienteDetails(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var idUsuarioLogado = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(idUsuarioLogado))
        {
            return Challenge();
        }

        var profissional = await _context.TbProfissionals
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdUser == idUsuarioLogado);

        if (profissional == null)
        {
            return NotFound("Cadastro profissional não encontrado para o usuário logado.");
        }

        var vinculoPaciente = await _context.TbMedicoPacientes
            .AsNoTracking()
            .Include(mp => mp.IdPacienteNavigation)
                .ThenInclude(p => p.IdCidadeNavigation)
            .FirstOrDefaultAsync(mp =>
                mp.IdProfissional == profissional.IdProfissional &&
                mp.IdPaciente == id.Value);

        if (vinculoPaciente == null)
        {
            return Forbid();
        }

        return View(vinculoPaciente);
    }

    // Gismar Pereira Barbosa
    //
    // Action GET responsável por carregar a tela de cadastro
    // de paciente pelo profissional logado.
    //
    // A tela permite cadastrar os dados básicos do paciente e também
    // a informação resumida do vínculo em tbMedico_Paciente.
    [Authorize(Roles = "Medico,Nutricionista")]
    [HttpGet]
    public async Task<IActionResult> CriarMeuPaciente()
    {
        var viewModel = new CriarMeuPacienteViewModel
        {
            DataNascimento = DateOnly.FromDateTime(DateTime.Today)
        };

        await PreencherCidadesPacienteAsync(viewModel);

        return View(viewModel);
    }

    // Gismar Pereira Barbosa
    //
    // Action POST responsável por cadastrar um novo paciente
    // para o profissional logado.
    //
    // Regra de segurança:
    // O IdProfissional não vem da View nem da URL.
    // Ele é obtido a partir do IdUser do usuário autenticado.
    //
    // Fluxo de gravação:
    // 1. Localiza o profissional logado;
    // 2. Cria o registro em tbPaciente;
    // 3. Cria o vínculo em tbMedico_Paciente;
    // 4. Redireciona para a listagem MeusPacientes.
    [Authorize(Roles = "Medico,Nutricionista")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarMeuPaciente(
        CriarMeuPacienteViewModel viewModel)
    {
        var idUsuarioLogado = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(idUsuarioLogado))
        {
            return Challenge();
        }

        var profissional = await _context.TbProfissionals
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdUser == idUsuarioLogado);

        if (profissional == null)
        {
            return NotFound("Cadastro profissional não encontrado para o usuário logado.");
        }

        if (viewModel.IdCidade.HasValue)
        {
            var cidadeExiste = await _context.TbCidades
                .AsNoTracking()
                .AnyAsync(c => c.IdCidade == viewModel.IdCidade.Value);

            if (!cidadeExiste)
            {
                ModelState.AddModelError(
                    nameof(viewModel.IdCidade),
                    "Cidade inválida.");
            }
        }

        if (!ModelState.IsValid)
        {
            await PreencherCidadesPacienteAsync(viewModel);
            return View(viewModel);
        }

        var paciente = new TbPaciente
        {
            Nome = viewModel.Nome,
            Rg = viewModel.Rg,
            Cpf = viewModel.Cpf,
            DataNascimento = viewModel.DataNascimento,
            NomeResponsavel = viewModel.NomeResponsavel,
            Sexo = viewModel.Sexo,
            Etnia = viewModel.Etnia,
            Endereco = viewModel.Endereco,
            Bairro = viewModel.Bairro,
            IdCidade = viewModel.IdCidade,
            TelResidencial = viewModel.TelResidencial,
            TelComercial = viewModel.TelComercial,
            TelCelular = viewModel.TelCelular,
            Profissao = viewModel.Profissao,
            FlgAtleta = viewModel.FlgAtleta,
            FlgGestante = viewModel.FlgGestante
        };

        var vinculo = new TbMedicoPaciente
        {
            IdProfissional = profissional.IdProfissional,
            IdPacienteNavigation = paciente,
            InformacaoResumida = viewModel.InformacaoResumida
        };

        try
        {
            _context.TbPacientes.Add(paciente);
            _context.TbMedicoPacientes.Add(vinculo);

            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] =
                "Paciente cadastrado e vinculado ao profissional com sucesso.";

            return RedirectToAction(nameof(MeusPacientes));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Não foi possível cadastrar o paciente. Verifique os dados informados e tente novamente.");

            await PreencherCidadesPacienteAsync(viewModel);
            return View(viewModel);
        }
    }

    // Gismar Pereira Barbosa
    //
    // Action GET responsável por carregar a tela de edição
    // de paciente vinculado ao profissional logado.
    //
    // Regra de segurança:
    // A action recebe o IdPaciente pela rota, mas só permite a edição
    // se existir vínculo em tbMedico_Paciente entre o paciente solicitado
    // e o profissional autenticado.
    [Authorize(Roles = "Medico,Nutricionista")]
    [HttpGet]
    public async Task<IActionResult> EditarMeuPaciente(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var idUsuarioLogado = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(idUsuarioLogado))
        {
            return Challenge();
        }

        var profissional = await _context.TbProfissionals
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdUser == idUsuarioLogado);

        if (profissional == null)
        {
            return NotFound("Cadastro profissional não encontrado para o usuário logado.");
        }

        var vinculoPaciente = await _context.TbMedicoPacientes
            .AsNoTracking()
            .Include(mp => mp.IdPacienteNavigation)
            .FirstOrDefaultAsync(mp =>
                mp.IdProfissional == profissional.IdProfissional &&
                mp.IdPaciente == id.Value);

        if (vinculoPaciente == null)
        {
            return Forbid();
        }

        var paciente = vinculoPaciente.IdPacienteNavigation;

        var viewModel = new EditarMeuPacienteViewModel
        {
            IdPaciente = paciente.IdPaciente,
            IdMedicoPaciente = vinculoPaciente.IdMedicoPaciente,
            Nome = paciente.Nome,
            Rg = paciente.Rg,
            Cpf = paciente.Cpf,
            DataNascimento = paciente.DataNascimento,
            Sexo = paciente.Sexo,
            Etnia = paciente.Etnia,
            NomeResponsavel = paciente.NomeResponsavel,
            Endereco = paciente.Endereco,
            Bairro = paciente.Bairro,
            IdCidade = paciente.IdCidade,
            TelResidencial = paciente.TelResidencial,
            TelComercial = paciente.TelComercial,
            TelCelular = paciente.TelCelular,
            Profissao = paciente.Profissao,
            FlgAtleta = paciente.FlgAtleta ?? false,
            FlgGestante = paciente.FlgGestante ?? false,
            InformacaoResumida = vinculoPaciente.InformacaoResumida
        };

        await PreencherCidadesPacienteAsync(viewModel);

        return View(viewModel);
    }

    // Gismar Pereira Barbosa
    //
    // Action POST responsável por salvar a edição de paciente
    // feita pelo profissional logado.
    //
    // Regra de segurança:
    // O Controller recarrega o vínculo em tbMedico_Paciente usando:
    // - IdProfissional do usuário autenticado;
    // - IdPaciente recebido na rota/formulário.
    //
    // Dessa forma, mesmo que alguém altere o HTML da página, a edição
    // só acontece se o paciente realmente estiver vinculado ao profissional.
    [Authorize(Roles = "Medico,Nutricionista")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarMeuPaciente(
        int id,
        EditarMeuPacienteViewModel viewModel)
    {
        if (id != viewModel.IdPaciente)
        {
            return NotFound();
        }

        var idUsuarioLogado = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(idUsuarioLogado))
        {
            return Challenge();
        }

        var profissional = await _context.TbProfissionals
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdUser == idUsuarioLogado);

        if (profissional == null)
        {
            return NotFound("Cadastro profissional não encontrado para o usuário logado.");
        }

        var vinculoPaciente = await _context.TbMedicoPacientes
            .Include(mp => mp.IdPacienteNavigation)
            .FirstOrDefaultAsync(mp =>
                mp.IdProfissional == profissional.IdProfissional &&
                mp.IdPaciente == id);

        if (vinculoPaciente == null)
        {
            return Forbid();
        }

        if (viewModel.IdCidade.HasValue)
        {
            var cidadeExiste = await _context.TbCidades
                .AsNoTracking()
                .AnyAsync(c => c.IdCidade == viewModel.IdCidade.Value);

            if (!cidadeExiste)
            {
                ModelState.AddModelError(
                    nameof(viewModel.IdCidade),
                    "Cidade inválida.");
            }
        }

        if (!ModelState.IsValid)
        {
            await PreencherCidadesPacienteAsync(viewModel);
            return View(viewModel);
        }

        var paciente = vinculoPaciente.IdPacienteNavigation;

        // Gismar Pereira Barbosa
        //
        // Atualização controlada dos dados do paciente.
        //
        // A edição só chega neste ponto após validar o vínculo entre
        // profissional logado e paciente em tbMedico_Paciente.
        paciente.Nome = viewModel.Nome;
        paciente.Rg = viewModel.Rg;
        paciente.Cpf = viewModel.Cpf;
        paciente.DataNascimento = viewModel.DataNascimento;
        paciente.Sexo = viewModel.Sexo;
        paciente.Etnia = viewModel.Etnia;
        paciente.NomeResponsavel = viewModel.NomeResponsavel;
        paciente.Endereco = viewModel.Endereco;
        paciente.Bairro = viewModel.Bairro;
        paciente.IdCidade = viewModel.IdCidade;
        paciente.TelResidencial = viewModel.TelResidencial;
        paciente.TelComercial = viewModel.TelComercial;
        paciente.TelCelular = viewModel.TelCelular;
        paciente.Profissao = viewModel.Profissao;
        paciente.FlgAtleta = viewModel.FlgAtleta;
        paciente.FlgGestante = viewModel.FlgGestante;

        // A informação resumida pertence ao vínculo profissional-paciente,
        // não ao cadastro principal do paciente.
        vinculoPaciente.InformacaoResumida = viewModel.InformacaoResumida;

        try
        {
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] =
                "Paciente atualizado com sucesso.";

            return RedirectToAction(nameof(MeuPacienteDetails), new { id = paciente.IdPaciente });
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Não foi possível salvar as alterações do paciente. Verifique os dados informados e tente novamente.");

            await PreencherCidadesPacienteAsync(viewModel);
            return View(viewModel);
        }
    }

    // Gismar Pereira Barbosa
    //
    // Action GET responsável por carregar a tela de confirmação
    // para remover o vínculo entre paciente e profissional logado.
    //
    // Regra de segurança:
    // A action recebe o IdPaciente pela rota, mas só exibe a confirmação
    // se existir vínculo em tbMedico_Paciente entre o paciente solicitado
    // e o profissional autenticado.
    //
    // Observação técnica:
    // Esta exclusão remove apenas o vínculo em tbMedico_Paciente.
    // O cadastro principal do paciente em tbPaciente é preservado.
    [Authorize(Roles = "Medico,Nutricionista")]
    [HttpGet]
    public async Task<IActionResult> ExcluirMeuPaciente(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var idUsuarioLogado = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(idUsuarioLogado))
        {
            return Challenge();
        }

        var profissional = await _context.TbProfissionals
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdUser == idUsuarioLogado);

        if (profissional == null)
        {
            return NotFound("Cadastro profissional não encontrado para o usuário logado.");
        }

        var vinculoPaciente = await _context.TbMedicoPacientes
            .AsNoTracking()
            .Include(mp => mp.IdPacienteNavigation)
                .ThenInclude(p => p.IdCidadeNavigation)
            .FirstOrDefaultAsync(mp =>
                mp.IdProfissional == profissional.IdProfissional &&
                mp.IdPaciente == id.Value);

        if (vinculoPaciente == null)
        {
            return Forbid();
        }

        return View(vinculoPaciente);
    }

    // Gismar Pereira Barbosa
    //
    // Action POST responsável por remover o vínculo entre paciente
    // e profissional logado.
    //
    // Regra de segurança:
    // A validação do vínculo é repetida no POST, pois a operação real
    // não pode depender apenas da tela de confirmação exibida no GET.
    //
    // Regra de negócio:
    // O registro em tbPaciente não é excluído. Apenas o vínculo em
    // tbMedico_Paciente é removido, preservando o paciente no banco.
    [Authorize(Roles = "Medico,Nutricionista")]
    [HttpPost, ActionName("ExcluirMeuPaciente")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirMeuPacienteConfirmed(int id)
    {
        var idUsuarioLogado = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(idUsuarioLogado))
        {
            return Challenge();
        }

        var profissional = await _context.TbProfissionals
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdUser == idUsuarioLogado);

        if (profissional == null)
        {
            return NotFound("Cadastro profissional não encontrado para o usuário logado.");
        }

        var vinculoPaciente = await _context.TbMedicoPacientes
            .FirstOrDefaultAsync(mp =>
                mp.IdProfissional == profissional.IdProfissional &&
                mp.IdPaciente == id);

        if (vinculoPaciente == null)
        {
            return Forbid();
        }

        try
        {
            _context.TbMedicoPacientes.Remove(vinculoPaciente);
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] =
                "Vínculo do paciente removido com sucesso.";

            return RedirectToAction(nameof(MeusPacientes));
        }
        catch (DbUpdateException)
        {
            TempData["MensagemErro"] =
                "Não foi possível remover o vínculo do paciente. Verifique se existem dependências no banco de dados.";

            return RedirectToAction(nameof(MeusPacientes));
        }
    }
    
    // Gismar Pereira Barbosa
    //
    // Método auxiliar responsável por montar um ViewModel novo
    // para as actions GET de registro.
    private async Task<RegistroProfissionalViewModel> MontarViewModelRegistroAsync(
        int idTipoProfissional,
        string tipoProfissionalNome)
    {
        var viewModel = new RegistroProfissionalViewModel
        {
            IdTipoProfissional = idTipoProfissional,
            TipoProfissionalNome = tipoProfissionalNome
        };

        await PreencherCombosRegistroAsync(viewModel);

        return viewModel;
    }

    // Gismar Pereira Barbosa
    //
    // Método auxiliar responsável por preencher os combos da View.
    //
    // Ele é usado tanto no GET quanto no POST inválido.
    // Isso evita duplicação de código e garante que a tela continue
    // funcional quando houver erro de validação.
    private async Task PreencherCombosRegistroAsync(
        RegistroProfissionalViewModel viewModel)
    {
        var tipoProfissionalNome = viewModel.IdTipoProfissional == 1
            ? "Médico"
            : "Nutricionista";

        viewModel.TipoProfissionalNome = tipoProfissionalNome;

        viewModel.Planos = await _context.TbPlanos
            .AsNoTracking()
            .Where(p => p.Nome.StartsWith(tipoProfissionalNome))
            .OrderBy(p => p.Nome)
            .Select(p => new SelectListItem
            {
                Value = p.IdPlano.ToString(),
                Text = $"{p.Nome} - R$ {p.Valor:N2}"
            })
            .ToListAsync();

        viewModel.Cidades = await _context.TbCidades
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .Select(c => new SelectListItem
            {
                Value = c.IdCidade.ToString(),
                Text = c.Nome ?? string.Empty
            })
            .ToListAsync();
    }

    // Gismar Pereira Barbosa
    //
    // Método auxiliar usado pela tela EditarMeusDados.
    //
    // Ele carrega a lista de cidades para o combo da View.
    // Foi separado do método de registro porque a edição não precisa
    // carregar planos, apenas cidades.
    private async Task PreencherCidadesEdicaoAsync(
        EditarMeusDadosProfissionalViewModel viewModel)
    {
        viewModel.Cidades = await _context.TbCidades
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .Select(c => new SelectListItem
            {
                Value = c.IdCidade.ToString(),
                Text = c.Nome ?? string.Empty
            })
            .ToListAsync();
    }

    // Gismar Pereira Barbosa
    //
    // Método auxiliar para validar se o gerente logado pode acessar
    // determinado tipo de profissional.
    //
    // Esta validação é usada nas actions gerenciais de Details, Edit e Delete.
    //
    // O objetivo é centralizar a regra de segurança:
    // GerenteMedico acessa apenas Médico.
    // GerenteNutricionista acessa apenas Nutricionista.
    // GerenteGeral acessa todos.
    private bool UsuarioGerencialPodeAcessarProfissional(int? idTipoProfissional)
    {
        if (User.IsInRole("GerenteGeral"))
        {
            return true;
        }

        if (User.IsInRole("GerenteMedico") && idTipoProfissional == 1)
        {
            return true;
        }

        if (User.IsInRole("GerenteNutricionista") && idTipoProfissional == 2)
        {
            return true;
        }

        return false;
    }

        // Gismar Pereira Barbosa
    //
    // Método auxiliar usado pela edição gerencial.
    //
    // Ele carrega a lista de cidades para o combo da View.
    // A lógica fica separada para evitar repetição dentro das actions
    // GerenciarEdit GET e POST.
    private async Task PreencherCidadesGerencialAsync(
        EditarProfissionalGerencialViewModel viewModel)
    {
        viewModel.Cidades = await _context.TbCidades
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .Select(c => new SelectListItem
            {
                Value = c.IdCidade.ToString(),
                Text = c.Nome ?? string.Empty
            })
            .ToListAsync();
    }

    // Gismar Pereira Barbosa
    //
    // Método auxiliar para converter o IdTipoProfissional em texto.
    //
    // O scaffold do Entity Framework não gerou propriedade de navegação
    // entre TbProfissional e TbTipoProfissional. Por isso, a conversão
    // é feita diretamente a partir do valor da coluna IdTipoProfissional.
    private string ObterNomeTipoProfissional(int? idTipoProfissional)
    {
        return idTipoProfissional switch
        {
            1 => "Médico",
            2 => "Nutricionista",
            _ => "Não identificado"
        };
    }

    // Gismar Pereira Barbosa
    //
    // Método auxiliar usado nas telas de criação e edição de pacientes.
    //
    // Ele carrega a lista de cidades para o combo da View.
    // A lista não é gravada diretamente no banco; apenas o IdCidade
    // selecionado é persistido no cadastro do paciente.
    private async Task PreencherCidadesPacienteAsync(
        CriarMeuPacienteViewModel viewModel)
    {
        viewModel.Cidades = await _context.TbCidades
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .Select(c => new SelectListItem
            {
                Value = c.IdCidade.ToString(),
                Text = c.Nome ?? string.Empty
            })
            .ToListAsync();
    }

    // Gismar Pereira Barbosa
    //
    // Sobrecarga do método de cidades usada na edição de paciente.
    //
    // A lógica é a mesma da criação, mas o ViewModel de edição
    // possui tipo próprio para separar responsabilidades da tela.
    private async Task PreencherCidadesPacienteAsync(
        EditarMeuPacienteViewModel viewModel)
    {
        viewModel.Cidades = await _context.TbCidades
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .Select(c => new SelectListItem
            {
                Value = c.IdCidade.ToString(),
                Text = c.Nome ?? string.Empty
            })
            .ToListAsync();
    }
}