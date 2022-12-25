using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using static Charger.ChargerUtils;
using static Charger.ModUtils;

namespace Charger
{
    public class BlockCharger : Block
    {
        WorldInteraction[] redInteractions;
        WorldInteraction[] greenInteractions;
        WorldInteraction[] powerButtonInteractions;

        public override bool DoParticalSelection(IWorldAccessor world, BlockPos pos) => true;

        public override Vec4f GetSelectionColor(ICoreClientAPI capi, BlockPos pos)
        {
            var i = capi.World.Player.CurrentBlockSelection.SelectionBoxIndex;
            return i switch
            {
                0 => new Vec4f(1, 1, 1, 1), // White
                1 => new Vec4f(1, 0, 1, 1), // Magenta
                _ => i.IfOddOrEven(ifOdd: Vec4fGreen, ifEven: Vec4fRed)
            };
        }

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);

            if (api.Side != EnumAppSide.Client) return;
            redInteractions = GetRedInteractions(api);
            greenInteractions = GetGreenInteractions(api);
            powerButtonInteractions = GePowerButtonInteractions();
        }

        public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
        {
            var original = base.GetPlacedBlockInteractionHelp(world, selection, forPlayer);
            var i = selection.SelectionBoxIndex;
            var extraInteractions = i switch
            {
                0 => Enumerable.Empty<WorldInteraction>(),
                1 => powerButtonInteractions,
                _ => i.IfOddOrEven(ifOdd: greenInteractions, ifEven: redInteractions)
            };
            return original.Concat(extraInteractions).ToArray();
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            if (world.BlockAccessor.GetBlockEntity(blockSel.Position) is not BlockEntityCharger bec) return false;
            var i = blockSel.SelectionBoxIndex;
            return i switch
            {
                0 => base.OnBlockInteractStart(world, byPlayer, blockSel),
                1 => bec.EnableOrDisable(),
                _ => bec.TryPut(byPlayer, i - 2) || bec.TryTake(byPlayer, i - 2)
            };
        }
    }
}