using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Charger
{
    public class BlockEntityCharger : BlockEntityDisplay
    {
        public bool isEnabled;
        internal InventoryCharger inventory;

        Matrixf mat = new();

        private readonly List<Vec4f> _translations = new()
        {
            new Vec4f(.3125f, .25f, .875f, 0),
            new Vec4f(.4375f, .25f, .875f, 0),
            new Vec4f(.5625f, .25f, .875f, 0),
            new Vec4f(.6875f, .25f, .875f, 0),

            new Vec4f(.875f, .25f, .6875f, 0),
            new Vec4f(.875f, .25f, .5625f, 0),
            new Vec4f(.875f, .25f, .4375f, 0),
            new Vec4f(.875f, .25f, .3125f, 0),

            new Vec4f(.6875f, .25f, .125f, 0),
            new Vec4f(.5625f, .25f, .125f, 0),
            new Vec4f(.4375f, .25f, .125f, 0),
            new Vec4f(.3125f, .25f, .125f, 0),

            new Vec4f(.125f, .25f, .3125f, 0),
            new Vec4f(.125f, .25f, .4375f, 0),
            new Vec4f(.125f, .25f, .5625f, 0),
            new Vec4f(.125f, .25f, .6875f, 0),
        };

        public override InventoryBase Inventory => inventory;
        public override string InventoryClassName => "charger";

        public override string AttributeTransformCode => "onChargerTransform";

        public BlockEntityCharger()
        {
            isEnabled = false;
            inventory = new InventoryCharger("charger-1", Api);
            meshes = new MeshData[16];
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);

            inventory.LateInitialize("charger-1", api);

            RegisterGameTickListener(OnGameTick, 100);
        }

        private void OnGameTick(float delta)
        {
            var transferValue = 0.001f;

            if (Api.Side.IsClient() || !isEnabled) return;
            if (inventory.Count % 2 != 0) return;
            for (var i = 0; i < inventory.Count; i += 2)
            {
                var senderSlot = inventory[i];
                var receiverSlot = inventory[i + 1];
                if (senderSlot.Empty || receiverSlot.Empty) continue;
                if (senderSlot.Itemstack.IsEmptyBattery() || receiverSlot.Itemstack.IsFullBattery()) continue;
                senderSlot.Itemstack.Attributes.SetFloat("dcb_charge", senderSlot.Itemstack.GetBatteryCharge().To3FloatPoints() - transferValue);
                receiverSlot.Itemstack.Attributes.SetFloat("dcb_charge", receiverSlot.Itemstack.GetBatteryCharge().To3FloatPoints() + transferValue);
                senderSlot.MarkDirty();
                receiverSlot.MarkDirty();
                MarkDirty();
            }
        }

        public bool TryPut(IPlayer byPlayer, int toSlotId)
        {
            var toSlot = inventory[toSlotId];
            var fromSlot = byPlayer.InventoryManager.ActiveHotbarSlot;

            if (fromSlot.Itemstack == null) return false;
            if (toSlot.StackSize > 0) return false;
            if (!fromSlot.Itemstack.IsDCBattery()) return false;

            fromSlot.TryPutInto(Api.World, toSlot);
            toSlot.MarkDirty();
            fromSlot.MarkDirty();
            updateMeshes();
            MarkDirty(true);
            return true;
        }

        public bool TryTake(IPlayer byPlayer, int fromSlotId)
        {
            var fromSlot = inventory[fromSlotId];
            var toSlot = byPlayer.InventoryManager.ActiveHotbarSlot;

            if (fromSlot.Itemstack == null) return false;
            if (fromSlot.StackSize < 0) return false;

            fromSlot.TryPutInto(Api.World, toSlot);
            toSlot.MarkDirty();
            fromSlot.MarkDirty();
            updateMeshes();
            MarkDirty(true);
            return true;
        }

        public override void TranslateMesh(MeshData mesh, int index)
        {
            var offset = mat.TransformVector(_translations[index].SubXYZ(0.5f));
            mesh.Translate(offset.XYZ);
        }

        public bool EnableOrDisable()
        {
            isEnabled = !isEnabled;
            MarkDirty();
            return true;
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor world)
        {
            base.FromTreeAttributes(tree, world);
            isEnabled = tree.GetBool("isEnabled");
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetBool("isEnabled", isEnabled);
        }

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            dsc.Append("State: ").AppendLine(isEnabled ? Lang.Get("On") : Lang.Get("Off"));

            var selBoxIndex = forPlayer.CurrentBlockSelection.SelectionBoxIndex;

            if (!(selBoxIndex is 0 or 1))
            {
                var slot = inventory?[selBoxIndex - 2];

                dsc.Append(slot.GetSlotName())
                    .AppendFormat(" [" + inventory.GetSlotId(slot) + "]: ")
                    .AppendLine(slot.Empty
                    ? Lang.Get("Empty")
                    : slot.Itemstack.GetBatteryCharge() + "/" + slot.Itemstack.GetBatteryCapacity());
            }
        }
    }
}