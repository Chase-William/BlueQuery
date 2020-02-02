using System.ComponentModel.DataAnnotations.Schema;

namespace BlueQueryLibrary.ArkBlueprints
{
    public class Giganotosaurus : Blueprint
    {
        [Column("fiber")]
        public int Fiber { get; set; }
        [Column("hide")]
        public int Hide { get; set; }
        [Column("metal")]
        public int Metal { get; set; }

        public override string ToString()
        {
            return $"Id: {Id} Armor: {Armor} Metal: {Metal} Hide: {Hide} Fiber: {Fiber}";
        }
    }
}
