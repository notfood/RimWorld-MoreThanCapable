using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MoreThanCapable
{
    [HarmonyPatch(typeof(WidgetsWork), nameof(WidgetsWork.TipForPawnWorker))]
    static class WidgetsWork_TipForPawnWorker
    {
        public static bool Prefix(Pawn p, WorkTypeDef wDef, ref string __result)
        {
            if (MoreThanCapableMod.CanDisable(p, wDef)) {
                return true;
            }
            if (p.WorkTypeIsDisabled(wDef)) {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(wDef.gerundLabel.CapitalizeFirst());
                stringBuilder.Append("MTC.CannotDoThisWork".Translate(p.LabelShort));
                __result = stringBuilder.ToString();
                return false;
            }
            return true;
        }
    }
}
