using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace Charger
{
    public static class SolarPanelUtils
    {
        public static bool IsSolarCell(this ItemStack itemstack) => itemstack.Collectible.HasBehavior<CollectibleBehaviorSolarCell>();
        public static bool IsSolarCell(this CollectibleObject obj) => obj.HasBehavior<CollectibleBehaviorSolarCell>();
        public static float GetEfficiency(this ItemStack itemstack) => itemstack.Attributes.GetAsString("dcb_efficiency").ToFloat();
    }
}