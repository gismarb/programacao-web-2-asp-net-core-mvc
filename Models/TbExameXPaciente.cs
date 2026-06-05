using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbExameXPaciente
{
    public int IdExameXPaciente { get; set; }

    public int IdExame { get; set; }

    public int IdPaciente { get; set; }

    public DateOnly? Data { get; set; }

    public string? Resultado { get; set; }

    public virtual TbExame IdExameNavigation { get; set; } = null!;

    public virtual TbPaciente IdPacienteNavigation { get; set; } = null!;
}
