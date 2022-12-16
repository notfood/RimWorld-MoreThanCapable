using HarmonyLib;
using RimWorld;
using Verse;

namespace MoreThanCapable
{
    [HarmonyPatch(typeof(PawnGenerator), "GenerateSkills")]
    static class PawnGenerator_GenerateSkills
    {
        static void Postfix(Pawn pawn)
        {
            if (IsTagDisabled(pawn, WorkTags.Animals)) {
                Disable(pawn, SkillDefOf.Animals);
            }
            if (IsTagDisabled(pawn, WorkTags.Artistic)) {
                Disable(pawn, SkillDefOf.Artistic);
            }
            if (IsTagDisabled(pawn, WorkTags.Caring)) {
                Disable(pawn, SkillDefOf.Medicine);
            }
            if (IsTagDisabled(pawn, WorkTags.Cooking)) {
                Disable(pawn, SkillDefOf.Cooking);
            }
            if (IsTagDisabled(pawn, WorkTags.Crafting)) {
                Disable(pawn, SkillDefOf.Crafting);
            }
            if (IsTagDisabled(pawn, WorkTags.Intellectual)) {
                Disable(pawn, SkillDefOf.Intellectual);
            }
            if (IsTagDisabled(pawn, WorkTags.ManualSkilled)) {
                Disable(pawn, SkillDefOf.Construction);
                Disable(pawn, SkillDefOf.Cooking);
                Disable(pawn, SkillDefOf.Crafting);
                Disable(pawn, SkillDefOf.Plants);
                Disable(pawn, SkillDefOf.Mining);
            }
            if (IsTagDisabled(pawn, WorkTags.Mining)) {
                Disable(pawn, SkillDefOf.Mining);
            }
            if (IsTagDisabled(pawn, WorkTags.PlantWork)) {
                Disable(pawn, SkillDefOf.Plants);
            }
            if (IsTagDisabled(pawn, WorkTags.Social)) {
                Disable(pawn, SkillDefOf.Social);
            }
            if (IsTagDisabled(pawn, WorkTags.Violent)) {
                Disable(pawn, SkillDefOf.Melee);
                Disable(pawn, SkillDefOf.Shooting);
            }
        }

        static bool IsTagDisabled(Pawn pawn, WorkTags workTag)
        {
            return (pawn.CombinedDisabledWorkTags & workTag) != WorkTags.None;
        }

        static void Disable(Pawn pawn, SkillDef skill)
        {
            pawn.skills.GetSkill(skill).passion = Passion.None;
        }
    }
}
