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
    [HarmonyPatch(typeof(TarrSpawnFX), nameof(TarrSpawnFX.OnSpawnBubbles))]
    internal static class TarrSpawnFXOnSpawnBubblesPatch
    {
        public static void Postfix(TarrSpawnFX __instance)
        {
            try
            {
                var slimePainter = __instance?.GetComponent<SlimePainter>();
                if (!slimePainter)
                    return;

                foreach (var fx in __instance.gameObject?.FindChildrenWithPartialName("FX Tarr Bubbles"))
                {
                    if (!fx)
                        return;

                    if (!fx.GetComponent<NoRaveballPainter>())
                        fx.AddComponent<NoRaveballPainter>().StartCoroutine(PaintAfterDelay(slimePainter, fx.GetComponent<NoRaveballPainter>()));
                }
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
