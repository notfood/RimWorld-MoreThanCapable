using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;

namespace MoreThanCapable
{
    [HarmonyPatch(typeof(FloatMenuUtility), nameof(FloatMenuUtility.GetMeleeAttackAction))]
    static class FloatMenuUtility_GetMeleeAttackAction
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++) {
                var strOperand = codes[i].operand as string;
                if (strOperand == "IsIncapableOfViolenceLower") {
                    codes[i].operand = "MTC.IsIncapableOfViolenceLower";
                }
            }
            return codes.AsEnumerable();
        }
    }
}
