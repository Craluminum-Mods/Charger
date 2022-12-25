using Vintagestory.API.Common;
using static Charger.ModUtils;

namespace Charger
{
    /// <summary>
    /// A slot that can only store solar cells
    /// </summary>
    public class ItemSlotSolarCell : ItemSlot
    {
        public ItemSlotSolarCell(InventoryBase inventory) : base(inventory)
        {
            // BackgroundIcon = "lightning";
            HexBackgroundColor = HexGreen;
        }

        public override int MaxSlotStackSize { get; set; } = 1;
        public override bool CanHold(ItemSlot sourceSlot) => sourceSlot.Itemstack.Collectible.HasBehavior<CollectibleBehaviorSolarCell>();
    }
}