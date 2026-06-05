using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbPaciente
{
    public int IdPaciente { get; set; }

    public string Nome { get; set; } = null!;

    public string Rg { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public DateOnly DataNascimento { get; set; }

    public string? NomeResponsavel { get; set; }

    public string Sexo { get; set; } = null!;

    public int Etnia { get; set; }

    public string? Endereco { get; set; }

    public string? Bairro { get; set; }

    public int? IdCidade { get; set; }

    public string? TelResidencial { get; set; }

    public string? TelComercial { get; set; }

    public string? TelCelular { get; set; }

    public string? Profissao { get; set; }

    public bool? FlgAtleta { get; set; }

    public bool? FlgGestante { get; set; }

    public virtual TbCidade? IdCidadeNavigation { get; set; }

    public virtual ICollection<TbAntropometrium> TbAntropometria { get; set; } = new List<TbAntropometrium>();

    public virtual ICollection<TbEscalaBristolPacienteConsultum> TbEscalaBristolPacienteConsulta { get; set; } = new List<TbEscalaBristolPacienteConsultum>();

    public virtual ICollection<TbExameXPaciente> TbExameXPacientes { get; set; } = new List<TbExameXPaciente>();

    public virtual ICollection<TbHistoriaPatologica> TbHistoriaPatologicas { get; set; } = new List<TbHistoriaPatologica>();

    public virtual ICollection<TbHistoricoAlimentarNutricional> TbHistoricoAlimentarNutricionals { get; set; } = new List<TbHistoricoAlimentarNutricional>();

    public virtual ICollection<TbHistoricoDoencaAtual> TbHistoricoDoencaAtuals { get; set; } = new List<TbHistoricoDoencaAtual>();

    public virtual ICollection<TbHistoricoSocialAlimentar> TbHistoricoSocialAlimentars { get; set; } = new List<TbHistoricoSocialAlimentar>();

    public virtual ICollection<TbHoraPacienteProfissional> TbHoraPacienteProfissionals { get; set; } = new List<TbHoraPacienteProfissional>();

    public virtual ICollection<TbMedicoPaciente> TbMedicoPacientes { get; set; } = new List<TbMedicoPaciente>();
}
