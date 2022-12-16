using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace MoreThanCapable
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    static class FloatMenuMakerMap_AddHumanlikeOrders
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            int start = matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldstr, "CannotEquip")
            ).Pos;
            int end = matcher.MatchStartForward(
                new CodeMatch(OpCodes.Br)
            ).Pos;
            return matcher.RemoveInstructionsInRange(start, end).Instructions();
        }
    }
}