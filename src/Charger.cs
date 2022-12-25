using Vintagestory.API.Common;

[assembly: ModInfo("Charger",
    Authors = new[] { "Craluminum2413" })]

namespace Charger
{
    class Charger : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.RegisterBlockClass("Charger", typeof(BlockCharger));
            api.RegisterBlockClass("SolarPanel", typeof(BlockSolarPanel));
            api.RegisterBlockEntityClass("BECharger", typeof(BlockEntityCharger));
            api.RegisterBlockEntityClass("BESolarPanel", typeof(BlockEntitySolarPanel));
            api.RegisterCollectibleBehaviorClass("DCBattery", typeof(CollectibleBehaviorDCBattery));
            api.RegisterCollectibleBehaviorClass("SolarCell", typeof(CollectibleBehaviorSolarCell));
            api.World.Logger.Event("started 'Charger' mod");
        }
    }
}