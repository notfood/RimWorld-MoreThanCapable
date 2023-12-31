using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MoreThanCapable
{
    /*
     * Right click -> Orders, ignore GetPriority unless...
     */
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddJobGiverWorkOrders")]
    internal static class FloatMenuMakerMap_AddJobGiverWorkOrders
    {
        static bool Prepare()
        {
            return MoreThanCapableMod.Settings.allowFDJ;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var target = AccessTools.Method(typeof(Pawn_WorkSettings), nameof(Pawn_WorkSettings.GetPriority));
            var replacement = AccessTools.Method(typeof(FloatMenuMakerMap_AddJobGiverWorkOrders), nameof(GetPriority));

            foreach (var code in instructions)
            {
	            if (code.opcode == OpCodes.Callvirt && target.Equals(code.operand))
	            {
		            code.opcode = OpCodes.Call;
		            code.operand = replacement;
	            }

	            yield return code;
            }
        }

        static int GetPriority(Pawn pawn, WorkTypeDef wtd)
        {
            if (MoreThanCapableMod.CanDisable(pawn, wtd)) {
                return pawn.workSettings.GetPriority(wtd);
            }
            return 1;
        }
    }
}
