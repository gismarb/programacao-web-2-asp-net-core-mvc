using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbPatologium
{
    public int IdPatologia { get; set; }

    public string Nome { get; set; } = null!;

    public string? InformacaoComplementar { get; set; }

    public virtual ICollection<TbGrupoPatologicoXPatologium> TbGrupoPatologicoXPatologia { get; set; } = new List<TbGrupoPatologicoXPatologium>();

    public virtual ICollection<TbHistoriaPatologica> TbHistoriaPatologicas { get; set; } = new List<TbHistoriaPatologica>();

    public virtual ICollection<TbHistoricoDoencaAtual> TbHistoricoDoencaAtuals { get; set; } = new List<TbHistoricoDoencaAtual>();
}
