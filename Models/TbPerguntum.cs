using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbPerguntum
{
    public int IdPergunta { get; set; }

    public int IdProfissional { get; set; }

    public string Nome { get; set; } = null!;

    public bool Ativo { get; set; }

    public virtual TbProfissional IdProfissionalNavigation { get; set; } = null!;

    public virtual ICollection<TbRastreamentoRespostum> TbRastreamentoResposta { get; set; } = new List<TbRastreamentoRespostum>();
}
