using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ColorfulSlimes
{
    [HarmonyPatch(typeof(SplatOnImpact))]
    [HarmonyPatch("ProcessCollisionEnter")]
    internal static class AppearancePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach (var v in instr)
            {
                if (v.opcode == OpCodes.Callvirt && (v.operand as MethodInfo).Name == "GetAppearancePalette")
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AppearancePatch), "Replacement"));
                }
                else yield return v;
            }
        }

        public static SlimeAppearance.Palette Replacement(SlimeAppearanceApplicator __instance)
        {
            var originalPallete = __instance.GetAppearancePalette();
            var painter = __instance.GetComponent<SlimePainter>();

            if (!painter) return originalPallete;
            if (painter.dataPiece == null)
            {
                return new SlimeAppearance.Palette()
                {
                    Top = painter.currTop,
                    Middle = painter.currMid,
                    Bottom = painter.currBot,
                    Ammo = originalPallete.Ammo
                };
            }
            else
            {
                return new SlimeAppearance.Palette()
                {
                    Top = painter.dataPiece.GetValue<Color>("top"),
                    Middle = painter.dataPiece.GetValue<Color>("middle"),
                    Bottom = painter.dataPiece.GetValue<Color>("bottom"),
                    Ammo = originalPallete.Ammo
                };
            }
        }
    }
}
