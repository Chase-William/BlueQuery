using System.ComponentModel.DataAnnotations.Schema;

namespace BlueQueryLibrary.ArkBlueprints
{
    /// <summary>
    ///     Child class of blueprint but super class to all other saddle based classes
    /// </summary>
    [Table(nameof(Saddle))]
    public abstract class Saddle
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("armor")]
        public float Armor { get; set; }
        [Column("discriminator")]
        public int Discriminator { get; set; }
        [Column("comment")]
        public string Comment { get; set; }
    }

    public enum SaddleType { Giganotosaurus, Managarmr }
}
