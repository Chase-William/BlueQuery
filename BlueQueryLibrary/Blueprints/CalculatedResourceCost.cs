using System;
using System.Collections.Generic;
using System.Text;

namespace BlueQueryLibrary.Blueprints
{
    /// <summary>
    ///     Represents a blueprint resource type that can contain other blueprints within itself (recursive).
    /// </summary>
    public class CalculatedResourceCost
    {
        /// <summary>
        ///     The blueprint's type.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        ///     The amount of instances of the blueprint.
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        ///     A collection of calculated blueprint cost required to craft the desired amount of this blueprint.
        /// </summary>
        public IEnumerable<CalculatedResourceCost> CalculatedResourceCosts { get; set; }
    }
}
