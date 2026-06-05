using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbLancarReceitasDespesa
{
    public int IdLancamento { get; set; }

    public int IdReceitaDespesa { get; set; }

    public DateTime Data { get; set; }

    public virtual TbGruposReceitasDespesa IdReceitaDespesaNavigation { get; set; } = null!;
}
