using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbTipoAcesso
{
    public int IdTipoAcesso { get; set; }

    public string Nome { get; set; } = null!;

    public bool FlagAtivo { get; set; }

    public virtual ICollection<TbProfissional> TbProfissionals { get; set; } = new List<TbProfissional>();
}
