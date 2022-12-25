using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using static Charger.ModUtils;

namespace Charger
{
    public class BlockSolarPanel : Block
    {
        // public override bool DoParticalSelection(IWorldAccessor world, BlockPos pos) => true;

        public override Vec4f GetSelectionColor(ICoreClientAPI capi, BlockPos pos)
        {
            var i = capi.World.Player.CurrentBlockSelection.SelectionBoxIndex;
            return i switch
            {
                0 => new Vec4f(1, 1, 1, 1), // White
                1 => new Vec4f(1, 0, 1, 1), // Magenta
                2 => Vec4fGreen, // Green
                _ => Vec4fYellow
            };
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            if (world.BlockAccessor.GetBlockEntity(blockSel.Position) is not BlockEntitySolarPanel besp) return false;
            var i = blockSel.SelectionBoxIndex;
            return i switch
            {
                0 => base.OnBlockInteractStart(world, byPlayer, blockSel),
                1 => besp.EnableOrDisable(),
                2 => besp.TryPut(byPlayer, besp.inventory.Count - 1) || besp.TryTake(byPlayer, besp.inventory.Count - 1),
                _ => besp.TryPut(byPlayer, i - 3) || besp.TryTake(byPlayer, i - 3)
            };
        }
    }
}