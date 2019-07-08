using RimWorld;
using Verse;

namespace MoreThanCapable
{
    public class Alert_BadWorkAssignment : Alert
    {
        public Alert_BadWorkAssignment()
        {
            defaultLabel = "MTC.BadWorkAssignment.Label".Translate();
            defaultExplanation = "MTC.BadWorkAssignment.Explanation".Translate();
            defaultPriority = AlertPriority.High;
        }
        public override AlertReport GetReport()
        {
            foreach (Pawn current in PawnsFinder.AllMaps_FreeColonistsSpawned) {
                if (MoreThanCapableMod.HasBadWork(current)) {
                    return current;
                }
            }
            return false;
        }
    }

    public class Alert_AssignedAWeapon : Alert
    {
        public Alert_AssignedAWeapon()
        {
            defaultLabel = "MTC.AssignedAWeapon.Label".Translate();
            defaultExplanation = "MTC.AssignedAWeapon.Explanation".Translate();
            defaultPriority = AlertPriority.High;
        }
        public override AlertReport GetReport()
        {
            foreach (Pawn current in PawnsFinder.AllMaps_FreeColonistsSpawned) {
                if (current.story.DisabledWorkTypes.Contains(WorkTypeDefOf.Hunting)
                  && ((current.equipment.Primary != null) && MoreThanCapableMod.HasWeapon(current))) {
                    return current;
                }
            }
            return false;
        }
    }
}
