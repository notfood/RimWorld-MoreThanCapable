using HarmonyLib;
using Verse;

namespace MoreThanCapable
{
    [HarmonyPatch(typeof(GameInitData), "PrepForMapGen")]
    static class GameInitData_PrepForMapGen
    {
        static void Postfix(ref GameInitData __instance)
        {
            foreach (Pawn startingPawn in __instance.startingAndOptionalPawns) {
                MoreThanCapableMod.ResetHatedWorkTypes(startingPawn);
            }
        }
    }
}
