using System;
using System.Collections.Generic;

namespace Projeto1_IF.Models;

public partial class TbGrupoPatologico
{
    public int IdGrupoPatologico { get; set; }

    public string Nome { get; set; } = null!;

    public virtual ICollection<TbGrupoPatologicoXPatologium> TbGrupoPatologicoXPatologia { get; set; } = new List<TbGrupoPatologicoXPatologium>();
}
