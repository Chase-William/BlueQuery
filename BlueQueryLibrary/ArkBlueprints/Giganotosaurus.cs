using System.ComponentModel.DataAnnotations.Schema;

namespace BlueQueryLibrary.ArkBlueprints
{
    public class Giganotosaurus : Saddle
    {
        [Column("fiber")]
        public int Fiber { get; set; }
        [Column("hide")]
        public int Hide { get; set; }
        [Column("metal")]
        public int Metal { get; set; }
    }
}
