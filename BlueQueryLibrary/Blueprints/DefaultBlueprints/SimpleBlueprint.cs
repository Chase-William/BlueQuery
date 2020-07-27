using BlueQueryLibrary.Lang;
using System.Collections.Generic;

namespace BlueQueryLibrary.Blueprints.DefaultBlueprints
{
    /// <summary>
    ///     Represents a simple blueprint that doesn't take any resources that need to be crafted before hand.<br/>
    ///     Example blueprint: Electronics <- Needs to be crafted itself but doesn't have any resources it personally needs crafted before it itself can be crafted
    /// </summary>
    public class SimpleBlueprint : IResourceCalculator
    {
        public const string BUNDLED_AMOUNT_KEY = "amount";
        public SortedList<string, double> Resources { get; set; }
        public int Yield { get; set; }

        public virtual IEnumerable<CalculatedResourceCost> GetResourceCost(Bundle _bundle)
        {
            var calculatedResources = new List<CalculatedResourceCost>();

            for (int i = 0; i < Resources.Count; i++)
            {
                calculatedResources.Add(new CalculatedResourceCost
                {
                    Type = Resources.Keys[i],
                    // (resource value * how many) / by how many are produced.
                    Amount = (Resources.Values[i] * (int)_bundle.BundledInformation[BUNDLED_AMOUNT_KEY]) / Yield
                });
            }

            return calculatedResources;
        }
    }    
}
