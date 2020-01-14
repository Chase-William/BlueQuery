using System.ComponentModel.DataAnnotations.Schema;

namespace BlueQueryLibrary.ArkBlueprints
{
    /// <summary>
    ///     Super class for all blueprints    
    /// </summary>
    [Table("Blueprint")]
    public class Blueprint
    {
        [Column("comment")]
        public string Comment { get; set; }
        [Column("image_path")]
        public string ImagePath { get; set; }
        [Column("id")]
        public int Id { get; set; }
    }
}
