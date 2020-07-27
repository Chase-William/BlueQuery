using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueQueryLibrary.Blueprints.CustomBlueprints
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
        ///     Property converts the BlueprintType enum to an integer whe and setting and reversed for getting.
        /// </summary>
        public BlueprintType Discriminator 
        { 
            get => (BlueprintType)discriminator;
            set {
                discriminator = (int)value;
            } 
        }

        /// <summary>
        ///     Prints the important information about the <see cref="Blueprint"/> class to the console.
        ///     The formatting matches the formatting returned by <see cref="GetQueryHeader"/>.
        /// </summary>
        public override string ToString()
        {
            return $"{Id,-10}{Discriminator,-10}";
        }

        /// <summary>
        ///     When querying data, use this method to add a header to the console that will 
        ///         align nicely with the ToString calls for the intances.
        /// </summary>
        /// <returns> A header formatted to match ToString formatting. </returns>
        public static string GetQueryHeader()
        {            
            return $"--{nameof(Blueprint)}--\n\n{nameof(Id),-10}{nameof(Discriminator),-10}\n";
        }
    }

    public enum BlueprintType { Giganotosaurus, Managarmr }
}
