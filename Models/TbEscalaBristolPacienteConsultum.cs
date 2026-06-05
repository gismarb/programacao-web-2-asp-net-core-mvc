using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbEscalaBristolPacienteConsultum
{
    public int IdEscalaBristolPacienteConsulta { get; set; }

    public int IdEscalaBristol { get; set; }

    public int IdPaciente { get; set; }

    public int IdHoraPacienteProfissional { get; set; }

    public virtual TbEscalaBristol IdEscalaBristolNavigation { get; set; } = null!;

    public virtual TbHoraPacienteProfissional IdHoraPacienteProfissionalNavigation { get; set; } = null!;

    public virtual TbPaciente IdPacienteNavigation { get; set; } = null!;
}
