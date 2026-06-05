using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbPlano
{
    public int IdPlano { get; set; }

    public string Nome { get; set; } = null!;

    public int Validade { get; set; }

    public decimal Valor { get; set; }

    public virtual ICollection<TbContrato> TbContratos { get; set; } = new List<TbContrato>();
}
