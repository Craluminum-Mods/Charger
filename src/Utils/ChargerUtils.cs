using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace Charger
{
    public static class ChargerUtils
    {
        public static bool IsDCBattery(this ItemStack itemstack) => itemstack.Collectible.HasBehavior<CollectibleBehaviorDCBattery>();
        public static bool IsDCBattery(this CollectibleObject obj) => obj.HasBehavior<CollectibleBehaviorDCBattery>();
        public static bool IsEmptyBattery(this ItemStack itemstack) => itemstack.GetBatteryCharge() == 0;
        public static bool IsFullBattery(this ItemStack itemstack) => itemstack.GetBatteryCharge() == itemstack.GetBatteryCapacity();
        public static float GetBatteryCharge(this ItemStack itemstack) => itemstack.Attributes.GetAsString("dcb_charge").ToFloat();
        public static float GetBatteryCapacity(this ItemStack itemstack) => itemstack.Attributes.GetAsString("dcb_capacity").ToFloat();

        public static WorldInteraction[] GetRedInteractions(ICoreAPI api)
        {
            return ObjectCacheUtil.GetOrCreate(api, "chargerBlockInteractions-red", () =>
            {
                var batteryStacks = new List<ItemStack>();

                foreach (var obj in api.World.Collectibles)
                {
                    if (IsDCBattery(obj)) batteryStacks.Add(new ItemStack(obj));
                }

                return new WorldInteraction[] {
                    new WorldInteraction()
                    {
                        ActionLangCode = "charger:blockhelp-charger-addToDischargingSlot",
                        MouseButton = EnumMouseButton.Right,
                        Itemstacks = batteryStacks.ToArray()
                    },
                    new WorldInteraction()
                    {
                        ActionLangCode = "charger:blockhelp-charger-removeFromDischargingSlot",
                        MouseButton = EnumMouseButton.Right
                    }
                };
            });
        }

        public static WorldInteraction[] GetGreenInteractions(ICoreAPI api)
        {
            return ObjectCacheUtil.GetOrCreate(api, "chargerBlockInteractions-green", () =>
            {
                var batteryStacks = new List<ItemStack>();

                foreach (var obj in api.World.Collectibles)
                {
                    if (IsDCBattery(obj)) batteryStacks.Add(new ItemStack(obj));
                }

                return new WorldInteraction[] {
                    new WorldInteraction()
                    {
                        ActionLangCode = "charger:blockhelp-charger-addToChargingSlot",
                        MouseButton = EnumMouseButton.Right,
                        Itemstacks = batteryStacks.ToArray()
                    },
                    new WorldInteraction()
                    {
                        ActionLangCode = "charger:blockhelp-charger-removeFromChargingSlot",
                        MouseButton = EnumMouseButton.Right
                    }
                };
            });
        }
    }
}