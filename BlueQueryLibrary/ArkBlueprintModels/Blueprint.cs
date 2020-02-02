using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueQueryLibrary.ArkBlueprints
{
    /// <summary>
    ///     Child class of blueprint but super class to all other saddle based classes
    /// </summary>
    [Table(nameof(Blueprint))]
    public abstract class Blueprint
    {
        [Key]
        public int Id { get; set; }
        [Column("armor")]
        public float Armor { get; set; }        
        [Column("comment")]
        public string Comment { get; set; }
        [Column("image_path")]
        public string ImagePath { get; set; }
        [Column("discriminator")]
        private int discriminator;

        /// <summary>
        ///     Property converts the BlueprintType enum to an integer when getting and setting for the database.
        /// </summary>
        public BlueprintType Discriminator 
        { 
            get => (BlueprintType)discriminator;
            set {
                discriminator = (int)value;
            } 
        }
    }

    public enum BlueprintType { Giganotosaurus, Managarmr }
}
