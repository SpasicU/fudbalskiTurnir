using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace fudbalskiTurnir.Models;

[Table("Tim")]
public partial class Tim
{
    [Key]
    public int IdTima { get; set; }

    public string Naziv { get; set; } = null!;

    public int Bodovi { get; set; }

    [InverseProperty("IdTimaNavigation")]
    public virtual ICollection<Igrac> Igracs { get; set; } = new List<Igrac>();

    [InverseProperty("Tim1")]
    public virtual ICollection<Rezultati> RezultatiTim1s { get; set; } = new List<Rezultati>();

    [InverseProperty("Tim2")]
    public virtual ICollection<Rezultati> RezultatiTim2s { get; set; } = new List<Rezultati>();

    public override string ToString()
    {
        return "IdTima-" + IdTima + " Naziv-"+Naziv;
    }
}
