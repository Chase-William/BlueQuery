using BlueQueryLibrary.Blueprints.DefaultBlueprints;
using BlueQueryLibrary.Lang;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueQueryLibrary.Blueprints
{
    public interface IResourceCalculator
    {
        /// <summary>
        ///     A common function that declares whatever implements it has a resource cost<br/>       
        ///     AdvancedBlueprints implement this function in a recursive manner because they have resources that are blueprints themselves.<br/>      
        /// </summary>
        IEnumerable<CalculatedResourceCost> GetResourceCost(Bundle _bundle);
    }
}
