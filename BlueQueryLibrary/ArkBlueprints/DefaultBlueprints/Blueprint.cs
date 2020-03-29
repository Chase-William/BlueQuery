using System.Collections.Generic;

namespace BlueQueryLibrary.ArkBlueprints.DefaultBlueprints
{
    public class Blueprint
    {
        public SortedList<string, double> Resources { get; set; }
        public int Yield { get; set; }
        public virtual IEnumerable<CalculatedResourceCost> GetResourceCost(double _amount)
        {
            var calculatedResources = new List<CalculatedResourceCost>();

            for (int i = 0; i < Resources.Count; i++)
            {
                calculatedResources.Add(new CalculatedResourceCost
                {
                    Type = Resources.Keys[i],
                    // (resource value * how many) / by how many are produced.
                    Amount = (Resources.Values[i] * _amount) / Yield
                });
            }

            return calculatedResources;
        }
    }

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
