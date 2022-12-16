using HarmonyLib;
using RimWorld;
using Verse;

namespace MoreThanCapable
{
    [HarmonyPatch(typeof(Pawn_WorkSettings), nameof(Pawn_WorkSettings.GetPriority))]
    static class Pawn_WorkSettings_GetPriority
    {
        [HarmonyPriority(Priority.VeryLow)]
        static bool Prefix(WorkTypeDef w, Pawn ___pawn, ref int __result)
        {
            __result = 1;

            if (MoreThanCapableMod.Settings.allowFDJ && FloatMenuMakerMap_ChoicesAtFor.executing) {

                return MoreThanCapableMod.CanDisable(___pawn, w);
            }

            return true;
        }
    }
}
