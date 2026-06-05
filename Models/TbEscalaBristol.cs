using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbEscalaBristol
{
    public int IdEscalaBristol { get; set; }

    public string Nome { get; set; } = null!;

    public bool? Sangue { get; set; }

    public virtual ICollection<TbEscalaBristolPacienteConsultum> TbEscalaBristolPacienteConsulta { get; set; } = new List<TbEscalaBristolPacienteConsultum>();
}
