using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbReceitaAlimentarPadrao
{
    public int IdReceitaAlimentarPadrao { get; set; }

    public int? IdProfissional { get; set; }

    public string Nome { get; set; } = null!;

    public string? InformacaoComplementar { get; set; }

    public virtual TbProfissional? IdProfissionalNavigation { get; set; }

    public virtual ICollection<TbReceitaAlimentarPadraoXAlimento> TbReceitaAlimentarPadraoXAlimentos { get; set; } = new List<TbReceitaAlimentarPadraoXAlimento>();
}
