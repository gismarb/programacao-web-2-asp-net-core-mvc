using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbGruposReceitasDespesa
{
    public int IdReceitaDespesa { get; set; }

    public int Tipo { get; set; }

    public string Nome { get; set; } = null!;

    public virtual ICollection<TbLancarReceitasDespesa> TbLancarReceitasDespesas { get; set; } = new List<TbLancarReceitasDespesa>();
}
