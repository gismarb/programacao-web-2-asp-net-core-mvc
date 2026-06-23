using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Projeto1_IF.ViewModels;

// Gismar Pereira Barbosa
//
// ViewModel criado para edição gerencial de profissionais.
//
// Justificativa técnica:
// A edição feita por gerente possui permissões diferentes da edição feita
// pelo próprio profissional. Nesta tela, o gerente pode editar o CPF,
// mas não altera dados estruturais como IdUser, IdContrato, IdTipoProfissional
// ou IdTipoAcesso.
//
// O tipo profissional é mantido apenas para validação de autorização no
// Controller e para exibição na View.
public class EditarProfissionalGerencialViewModel
{
    public int IdProfissional { get; set; }

    public int? IdTipoProfissional { get; set; }

    public string TipoProfissionalNome { get; set; } = string.Empty;

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

    [Required(ErrorMessage = "A cidade é obrigatória.")]
    [Display(Name = "Cidade")]
    public int IdCidade { get; set; }

    [Display(Name = "DDD principal")]
    public string? Ddd1 { get; set; }

    [Display(Name = "Telefone principal")]
    public string? Telefone1 { get; set; }

    [Display(Name = "DDD secundário")]
    public string? Ddd2 { get; set; }

    [Display(Name = "Telefone secundário")]
    public string? Telefone2 { get; set; }

    public IEnumerable<SelectListItem> Cidades { get; set; } = new List<SelectListItem>();
}