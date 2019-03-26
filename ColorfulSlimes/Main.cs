using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using SRML;
using SRML.SR;
using SRML.SR.SaveSystem;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace ColorfulSlimes
{
    public class Main : ModEntryPoint
    {
        const string raveball_key = "t.rave_ball_toy";
        private const string raveball_ui_key = "m.toy.name.t.rave_ball_toy";
        private const string raveball_desc_key = "m.toy.desc.t.rave_ball_toy";
        public static Identifiable.Id RAVE_BALL_ID = (Identifiable.Id)9988;
        public override void PreLoad(HarmonyInstance instance)
        {
            instance.PatchAll(Assembly.GetExecutingAssembly());
            
            TranslationPatcher.AddTranslationKey("pedia",raveball_key,"Rave Ball");
            TranslationPatcher.AddTranslationKey("pedia", raveball_ui_key, "Rave Ball");
            TranslationPatcher.AddTranslationKey("pedia", raveball_desc_key,
                "This ball may look plain, but it makes slimes want to party!");
            var id = IdentifiablePatcher.CreateIdentifiableId(RAVE_BALL_ID, "RAVE_BALL_TOY");
            var g = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Main), "colorfulslimes")).LoadAsset<GameObject>("raveball");

            g.AddComponent<RegionMember>();
            g.AddComponent<Identifiable>().id = id;
            g.layer = LayerMask.NameToLayer("Actor");
            g.AddComponent<Vacuumable>().size = Vacuumable.Size.LARGE;
            LookupRegistry.RegisterIdentifiablePrefab(g);
            SRCallbacks.OnSaveGameLoaded += (t) =>
            {
                SRBehaviour.InstantiateActor(g, t.GameModel.GetPlayerModel().position,
                    t.GameModel.GetPlayerModel().rotation);
            };

        }
        public override void PostLoad()
        {
            var toyEntry = new LookupDirector.ToyEntry()
            {
                cost = 1000,
                icon = GameContext.Instance.LookupDirector.toyDict[Identifiable.Id.DISCO_BALL_TOY].icon,
                nameKey = raveball_key,
                toyId = RAVE_BALL_ID
            };
            GameContext.Instance.LookupDirector.toyEntries.Add(toyEntry);
            GameContext.Instance.LookupDirector.toyDict.Add(toyEntry.toyId, toyEntry);

            ToyDirector.BASE_TOYS.Add(RAVE_BALL_ID);

            foreach (var g in GameContext.Instance.LookupDirector.identifiablePrefabs)
            {
                if (Identifiable.IsSlime(Identifiable.GetId(g))&&Identifiable.GetId(g)!=Identifiable.Id.GOLD_SLIME) g.AddComponent<SlimePainter>();
            }
        }

    }
}
