using System.Linq;
using BlueQueryLibrary.ArkBlueprints;

namespace BlueQueryLibrary
{
    public class BlueprintProvider : IBlueprintProvider
    {
        private readonly BlueprintContext blueprintContext;

        public BlueprintProvider(BlueprintContext _blueprintContext)
        {
            this.blueprintContext = _blueprintContext;
        }

        public Blueprint GetBlueprint(int _id)
        {
            return blueprintContext.Blueprints.Where(e => e.Id == _id).FirstOrDefault();
        }
    }
}
