using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace MoreThanCapable
{
    [HarmonyPatch]
    static class CharacterCardUtility_DrawCharacterCard
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(
                AccessTools.Inner(
                    AccessTools.TypeByName("RimWorld.CharacterCardUtility"),
                    "<>c__DisplayClass42_1"
                ), "<DoLeftSection>b__15");
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++) {
                var strOperand = codes[i].operand as string;
                if (strOperand == "IncapableOf") {
                    codes[i].operand = "MTC.IncapableOf";
                    break;
                }
            }
            return codes;
        }
    }
}
