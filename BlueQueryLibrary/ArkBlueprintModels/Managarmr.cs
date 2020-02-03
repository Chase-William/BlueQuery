using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BlueQueryLibrary.ArkBlueprints
{
    public class Managarmr : Blueprint
    {
        [Column("fiber")]
        public int Fiber { get; set; }
        [Column("hide")]
        public int Hide { get; set; }
        [Column("chitin")]
        public int Chitin { get; set; }

        public override string ToString()
        {
            return $"{Id,-9}{Armor,-12}{Hide,-10}{Fiber,-10}{Chitin}";
        }
    }
}
