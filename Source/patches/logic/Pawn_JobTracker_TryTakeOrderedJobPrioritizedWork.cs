using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoreThanCapable
{
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
}
