using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbReceitaMedicaPadrao
{
    public int IdReceitaMedicaPadrao { get; set; }

    public int IdProfissional { get; set; }

    public string Nome { get; set; } = null!;

    public string? InformacaoComplementar { get; set; }

    public virtual TbProfissional IdProfissionalNavigation { get; set; } = null!;
}
