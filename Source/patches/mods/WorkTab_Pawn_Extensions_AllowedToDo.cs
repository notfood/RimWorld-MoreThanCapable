using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace MoreThanCapable
{
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
