using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace fudbalskiTurnir.Models;

[Table("Rezultati")]
public partial class Rezultati
{
    [Key]
    public int IdRez { get; set; }

    [Column("tim1Id")]
    public int Tim1Id { get; set; }

    [Column("tim2Id")]
    public int Tim2Id { get; set; }

    [Column("tim1Golovi")]
    public int Tim1Golovi { get; set; }

    [Column("tim2Golovi")]
    public int Tim2Golovi { get; set; }

    [ForeignKey("Tim1Id")]
    [InverseProperty("RezultatiTim1s")]
    public virtual Tim? Tim1 { get; set; } = null!;

    [ForeignKey("Tim2Id")]
    [InverseProperty("RezultatiTim2s")]
    public virtual Tim? Tim2 { get; set; } = null!;
}
