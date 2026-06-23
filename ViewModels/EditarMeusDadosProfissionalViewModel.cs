using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Projeto1_IF.ViewModels;

// Gismar Pereira Barbosa
//
// ViewModel criado para a edição dos dados do próprio profissional logado.
//
// Justificativa técnica:
// Não utilizamos diretamente o model TbProfissional na tela de edição,
// porque a tabela possui campos que não devem ser alterados pelo usuário,
// como IdUser, IdContrato, IdTipoProfissional, IdTipoAcesso e CPF.
//
// O CPF aparece apenas para visualização na View, mas não será atualizado
// pelo Controller. Isso atende ao requisito do trabalho que impede o
// profissional de alterar o CPF após o cadastro.
public class EditarMeusDadosProfissionalViewModel
{
    // Identificador interno do profissional.
    // Será usado pelo Controller apenas para controle da operação,
    // sempre validando também o IdUser do usuário autenticado.
    public int IdProfissional { get; set; }

    [Display(Name = "CPF")]
    public string Cpf { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [Display(Name = "Nome completo")]
    public string Nome { get; set; } = string.Empty;

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

    // Lista usada para montar o combo de cidades na View.
    // Não é gravada diretamente no banco.
    public IEnumerable<SelectListItem> Cidades { get; set; } = new List<SelectListItem>();
}