using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbRastreamentoMetabolico
{
    public int IdRastreamentoMetabolico { get; set; }

    public int IdRastreamentoResposta { get; set; }

    public int IdHoraPacienteProfissional { get; set; }

    public string? ObsGeral { get; set; }

    public int? Total { get; set; }

    public virtual TbHoraPacienteProfissional IdHoraPacienteProfissionalNavigation { get; set; } = null!;

    public virtual TbRastreamentoRespostum IdRastreamentoRespostaNavigation { get; set; } = null!;
}
