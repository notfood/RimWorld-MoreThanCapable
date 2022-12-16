using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoreThanCapable
{
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
}
