using HarmonyLib;
using RimWorld;

namespace MoreThanCapable
{
    [HarmonyPatch(typeof(SkillRecord), "CalculateTotallyDisabled")]
    static class SkillRecord_CalculateTotallyDisabled
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(SkillRecord), "CalculatePermanentlyDisabled")]
    static class SkillRecord_CalculatePermanentlyDisabled
    {
        public static bool Prefix() => false;
    }
}
