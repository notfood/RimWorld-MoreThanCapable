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
            return AlertReport.CulpritsAre(PawnsFinder.AllMaps_FreeColonistsSpawned.FindAll(MoreThanCapableMod.HasBadWork));
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
            return AlertReport.CulpritsAre(PawnsFinder.AllMaps_FreeColonistsSpawned.FindAll(MoreThanCapableMod.IsNonViolentArmed));
        }
    }
}
