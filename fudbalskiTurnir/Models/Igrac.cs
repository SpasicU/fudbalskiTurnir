using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace fudbalskiTurnir.Models;

[Table("Igrac")]
public partial class Igrac
{
    [Key]
    public int IdIgrac { get; set; }

    public string ImeIgraca { get; set; } = null!;

    public int IdTima { get; set; }

    [ForeignKey("IdTima")]
    [InverseProperty("Igracs")]
    public virtual Tim? IdTimaNavigation { get; set; } = null!;
}
