using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace Charger
{
    public class CollectibleBehaviorSolarCell : CollectibleBehavior
    {
        public CollectibleBehaviorSolarCell(CollectibleObject collObj) : base(collObj) { }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            dsc.Append(Lang.Get("charger:description-efficiency", inSlot.Itemstack.GetEfficiency()));
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
        }
    }
}