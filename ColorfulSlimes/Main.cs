using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Harmony;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using MonomiPark.SlimeRancher.Regions;
using SRML;
using SRML.SR;
using SRML.SR.SaveSystem;
using SRML.SR.SaveSystem.Data;
using SRML.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ColorfulSlimes
{
    public class Main : ModEntryPoint
    {
        const string RAVEBALL_KEY = "t.rave_ball_toy";
        private const string RAVEBALL_UI_KEY = "m.toy.name.t.rave_ball_toy";
        private const string RAVEBALL_DESC_KEY = "m.toy.desc.t.rave_ball_toy";
        public const Identifiable.Id RAVE_BALL_ID = (Identifiable.Id) 9988;
        public const Gadget.Id GADGET_ID = (Gadget.Id) 9999;

        public override void PreLoad()
        {
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            TranslationPatcher.AddTranslationKey("pedia", RAVEBALL_KEY, "Rave Ball");
            TranslationPatcher.AddTranslationKey("pedia", RAVEBALL_UI_KEY, "Rave Ball");
            TranslationPatcher.AddTranslationKey("pedia", RAVEBALL_DESC_KEY,
                "This ball may look plain, but it makes slimes want to party!");
            IdentifiablePatcher.CreateIdentifiableId(RAVE_BALL_ID, "RAVE_BALL_TOY");
            var bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(typeof(Main), "colorfulslimes"));
            var g = bundle.LoadAsset<GameObject>("raveball");

            g.AddComponent<RegionMember>();
            g.AddComponent<Identifiable>().id = RAVE_BALL_ID;
            g.layer = LayerMask.NameToLayer("Actor");
            g.AddComponent<Vacuumable>().size = Vacuumable.Size.LARGE;
            g.AddComponent<Raveball>();
            g.transform.GetChild(0).gameObject.AddComponent<VacDelaunchTrigger>();

            LookupRegistry.RegisterIdentifiablePrefab(g);


        }

        public override void PostLoad()
        {
            var toyEntry = new LookupDirector.ToyEntry()
            {
                cost = 1000,
                icon = GameContext.Instance.LookupDirector.toyDict[Identifiable.Id.DISCO_BALL_TOY].icon,
                nameKey = RAVEBALL_KEY,
                toyId = RAVE_BALL_ID
            };


            GameContext.Instance.LookupDirector.toyEntries.Add(toyEntry);
            GameContext.Instance.LookupDirector.toyDict.Add(toyEntry.toyId, toyEntry);

            ToyDirector.BASE_TOYS.Add(RAVE_BALL_ID);



            foreach (var g in GameContext.Instance.LookupDirector.identifiablePrefabs)
            {
                if (Identifiable.IsSlime(Identifiable.GetId(g)) && Identifiable.GetId(g) != Identifiable.Id.GOLD_SLIME)
                    g.AddComponent<SlimePainter>();
            }
        }

        
    }
}
