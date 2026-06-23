using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Projeto1_IF.ViewModels;

// Gismar Pereira Barbosa
//
// ViewModel criado para edição de paciente pelo profissional logado.
//
// Justificativa técnica:
// A edição envolve dados do paciente, armazenados em tbPaciente,
// e também a informação resumida do vínculo, armazenada em tbMedico_Paciente.
//
// O IdProfissional não é recebido da tela. Ele é identificado no Controller
// a partir do usuário autenticado no ASP.NET Core Identity.
public class EditarMeuPacienteViewModel
{
    public int IdPaciente { get; set; }

    public int IdMedicoPaciente { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O RG é obrigatório.")]
    [Display(Name = "RG")]
    public string Rg { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [Display(Name = "CPF")]
    public string Cpf { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data de nascimento")]
    public DateOnly DataNascimento { get; set; }

    [Required(ErrorMessage = "O sexo é obrigatório.")]
    [Display(Name = "Sexo")]
    public string Sexo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A etnia é obrigatória.")]
    [Display(Name = "Etnia")]
    public int Etnia { get; set; }

    [Display(Name = "Nome do responsável")]
    public string? NomeResponsavel { get; set; }

    [Display(Name = "Endereço")]
    public string? Endereco { get; set; }

    [Display(Name = "Bairro")]
    public string? Bairro { get; set; }

    [Display(Name = "Cidade")]
    public int? IdCidade { get; set; }

    [Display(Name = "Telefone residencial")]
    public string? TelResidencial { get; set; }

    [Display(Name = "Telefone comercial")]
    public string? TelComercial { get; set; }

    [Display(Name = "Telefone celular")]
    public string? TelCelular { get; set; }

    [Display(Name = "Profissão")]
    public string? Profissao { get; set; }

    // Campos booleanos usados pelos checkboxes da View.
    // Foi utilizado bool em vez de bool? porque, na tela, a escolha é binária:
    // marcado representa Sim; desmarcado representa Não.
    [Display(Name = "Atleta")]
    public bool FlgAtleta { get; set; }

    [Display(Name = "Gestante")]
    public bool FlgGestante { get; set; }

    [Display(Name = "Informação resumida")]
    public string? InformacaoResumida { get; set; }

    public IEnumerable<SelectListItem> Cidades { get; set; } = new List<SelectListItem>();
}