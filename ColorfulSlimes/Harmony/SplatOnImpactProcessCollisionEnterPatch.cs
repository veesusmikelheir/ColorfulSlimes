using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using UnityEngine;
using ColorfulSlimes.Components.Painters;

namespace ColorfulSlimes
{
    [HarmonyPatch(typeof(SplatOnImpact), nameof(SplatOnImpact.ProcessCollisionEnter))]
    internal static class SplatOnImpactProcessCollisionEnterPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach (var v in instr)
            {
                if (v.opcode == OpCodes.Callvirt && (v.operand as MethodInfo).Name == "GetAppearancePalette")
                    yield return new CodeInstruction(OpCodes.Call, typeof(SplatOnImpactProcessCollisionEnterPatch).GetMethod(nameof(Replacement)));
                else yield return v;
            }
        }

        public static SlimeAppearance.Palette Replacement(SlimeAppearanceApplicator __instance)
        {
            var originalPallete = __instance.GetAppearancePalette();
            var slimePainter = __instance.GetComponent<SlimePainter>();

            if (!slimePainter)
                return originalPallete;

            if (!slimePainter.IsDataValid())
            {
                return new SlimeAppearance.Palette()
                {
                    Top = slimePainter.currTop,
                    Middle = slimePainter.currMid,
                    Bottom = slimePainter.currBot,
                    Ammo = slimePainter.currTop
                };
            }
            else
            {
                return new SlimeAppearance.Palette()
                {
                    Top = slimePainter.dataPiece.GetValue<Color>("top"),
                    Middle = slimePainter.dataPiece.GetValue<Color>("middle"),
                    Bottom = slimePainter.dataPiece.GetValue<Color>("bottom"),
                    Ammo = slimePainter.dataPiece.GetValue<Color>("top")
                };
            }
        }
    }
}
