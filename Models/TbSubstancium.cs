using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbSubstancium
{
    public int IdSubstancia { get; set; }

    public string Nome { get; set; } = null!;

    public string? InformacaoComplementar { get; set; }
}
