using HarmonyLib;
using RimWorld;

namespace MoreThanCapable
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), nameof(FloatMenuMakerMap.ChoicesAtFor))]
    internal static class FloatMenuMakerMap_ChoicesAtFor
    {
        internal static bool executing;

        [HarmonyPriority(Priority.VeryLow)]
        static void Prefix() => executing = true;

        [HarmonyPriority(Priority.VeryHigh)]
        static void Postfix() => executing = false;
    }
}
