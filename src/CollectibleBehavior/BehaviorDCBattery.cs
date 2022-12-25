using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace Charger
{
    public class CollectibleBehaviorDCBattery : CollectibleBehavior
    {
        public CollectibleBehaviorDCBattery(CollectibleObject collObj) : base(collObj) { }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            dsc.Append(Lang.Get("charger:description-chargeAndCapacity", inSlot.Itemstack.GetBatteryCharge(), inSlot.Itemstack.GetBatteryCapacity()));
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
        }
    }
}