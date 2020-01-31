using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BlueQueryLibrary.ArkBlueprints
{
    public class Managarmr : Saddle
    {
        [Column("fiber")]
        public int Fiber { get; set; }
        [Column("hide")]
        public int Hide { get; set; }
        [Column("chitin")]
        public int Chitin { get; set; }
    }
}
