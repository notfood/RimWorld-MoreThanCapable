using System.Linq;
using RimWorld;
using Verse;

namespace MoreThanCapable
{
    public class ThoughtWorker_AssignedToBadWork : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.health.hediffSet.HasHediff(def.hediff)) {
                return false;
            }

            var whine = (Hediff_Whine) p.health.hediffSet.GetFirstHediffOfDef(def.hediff);

            float severity = whine.Severity;

            if (whine.peaked) {
                if (severity > 0f) {
                    return ThoughtState.ActiveAtStage(def.stages.Count - 1);
                }

                return false;
            }

            if (severity > 0.4f) {
                return ThoughtState.ActiveAtStage(4);
            }
            if (severity > 0.3f) {
                return ThoughtState.ActiveAtStage(3);
            }
            if (severity > 0.2f) {
                return ThoughtState.ActiveAtStage(2);
            }
            if (severity > 0.1f) {
                return ThoughtState.ActiveAtStage(1);
            }
            if (severity > 0f) {
                return ThoughtState.ActiveAtStage(0);
            }

            return false;
        }
    }
}
