namespace Projeto1_IF.ViewModels;

// Gismar Pereira Barbosa
//
// ViewModel criado para representar a listagem de pacientes
// vinculados ao profissional logado.
//
// Justificativa técnica:
// A tela MeusPacientes não precisa receber o model TbPaciente completo.
// Ela exibe apenas dados resumidos do paciente e informações do vínculo
// existente em tbMedico_Paciente.
//
// A filtragem de segurança é feita no Controller, sempre a partir do
// IdUser do usuário autenticado e do IdProfissional correspondente.
public class PacienteProfissionalViewModel
{
    public int IdPaciente { get; set; }

    public int IdMedicoPaciente { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Cpf { get; set; } = string.Empty;

    public string Rg { get; set; } = string.Empty;

    public DateOnly DataNascimento { get; set; }

    public string Sexo { get; set; } = string.Empty;

    public string? Cidade { get; set; }

    public string? TelCelular { get; set; }

    public string? InformacaoResumida { get; set; }
}