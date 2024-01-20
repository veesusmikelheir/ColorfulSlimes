using ColorfulSlimes.Components.Painters;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorfulSlimes.Harmony
{
    [HarmonyPatch(typeof(TentacleGrapple), nameof(TentacleGrapple.MaybeGrapple))]
    internal static class TentacleGrappleMaybeGrapplePatch
    {
        public static void Postfix(TentacleGrapple __instance)
        {
            try
            {
                var tentacle = __instance?.activeTentacle;
                var slimePainter = __instance?.GetComponent<SlimePainter>();
                if (!slimePainter)
                    return;

                if (!tentacle?.GetComponent<NoRaveballPainter>())
                    tentacle?.AddComponent<NoRaveballPainter>().StartCoroutine(PaintAfterDelay(slimePainter, tentacle?.GetComponent<NoRaveballPainter>()));
            }
            catch { }
        }

        public static IEnumerator PaintAfterDelay(SlimePainter slimePainter, NoRaveballPainter noRaveballPainter, float delay = 0.1f)
        {
            yield return new WaitForSeconds(delay);
            noRaveballPainter.Paint(slimePainter.currTop, slimePainter.currMid, slimePainter.currBot);
            yield break;
        }
    }
}
