using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace Charger
{
    public static class ModUtils
    {
        public static Vec4f Vec4fRed => new(1, 0, 0, 1);
        public static Vec4f Vec4fGreen => new(0, 1, 0, 1);
        public static Vec4f Vec4fBlue => new(0, 0, 1, 1);
        public static Vec4f Vec4fYellow => new(1, 1, 0, 1);
        public static string HexRed => "#FF0000";
        public static string HexGreen => "#00FF00";

        public static Vec4f SubXYZ(this Vec4f vector, float value) => new(vector.X - value, vector.Y - value, vector.Z - value, vector.W);
        public static T IfOddOrEven<T>(this int value, T ifOdd, T ifEven) => value % 2 != 0 ? ifOdd : ifEven;

        public static float To3FloatPoints(this float value) => string.Format("{0:0.###}", value).ToFloat();

        public static float GetSpeedOfTime(this ICoreAPI api) => api.World.Calendar.SpeedOfTime / 60;

        public static string GetSlotName(this ItemSlot slot)
        {
            if (slot is ItemSlotDischarging) return Lang.Get("charger:slot-discharging");
            if (slot is ItemSlotCharging) return Lang.Get("charger:slot-charging");
            if (slot is ItemSlotSolarCell) return Lang.Get("charger:slot-solarcell");
            return Lang.Get("Slot");
        }

        public static WorldInteraction[] GePowerButtonInteractions()
        {
            return new WorldInteraction[] {
                new WorldInteraction()
                {
                    ActionLangCode = string.Format("{0}/{1}", Lang.Get("On"), Lang.Get("Off")),
                    MouseButton = EnumMouseButton.Right,
                }
            };
        }
    }
}