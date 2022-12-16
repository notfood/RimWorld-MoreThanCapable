using HarmonyLib;
using Verse;

namespace MoreThanCapable
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.WorkTypeIsDisabled))]
    static class Pawn_WorkTypeIsDisabled
    {
        public static bool Prefix(Pawn __instance, WorkTypeDef w)
        {
            return MoreThanCapableMod.CanDisable(__instance, w);
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.WorkTagIsDisabled))]
    static class Pawn_WorkTagIsDisabled
    {
        public static bool Prefix(Pawn __instance, WorkTags w)
        {
            return MoreThanCapableMod.CanDisable(__instance, w);
        }
    }
}
