using System.Collections.Generic;
using HarmonyLib;
using RimWorld;

namespace MoreThanCapable
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddJobGiverWorkOrders")]
    static class FloatMenuMakerMap_AddUndraftedOrders
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++) {
                var strOperand = codes[i].operand as string;
                if (strOperand == "CannotPrioritizeWorkGiverDisabled") {
                    codes[i].operand = "MTC.CannotPrioritizeWorkGiverDisabled";
                }
                if (strOperand == "CannotPrioritizeNotAssignedToWorkType") {
                    codes[i].operand = "MTC.CannotPrioritizeNotAssignedToWorkType";
                }
            }
            return codes;
        }
    }
}
