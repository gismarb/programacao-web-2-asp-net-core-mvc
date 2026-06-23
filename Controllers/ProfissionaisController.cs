using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projeto1_IF.Data;
using Projeto1_IF.Models;
using Projeto1_IF.ViewModels;

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
    private readonly UserManager<IdentityUser> _userManager;

    // Gismar Pereira Barbosa
    //
    // Injeção de dependências usada pelo ASP.NET Core.
    //
    // db_IFContext:
    // acesso às tabelas do banco da disciplina, como tbProfissional,
    // tbContrato, tbPlano e tbCidade.
    //
    // UserManager<IdentityUser>:
    // serviço do ASP.NET Core Identity usado para criar usuários,
    // consultar usuário logado e associar Roles.
    public ProfissionaisController(
        db_IFContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
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
    // Não usamos scaffolding aqui porque o cadastro envolve múltiplas tabelas
    // e regras de negócio que não pertencem a um CRUD simples.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(RegistroProfissionalViewModel viewModel)
    {
        // Gismar Pereira Barbosa
        //
        // Validação defensiva do tipo profissional.
        // Mesmo que a View envie o IdTipoProfissional em campo hidden,
        // não podemos confiar cegamente no valor vindo do navegador.
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
            // nas tabelas de domínio, removemos esse usuário para evitar
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
}