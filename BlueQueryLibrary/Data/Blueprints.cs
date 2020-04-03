using BlueQueryLibrary.ArkBlueprints.DefaultBlueprints;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System;

namespace BlueQueryLibrary.Data
{
    public static class Blueprints
    {
        public static Dictionary<string, IResourceCalculator> DefaultBlueprints = new Dictionary<string, IResourceCalculator>();

        static Blueprints()
        {
            //var test = new Dictionary<string, Blueprint>()
            //{
            //    { nameof(AdvancedRifleBullet),
            //        new AdvancedRifleBullet() {
            //            Resources = new SortedList<string, double>()
            //            {
            //                { "Metal Ingots", 1 },
            //                { "Gun Powder", 9 }
            //            },
            //            Yield = 2
            //        }
            //    },
            //    { nameof(SimpleRifleBullet),
            //        new SimpleRifleBullet() {
            //            Resources = new SortedList<string, double>()
            //            {
            //                { "Metal Ingots", 2 },
            //                { "Gun Powder", 12 }
            //            },
            //            Yield = 2
            //        }
            //    },
            //    { nameof(SimpleBullet),
            //        new SimpleBullet() {
            //            Resources = new SortedList<string, double>()
            //            {
            //                { "Metal Ingots", 1 },
            //                { "Gun Powder", 6 }
            //            },
            //            Yield = 2
            //        }
            //    },
            //    { nameof(SimpleShotgunBullet),
            //        new SimpleShotgunBullet() {
            //            Resources = new SortedList<string, double>()
            //            {
            //                { "Metal Ingots", 1 },
            //                { "Gun Powder", 3 },
            //                { nameof(SimpleBullet), 3 }
            //            },
            //            Yield = 1
            //        }
            //    },
            //    { nameof(AdvancedBullet),
            //        new AdvancedBullet() {
            //            Resources = new SortedList<string, double>()
            //            {
            //                { "Metal Ingots", 1 },
            //                { "Gun Powder", 3 }
            //            },
            //            Yield = 3
            //        }
            //    },
            //    { nameof(AdvancedSniperBullet),
            //        new AdvancedSniperBullet() {
            //            Resources = new SortedList<string, double>()
            //            {
            //                { "Metal Ingots", 2 },
            //                { "Gun Powder", 12 }
            //            },
            //            Yield = 2
            //        }
            //    },
            //};

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };

            //using StreamWriter writer = new StreamWriter("../../../../default_blueprints.json");
            //writer.Write(JsonConvert.SerializeObject(test, settings));
            //writer.Close();



            //var test = new CraftableResource();

            //using StreamWriter writer = new StreamWriter("../../../../test.json");
            //writer.Write(JsonConvert.SerializeObject(test, settings));
            //writer.Close();

            //using StreamWriter writer = new StreamWriter("../../../../default_blueprints.json");

            //writer.Write(JsonConvert.SerializeObject(StaticBlueprints, settings));

            //writer.Close();

            //var testString = JsonConvert.SerializeObject(ammos, settings);

            // read from json file and populate data

            DefaultBlueprints = JsonConvert.DeserializeObject<Dictionary<string, IResourceCalculator>>(new StreamReader("../../../../default_blueprints.json").ReadToEnd(), settings);
        }
    }
}
