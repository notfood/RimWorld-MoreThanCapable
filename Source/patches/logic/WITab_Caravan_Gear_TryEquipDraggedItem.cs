using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld.Planet;

namespace MoreThanCapable
{
    [HarmonyPatch(typeof(WITab_Caravan_Gear), "TryEquipDraggedItem")]
    static class WITab_Caravan_Gear_TryEquipDraggedItem
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            int start = matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldstr, "MessageCantEquipIncapableOfViolence")
            ).Pos;
            int end = matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ret)
            ).Pos;
            return matcher.RemoveInstructionsInRange(start, end).Instructions();
        }
    }
}
