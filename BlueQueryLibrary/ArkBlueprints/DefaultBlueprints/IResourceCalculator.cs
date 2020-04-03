using BlueQueryLibrary.Lang;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueQueryLibrary.ArkBlueprints.DefaultBlueprints
{
    public interface IResourceCalculator
    {
        IEnumerable<CalculatedResourceCost> GetResourceCost(Bundle _bundle);
    }
}
