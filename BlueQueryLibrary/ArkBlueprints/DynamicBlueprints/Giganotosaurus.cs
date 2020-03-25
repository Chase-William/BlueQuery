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

        /// <summary>
        ///     Prints the important information about this class and its dervied class in a formatted manner.
        ///     The formatting matches the formatting returned by <see cref="GetQueryHeader"/>.
        ///     <para/>
        ///     IMPORTANT:
        ///     <para> 
        ///         This function won't be implicitly called by the base class ToString because this ToString 
        ///         implementation uses the "new" keyword instead of "override" keyword. 
        ///     </para>        
        /// </summary>
        /// <returns> The important properties of this class's formatted to match GetQueryHeader formatting. </returns>
        public new string ToString()
        {
            return $"{Id,-10}{Armor,-10}{Metal,-10}{Hide,-10}{Fiber,-10}";
        }

        /// <summary>
        ///     When querying data, use this method to add a header to the console that will 
        ///         align nicely with the ToString calls for the intances.
        /// </summary>
        /// <returns> A header formatted to match ToString formatting. </returns>
        public new static string GetQueryHeader()
        {            
            return $"--{nameof(Giganotosaurus)}--\n\n{nameof(Id),-10}{nameof(Armor),-10}{nameof(Metal),-10}{nameof(Hide),-10}{nameof(Fiber),-10}\n";
        }
    }
}
