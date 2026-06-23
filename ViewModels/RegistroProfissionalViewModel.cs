using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Projeto1_IF.ViewModels;

// Gismar Pereira Barbosa
//
// ViewModel criado para atender ao registro completo de profissionais
// solicitado no Trabalho Final de Programação Web II.
//
// Justificativa técnica:
// A tela de registro de Médico/Nutricionista não representa apenas uma tabela.
// Ela precisa reunir informações de autenticação do ASP.NET Core Identity
// e dados das tabelas de domínio da aplicação, como tbProfissional,
// tbContrato, tbPlano e tbCidade.
//
// Por esse motivo, não é adequado usar diretamente o model TbProfissional
// como model da View. O ViewModel abaixo representa os campos necessários
// para a tela de cadastro completo, sem alterar a estrutura do banco de dados.
public class RegistroProfissionalViewModel
{
    // Dados de autenticação do usuário.
    // Esses campos serão usados para criar um registro em AspNetUsers
    // por meio do UserManager<IdentityUser>.
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, ErrorMessage = "A senha deve ter no mínimo {2} e no máximo {1} caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação da senha é obrigatória.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "A senha e a confirmação não conferem.")]
    [Display(Name = "Confirmar senha")]
    public string ConfirmPassword { get; set; } = string.Empty;

    // Dados principais do profissional.
    // Esses campos serão usados para criar o registro em tbProfissional.
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [Display(Name = "Nome completo")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [Display(Name = "CPF")]
    public string Cpf { get; set; } = string.Empty;

    [Display(Name = "CRM/CRN")]
    public string? CrmCrn { get; set; }

    [Display(Name = "Especialidade")]
    public string? Especialidade { get; set; }

    // Dados de endereço.
    // Alguns campos são obrigatórios porque o model TbProfissional
    // possui propriedades não anuláveis no banco, como Numero, Bairro e Cep.
    [Display(Name = "Logradouro")]
    public string? Logradouro { get; set; }

    [Required(ErrorMessage = "O número é obrigatório.")]
    [Display(Name = "Número")]
    public string Numero { get; set; } = string.Empty;

    [Required(ErrorMessage = "O bairro é obrigatório.")]
    [Display(Name = "Bairro")]
    public string Bairro { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CEP é obrigatório.")]
    [Display(Name = "CEP")]
    public string Cep { get; set; } = string.Empty;

    // Chave estrangeira para tbCidade.
    // Na tela, o usuário escolherá o nome da cidade em um combo,
    // mas o valor gravado será o IdCidade.
    [Required(ErrorMessage = "A cidade é obrigatória.")]
    [Display(Name = "Cidade")]
    public int IdCidade { get; set; }

    // Chave estrangeira para tbPlano.
    // O plano será usado para criar um registro em tbContrato.
    [Required(ErrorMessage = "O plano é obrigatório.")]
    [Display(Name = "Plano")]
    public int IdPlano { get; set; }

    // Dados opcionais de contato.
    [Display(Name = "DDD principal")]
    public string? Ddd1 { get; set; }

    [Display(Name = "Telefone principal")]
    public string? Telefone1 { get; set; }

    [Display(Name = "DDD secundário")]
    public string? Ddd2 { get; set; }

    [Display(Name = "Telefone secundário")]
    public string? Telefone2 { get; set; }

    // Tipo de profissional definido pelo fluxo escolhido:
    // 1 = Médico
    // 2 = Nutricionista
    //
    // Esse campo não deve ser escolhido livremente pelo usuário.
    // Ele será preenchido pelo Controller conforme a rota acessada.
    public int IdTipoProfissional { get; set; }

    // Texto usado apenas para exibição na View, por exemplo:
    // "Médico" ou "Nutricionista".
    public string TipoProfissionalNome { get; set; } = string.Empty;

    // Listas usadas para montar os combos da tela.
    // Elas não são gravadas diretamente no banco.
    public IEnumerable<SelectListItem> Cidades { get; set; } = new List<SelectListItem>();

    public IEnumerable<SelectListItem> Planos { get; set; } = new List<SelectListItem>();
}