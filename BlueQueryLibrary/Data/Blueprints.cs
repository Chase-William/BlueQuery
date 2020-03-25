using BlueQueryLibrary.ArkBlueprints.DefaultBlueprints;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BlueQueryLibrary.Data
{
    public class Blueprints
    {
        public Dictionary<string, object> DefaultBlueprints = new Dictionary<string, object>();

        public Blueprints()
        {
            //StaticBlueprints = new Dictionary<string, object>()
            //{
            //    { nameof(AdvancedRifleBullet),  new AdvancedRifleBullet() },
            //    { nameof(SimpleRifleBullet),  new SimpleRifleBullet() },
            //    { nameof(SimpleBullet),  new SimpleBullet() },
            //    { nameof(SimpleShotgunBullet),  new SimpleShotgunBullet() },
            //    { nameof(AdvancedBullet),  new AdvancedBullet() },
            //    { nameof(AdvancedSniperBullet),  new AdvancedSniperBullet() }
            //};

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };

            //using StreamWriter writer = new StreamWriter("../../../../default_blueprints.json");

            //writer.Write(JsonConvert.SerializeObject(StaticBlueprints, settings));

            //writer.Close();

            //var testString = JsonConvert.SerializeObject(ammos, settings);

            // read from json file and populate data

            DefaultBlueprints = JsonConvert.DeserializeObject<Dictionary<string, object>>(new StreamReader("../../../../default_blueprints.json").ReadToEnd(), settings);
        }
    }
}
