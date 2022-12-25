using Vintagestory.API.Common;
using static Charger.ModUtils;

namespace Charger
{
    /// <summary>
    /// A slot that can only hold batteries
    /// </summary>
    public class ItemSlotCharging : ItemSlot
    {
        public ItemSlotCharging(InventoryBase inventory) : base(inventory)
        {
            // BackgroundIcon = "lightning";
            HexBackgroundColor = HexGreen;
        }

        public override int MaxSlotStackSize { get; set; } = 1;
        public override bool CanHold(ItemSlot sourceSlot) => sourceSlot.Itemstack.Collectible.HasBehavior<CollectibleBehaviorDCBattery>();
    }
}