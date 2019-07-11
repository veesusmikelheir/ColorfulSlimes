using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using MonomiPark.SlimeRancher.Regions;
using SRML;
using SRML.SR;
using SRML.SR.SaveSystem;
using SRML.SR.SaveSystem.Data;
using SRML.Utils;
using SRML.Utils.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ColorfulSlimes
{
    public class Main : IModEntryPoint
    {
        const string RAVEBALL_KEY = "t.rave_ball_toy";
        private const string RAVEBALL_UI_KEY = "m.toy.name.t.rave_ball_toy";
        private const string RAVEBALL_DESC_KEY = "m.toy.desc.t.rave_ball_toy";

        public void PreLoad()
        {
            HarmonyPatcher.GetInstance().PatchAll(Assembly.GetExecutingAssembly());

            SRML.Console.Console.RegisterCommand(new SetColorsCommand());
            TranslationPatcher.AddTranslationKey("pedia", RAVEBALL_KEY, "Rave Ball");
            TranslationPatcher.AddTranslationKey("pedia", RAVEBALL_UI_KEY, "Rave Ball");
            TranslationPatcher.AddTranslationKey("pedia", RAVEBALL_DESC_KEY,
                "This ball may look plain, but it makes slimes want to party!");
            //IdentifiableRegistry.CreateIdentifiableId(RAVE_BALL_ID, "RAVE_BALL_TOY");
            var bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(typeof(Main), "colorfulslimes"));
            var g = bundle.LoadAsset<GameObject>("raveball");
            g.AddComponent<RegionMember>();
            g.AddComponent<Identifiable>().id = CustomIds.RAVE_BALL_TOY;
            g.layer = LayerMask.NameToLayer("Actor");
            g.AddComponent<Vacuumable>().size = Vacuumable.Size.LARGE;
            g.AddComponent<Raveball>();
            g.transform.GetChild(0).gameObject.AddComponent<VacDelaunchTrigger>();

            LookupRegistry.RegisterIdentifiablePrefab(g);

            SaveRegistry.RegisterDataParticipant<SlimePainter>();
        }

        public void PostLoad()
        {

            foreach (var g in GameContext.Instance.LookupDirector.identifiablePrefabs)
            {
                if (Identifiable.IsSlime(Identifiable.GetId(g)) && Identifiable.GetId(g) != Identifiable.Id.GOLD_SLIME)
                    g.AddComponent<SlimePainter>();
            }
        }

        public void Load()
        {
            var toyEntry = new LookupDirector.ToyEntry()
            {
                cost = 1000,
                icon = GameContext.Instance.LookupDirector.toyDict[Identifiable.Id.DISCO_BALL_TOY].icon,
                nameKey = RAVEBALL_KEY,
                toyId = CustomIds.RAVE_BALL_TOY
            };


            GameContext.Instance.LookupDirector.toyEntries.Add(toyEntry);
            GameContext.Instance.LookupDirector.toyDict.Add(toyEntry.toyId, toyEntry);

            ToyDirector.BASE_TOYS.Add(CustomIds.RAVE_BALL_TOY);


        }
    }

    [HarmonyPatch(typeof(SplatOnImpact))]
    [HarmonyPatch("ProcessCollisionEnter")]
    internal static class AppearancePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach(var v in instr)
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
            return new SlimeAppearance.Palette()
            {
                Top = painter.dataPiece.GetValue<Color>("top"),
                Middle = painter.dataPiece.GetValue<Color>("middle"),
                Bottom = painter.dataPiece.GetValue<Color>("bottom"),
                Ammo = originalPallete.Ammo
            };
        }
    }
    [EnumHolder]
    internal static class CustomIds
    {
        public static readonly Identifiable.Id RAVE_BALL_TOY;
    }
}
