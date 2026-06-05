using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbCategoriaMedicamento
{
    public int IdCategoriaMedicamento { get; set; }

    public string Nome { get; set; } = null!;

    public string? InformacaoComplementar { get; set; }

    public virtual ICollection<TbMedicamento> TbMedicamentos { get; set; } = new List<TbMedicamento>();
}
