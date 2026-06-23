namespace Projeto1_IF.ViewModels;

// Gismar Pereira Barbosa
//
// ViewModel criado para representar os dados exibidos na listagem gerencial
// de profissionais.
//
// Justificativa técnica:
// A tela gerencial não precisa receber o model TbProfissional completo.
// Ela precisa apenas de dados resumidos para exibição, como nome, CPF,
// tipo profissional, plano e e-mail do usuário vinculado.
//
// Esta abordagem reduz acoplamento com o model de banco e facilita aplicar
// filtros por perfil gerencial no Controller.
public class ProfissionalGerencialViewModel
{
    public int IdProfissional { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Cpf { get; set; } = string.Empty;

    public int? IdTipoProfissional { get; set; }

    public string TipoProfissional { get; set; } = string.Empty;

    public string? CrmCrn { get; set; }

    public string? Especialidade { get; set; }

    public string? Cidade { get; set; }

    public string EmailUsuario { get; set; } = string.Empty;

    public string Plano { get; set; } = string.Empty;
}