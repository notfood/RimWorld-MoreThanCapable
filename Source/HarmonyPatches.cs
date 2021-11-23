using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreThanCapable
{
    [HarmonyPatch(typeof(CharacterCardUtility), nameof(CharacterCardUtility.DrawCharacterCard))]
    static class CharacterCardUtility_DrawCharacterCard
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++) {
                var strOperand = codes[i].operand as string;
                if (strOperand == "IncapableOf") {
                    codes[i].operand = "MTC.IncapableOf";
                }
            }
            return codes;
        }
    }

    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    static class FloatMenuMakerMap_AddHumanlikeOrders
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            int start = matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldstr, "CannotEquip")
            ).Pos;
            int end = matcher.MatchForward(false,
                new CodeMatch(OpCodes.Br)
            ).Pos;
            return matcher.RemoveInstructionsInRange(start, end).Instructions();
        }
    }

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

    [HarmonyPatch(typeof(Pawn_WorkSettings), nameof(Pawn_WorkSettings.GetPriority))]
    static class Pawn_WorkSettings_GetPriority
    {
        [HarmonyPriority(Priority.VeryLow)]
        static bool Prefix(ref int __result)
        {
            __result = 1;

            return !MoreThanCapableMod.Settings.allowFDJ || !FloatMenuMakerMap_ChoicesAtFor.skip;
        }
    }

    [HarmonyPatch(typeof(FloatMenuMakerMap), nameof(FloatMenuMakerMap.ChoicesAtFor))]
    internal static class FloatMenuMakerMap_ChoicesAtFor
    {
        internal static bool skip;

        [HarmonyPriority(Priority.VeryLow)]
        static void Prefix()
        {
            skip = true;
        }
        [HarmonyPriority(Priority.VeryHigh)]
        static void Postfix()
        {
            skip = false;
        }
    }

    [HarmonyPatch(typeof(Pawn_JobTracker), "TryTakeOrderedJobPrioritizedWork")]
    static class Pawn_JobTracker_TryTakeOrderedJobPrioritizedWork
    {
        static void Postfix(WorkGiver giver, Pawn ___pawn, bool __result)
        {
            if (!__result) {
                return;
            }

            var workType = giver.def.workType;

            if (!MoreThanCapableMod.IsBadWork(___pawn, workType)) {
                return;
            }

            var badWork = ___pawn.RaceProps.hediffGiverSets
                .SelectMany(s => s.hediffGivers)
                .OfType<HediffGiver_AssignedToBadWork>()
                .FirstOrDefault(bw => bw.workType == workType);

            if (badWork == null) {
                return;
            }

            AdjustSeverity(___pawn, badWork.hediff, 0.025f, badWork.maxSeverity/2f);
        }

        static void AdjustSeverity(Pawn pawn, HediffDef hdDef, float sevOffset, float sevMax)
        {
            if (sevOffset != 0f) {
                Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hdDef, false);
                if (firstHediffOfDef != null && firstHediffOfDef.Severity < sevMax) {
                    firstHediffOfDef.Severity += sevOffset;
                } else if (sevOffset > 0f) {
                    firstHediffOfDef = HediffMaker.MakeHediff(hdDef, pawn, null);
                    firstHediffOfDef.Severity = sevOffset;
                    pawn.health.AddHediff(firstHediffOfDef, null, null, null);
                }
            }
        }
    }

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

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.WorkTagIsDisabled))]
    static class Pawn_WorkTagIsDisabled
    {
        public static bool Prefix(WorkTags w, Pawn __instance)
        {
            return !__instance.NonHumanlikeOrWildMan() && w == WorkTags.Violent && !MoreThanCapableMod.HasWeapon(__instance);
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.WorkTypeIsDisabled))]
    static class Pawn_StoryTracker_WorkTypeIsDisabled
    {
        public static bool Prefix(WorkTypeDef w)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(Pawn_WorkSettings), nameof(Pawn_WorkSettings.EnableAndInitialize))]
    static class Pawn_WorkSettings_EnableAndInitialize
    {
        static void Postfix(Pawn ___pawn)
        {
            MoreThanCapableMod.ResetHatedWorkTypes(___pawn);
        }
    }

    [HarmonyPatch(typeof(SkillRecord), "CalculateTotallyDisabled")]
    static class SkillRecord_CalculateTotallyDisabled
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(WidgetsWork), "DrawWorkBoxBackground")]
    static class WidgetsWork_DrawWorkBoxBackground
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var target = AccessTools.Method(typeof(GUI), nameof(GUI.DrawTexture), new [] {typeof(Rect), typeof(Texture)});
            var insert = AccessTools.Method(typeof(WidgetsWork_DrawWorkBoxBackground), nameof(RedIfBadWork));

            var codes = new List<CodeInstruction>(instructions);
            
            // ldarg.0 <- can't use, it's jump target
	        // ldloc.1
	        // call GUI.DrawTexture
            int index = codes.FirstIndexOf(c => target.Equals(c.operand)) - 1;
            codes.InsertRange(index, new[] {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldloca, 1),
                new CodeInstruction(OpCodes.Ldloca, 2),
                new CodeInstruction(OpCodes.Call, insert),
                new CodeInstruction(OpCodes.Ldarg_0),
            });

            return codes;
        }

        static void RedIfBadWork(Rect position, Pawn p, WorkTypeDef workDef, ref Texture workBoxBGTexAwful, ref Texture workBoxBGTexBad)
        {
            if (MoreThanCapableMod.IsBadWork(p, workDef))
            {
                workBoxBGTexAwful = Resources.workBoxBGTexDespised;
                workBoxBGTexBad = Resources.workBoxBGTexDespised;
            }
        }
    }

    [HarmonyPatch(typeof(WidgetsWork), nameof(WidgetsWork.TipForPawnWorker))]
    static class WidgetsWork_TipForPawnWorker
    {
        public static bool Prefix(Pawn p, WorkTypeDef wDef, ref string __result)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(wDef.gerundLabel.CapitalizeFirst());
            if (p.WorkTypeIsDisabled(wDef)) {
                stringBuilder.Append("MTC.CannotDoThisWork".Translate(p.LabelShort));
                __result = stringBuilder.ToString();
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(WITab_Caravan_Gear), "TryEquipDraggedItem")]
    static class WITab_Caravan_Gear_TryEquipDraggedItem
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            int start = matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldstr, "MessageCantEquipIncapableOfViolence")
            ).Pos;
            int end = matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ret)
            ).Pos;
            return matcher.RemoveInstructionsInRange(start, end).Instructions();
        }
    }

    [HarmonyPatch]
    static class WorkTab_Pawn_Extensions_AllowedToDo
    {
        static MethodBase target;
        static bool Prepare()
        {
            var mod = LoadedModManager.RunningMods.FirstOrDefault(m => m.Name == "Work Tab");
            if (mod == null) {
                return false;
            }
            var type = mod.assemblies.loadedAssemblies.FirstOrDefault(a => a.GetName().Name == "WorkTab").GetType("WorkTab.Pawn_Extensions");
            if (type == null) {
                Log.Warning("MoreThanCapable :: Can't patch WorkTab; no Pawn_Extensions found!");
                return false;
            }
            target = AccessTools.DeclaredMethod(type, "AllowedToDo");
            if (target == null) {
                Log.Warning("MoreThanCapable :: Can't patch WorkTab; no Pawn_Extensions.AllowedToDo found!");
                return false;
            }
            return true;
        }
        static MethodBase TargetMethod()
        {
            return target;
        }
        static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }
}
