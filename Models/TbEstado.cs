using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbEstado
{
    public int IdEstado { get; set; }

    public int IdPais { get; set; }

    public string? Nome { get; set; }

    public string? Uf { get; set; }

    public virtual TbPai IdPaisNavigation { get; set; } = null!;

    public virtual ICollection<TbCidade> TbCidades { get; set; } = new List<TbCidade>();
}
