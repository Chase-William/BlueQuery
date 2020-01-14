using System.ComponentModel.DataAnnotations.Schema;

namespace BlueQueryLibrary.ArkBlueprints
{
    /// <summary>
    ///     Child class of blueprint but super class to all other saddle based classes
    /// </summary>
    public abstract class Saddle : Blueprint
    {
        [Column("armor")]
        public float Armor { get; set; }
        [Column("saddle_id")]
        public int SaddleId { get; set; }
    }
}
