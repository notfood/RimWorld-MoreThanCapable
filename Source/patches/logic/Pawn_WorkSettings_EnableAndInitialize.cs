using HarmonyLib;
using RimWorld;
using Verse;

namespace MoreThanCapable
{
    [HarmonyPatch(typeof(Pawn_WorkSettings), nameof(Pawn_WorkSettings.EnableAndInitialize))]
    static class Pawn_WorkSettings_EnableAndInitialize
    {
        static void Postfix(Pawn ___pawn)
        {
            MoreThanCapableMod.ResetHatedWorkTypes(___pawn);
        }
    }
}
