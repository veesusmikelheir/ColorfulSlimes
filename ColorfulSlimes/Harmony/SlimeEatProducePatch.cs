using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ColorfulSlimes.Components.Painters;
using System.Collections;

namespace ColorfulSlimes.Harmony
{
    [HarmonyPatch(typeof(SlimeEat), nameof(SlimeEat.Produce))]
    internal static class SlimeEatProducePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instrs = instructions.ToList();
            for (int i = 0; i < instrs.Count; i++)
            {
                var instr = instrs[i];
                yield return instr;

                if (instr.operand is MethodInfo ia && ia.Name == "InstantiateActor" && Config.SHOULD_MODIFY_PLORT_COLORS)
                {
                    i++;
                    yield return instrs[i];

                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    yield return new CodeInstruction(OpCodes.Callvirt, typeof(SlimeEatProducePatch).GetMethod(nameof(Paint)));
                }
            }
        }

        public static IEnumerator PaintAfterDelay(SlimePainter slimePainter, PlortPainter plortPainter, float delay = 0.1f)
        {
            yield return new WaitForSeconds(delay);
            plortPainter.Paint(slimePainter.currTop, slimePainter.currMid, slimePainter.currBot);
            yield break;
        }

        public static void Paint(SlimeEat __instance, GameObject gameObj)
        {
            var slimePainter = __instance?.GetComponent<SlimePainter>();
            if (!slimePainter)
                return;

            if (Identifiable.IsPlort(Identifiable.GetId(gameObj)))
            {
                var plortPainter = gameObj.GetComponent<PlortPainter>();
                if (!plortPainter)
                    return;
                plortPainter.StartCoroutine(PaintAfterDelay(slimePainter, plortPainter));
            }
        }
    }
}
