using System;
using System.Collections.Generic;
using System.Collections.Specialized;
namespace BlueQueryLibrary.ArkBlueprints.DefaultBlueprints
{
    public abstract class Blueprint 
    {
        public SortedList<string, double> Resources { get; set; }
        public int Yield { get; set; }
        public abstract IEnumerable<CalculatedResourceCost> GetResourceCost(int _amount);
    }

    public abstract class Bullet : Blueprint
    {
        public override IEnumerable<CalculatedResourceCost> GetResourceCost(int _amount)
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
    public class AdvancedRifleBullet : Bullet { }
    public class SimpleRifleBullet : Bullet { }
    public class SimpleBullet : Bullet { }
    public class AdvancedBullet : Bullet { }
    public class AdvancedSniperBullet : Bullet { }

    public class SimpleShotgunBullet : Bullet
    {
        public float SimpleBullets { get; set; }

        // Adding special implmentation to handle the simplebullets
        public override IEnumerable<CalculatedResourceCost> GetResourceCost(int _amount)
        {
            return base.GetResourceCost(_amount);
        }
    }

    public struct CalculatedResourceCost
    {
        public string Type { get; set; }
        public double Amount { get; set; }
    }
}