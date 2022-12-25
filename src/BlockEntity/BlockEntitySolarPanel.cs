using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Charger
{
    // BlockEntityDisplay
    public class BlockEntitySolarPanel : BlockEntityContainer
    {
        public bool isEnabled;
        internal InventorySolarPanel inventory;

        // Matrixf mat = new();

        // private readonly List<Vec4f> _translations = new()
        // {
        //     new Vec4f(.3125f, .25f, .875f, 0),
        //     new Vec4f(.4375f, .25f, .875f, 0),
        //     new Vec4f(.5625f, .25f, .875f, 0),
        //     new Vec4f(.6875f, .25f, .875f, 0),

        //     new Vec4f(.875f, .25f, .6875f, 0),
        //     new Vec4f(.875f, .25f, .5625f, 0),
        //     new Vec4f(.875f, .25f, .4375f, 0),
        //     new Vec4f(.875f, .25f, .3125f, 0),

        //     new Vec4f(.6875f, .25f, .125f, 0),
        //     new Vec4f(.5625f, .25f, .125f, 0),
        //     new Vec4f(.4375f, .25f, .125f, 0),
        //     new Vec4f(.3125f, .25f, .125f, 0),

        //     new Vec4f(.125f, .25f, .3125f, 0),
        //     new Vec4f(.125f, .25f, .4375f, 0),
        //     new Vec4f(.125f, .25f, .5625f, 0),
        //     new Vec4f(.125f, .25f, .6875f, 0),
        // };

        public override InventoryBase Inventory => inventory;
        public override string InventoryClassName => "solarpanel";

        // public override string AttributeTransformCode => "onSolarPanelTransform";

        public BlockEntitySolarPanel()
        {
            isEnabled = false;
            inventory = new InventorySolarPanel("solarpanel-1", Api);
            // meshes = new MeshData[1];
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);

            inventory.LateInitialize("solarpanel-1", api);

            RegisterGameTickListener(OnGameTick, 1000);
        }

        private void OnGameTick(float delta)
        {
            if (Api.Side.IsClient() || !isEnabled) return;
            var receiverSlot = inventory?[25];
            if (receiverSlot.Empty) return;
            if (receiverSlot.Itemstack.IsFullBattery()) return;

            float totalOutput = 0;
            float sunlight = Api.World.BlockAccessor.GetLightLevel(Pos, EnumLightLevelType.OnlySunLight) / 22;
            sunlight = sunlight.To3FloatPoints();
            float daylight = Api.World.Calendar.GetDayLightStrength(Pos).To3FloatPoints();
            sunlight *= daylight * daylight;

            foreach (var slot in inventory)
            {
                if (slot is ItemSlotCharging || slot.Empty) continue;
                totalOutput += (sunlight * slot.Itemstack.GetEfficiency()).To3FloatPoints();
            }

            receiverSlot.Itemstack.Attributes.SetFloat("dcb_charge", (receiverSlot.Itemstack.GetBatteryCharge() + (Api.GetSpeedOfTime() * totalOutput)).To3FloatPoints());
            receiverSlot.MarkDirty();
            MarkDirty();
        }

        public bool TryPut(IPlayer byPlayer, int toSlotId)
        {
            var toSlot = inventory[toSlotId];
            var fromSlot = byPlayer.InventoryManager.ActiveHotbarSlot;

            if (fromSlot.Itemstack == null) return false;
            if (toSlot.StackSize > 0) return false;
            if (toSlot is ItemSlotSolarCell && !fromSlot.Itemstack.IsSolarCell()) return false;
            if (toSlot is ItemSlotCharging && !fromSlot.Itemstack.IsDCBattery()) return false;

            fromSlot.TryPutInto(Api.World, toSlot);
            toSlot.MarkDirty();
            fromSlot.MarkDirty();
            // updateMeshes();
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
            // updateMeshes();
            MarkDirty(true);
            return true;
        }

        // public override void TranslateMesh(MeshData mesh, int index)
        // {
        //     var offset = mat.TransformVector(_translations[index].SubXYZ(0.5f));
        //     mesh.Translate(offset.XYZ);
        // }

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

            if (!(selBoxIndex is 0 or 1 or 2))
            {
                var slot = inventory?[selBoxIndex - 3];

                dsc.Append(slot.GetSlotName())
                    .AppendFormat(" [" + inventory.GetSlotId(slot) + "]: ")
                    .Append(slot.Empty
                        ? Lang.Get("Empty")
                        : slot.GetStackName() + ": " + Lang.Get("charger:description-efficiency", slot.Itemstack.GetEfficiency()));
            }
            if (selBoxIndex is 2)
            {
                var slot = inventory?[25];

                dsc.Append(slot.GetSlotName())
                    .AppendFormat(" [" + inventory.GetSlotId(slot) + "]: ")
                    .Append(slot.Empty
                        ? Lang.Get("Empty")
                        : slot.Itemstack.GetBatteryCharge() + "/" + slot.Itemstack.GetBatteryCapacity());
            }

            float totalOutput = 0;
            float sunlight = Api.World.BlockAccessor.GetLightLevel(Pos, EnumLightLevelType.OnlySunLight) / 22;
            float daylight = Api.World.Calendar.GetDayLightStrength(Pos);
            sunlight *= daylight * daylight;

            foreach (var slot in inventory)
            {
                if (slot is ItemSlotCharging || slot.Empty) continue;
                totalOutput += sunlight * slot.Itemstack.GetEfficiency();
            }

            dsc.AppendLine().AppendFormat("Output: {0}", (Api.GetSpeedOfTime() * totalOutput).To3FloatPoints());
            dsc.AppendLine().AppendFormat("Sunlight: {0}", sunlight.To3FloatPoints());
        }
    }
}