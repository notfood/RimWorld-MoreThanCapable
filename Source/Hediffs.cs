using RimWorld;
using Verse;

namespace MoreThanCapable
{
    public class Hediff_Whine : Hediff
    {
        public bool peaked;
        public override bool Visible => Severity > 0.05;
    }

    public abstract class HediffGiver_AssignedToBad : HediffGiver
    {
        public const float DELTA = 60f;

        public readonly int totalDuration;
        public readonly float maxSeverity;

        protected HediffGiver_AssignedToBad(int totalDuration, float maxSeverity)
        {
            this.totalDuration = totalDuration;
            this.maxSeverity = maxSeverity;
        }

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if (!pawn.IsColonist) {
                return;
            }

            if (!pawn.health.hediffSet.HasHediff(hediff)) {

                // Should we be punished?
                if (Evaluate(pawn)) {
                    TryApply(pawn);

                    return;
                }

                return;
            }

            var whine = (Hediff_Whine) pawn.health.hediffSet.GetFirstHediffOfDef(hediff, false);

            float step = DELTA;

            if (whine.peaked || !Evaluate(pawn)) {
                step *= -1;
            }

            float value = whine.Severity + step * maxSeverity / totalDuration;

            if (value < maxSeverity && value > 0f) {
                whine.Severity = value;
            } else if (value <= 0f) {
                pawn.health.RemoveHediff(whine);
            } else {
                whine.Severity = maxSeverity;

                whine.peaked = true;
            }
        }

        /// <summary>Evaluates pawn for punishment.</summary>
        /// <returns>True if pawn must be punished.</returns>
        /// <param name="pawn">Pawn.</param>
        public abstract bool Evaluate(Pawn pawn);
    }

    public abstract class HediffGiver_AssignedToBadWork : HediffGiver_AssignedToBad
    {
        public WorkTypeDef workType;

        protected HediffGiver_AssignedToBadWork(int totalDuration, float maxSeverity) : base(totalDuration, maxSeverity)
        {
        }

        public override bool Evaluate(Pawn pawn)
        {
            return MoreThanCapableMod.IsBadWorkActive(pawn, workType);
        }
    }

    public class HediffGiver_AssignedToBadWork_Hard : HediffGiver_AssignedToBadWork
    {
        public HediffGiver_AssignedToBadWork_Hard() : base(GenDate.TicksPerDay * 4, 0.5f) { }
    }

    // AssignedToPlantCutting
    // AssignedToHauling
    // AssignedToCleaning
    public class HediffGiver_AssignedToBadWork_Easy : HediffGiver_AssignedToBadWork
    {
        public HediffGiver_AssignedToBadWork_Easy() : base(GenDate.TicksPerDay, 0.175f) { }
    }

    // AssignedAWeapon
    public class HediffGiver_AssignedAWeapon : HediffGiver_AssignedToBad
    {
        public HediffGiver_AssignedAWeapon() : base(GenDate.TicksPerDay * 4, 0.5f) { }

        public override bool Evaluate(Pawn pawn)
        {
            return MoreThanCapableMod.HasWeapon(pawn);
        }
    }
}
