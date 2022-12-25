using System;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Charger
{
    public class InventorySolarPanel : InventoryBase, ISlotProvider
    {
        ItemSlot[] slots;

        public ItemSlot[] Slots => slots;
        public override int Count => slots.Length;

        public InventorySolarPanel(string inventoryID, ICoreAPI api) : base(inventoryID, api)
        {
            slots = GenEmptySlots(26);
            // baseWeight = 4;
        }

        public override ItemSlot GetAutoPullFromSlot(BlockFacing atBlockFace) => null;
        public override ItemSlot GetAutoPushIntoSlot(BlockFacing atBlockFace, ItemSlot fromSlot) => null;

        protected override ItemSlot NewSlot(int slotId)
        {
            return slotId is 25 ? new ItemSlotCharging(this) : new ItemSlotSolarCell(this);
        }

        public override ItemSlot this[int slotId]
        {
            get => slotId < 0 || slotId >= Count ? null : slots[slotId];
            set
            {
                if (slotId < 0 || slotId >= Count) throw new ArgumentOutOfRangeException(nameof(slotId));
                slots[slotId] = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public override void FromTreeAttributes(ITreeAttribute tree) => slots = SlotsFromTreeAttributes(tree);
        public override void ToTreeAttributes(ITreeAttribute tree) => SlotsToTreeAttributes(slots, tree);
    }
}