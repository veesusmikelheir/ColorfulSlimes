using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using MonomiPark.SlimeRancher.Regions;
using SRML;
using SRML.SR;
using SRML.SR.SaveSystem;
using UnityEngine;

namespace ColorfulSlimes
{
    public class Main : ModEntryPoint
    {
        const string raveball_key = "t.rave_ball_toy";
        public override void PreLoad(HarmonyInstance instance)
        {
            instance.PatchAll(Assembly.GetExecutingAssembly());
            
            TranslationPatcher.AddTranslationKey("pedia",raveball_key,"Rave Ball");
            var id = IdentifiablePatcher.CreateIdentifiableId(9988, "RAVE_BALL_TOY");
            var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject.DontDestroyOnLoad(g);
            g.AddComponent<RegionMember>();
            g.AddComponent<Identifiable>().id = id;
            g.AddComponent<Rigidbody>();
            g.layer = LayerMask.NameToLayer("Actor");
            g.AddComponent<Vacuumable>().size = Vacuumable.Size.LARGE;
            LookupRegistry.RegisterIdentifiablePrefab(g);


        }
        public override void PostLoad()
        {
            var toyEntry = new LookupDirector.ToyEntry()
            {
                cost = 1000,
                icon = GameContext.Instance.LookupDirector.toyDict[Identifiable.Id.DISCO_BALL_TOY].icon,
                nameKey = raveball_key,
                toyId = (Identifiable.Id) 9988
            };
            GameContext.Instance.LookupDirector.toyEntries.Add(toyEntry);
            GameContext.Instance.LookupDirector.toyDict.Add(toyEntry.toyId, toyEntry);

            foreach (var g in GameContext.Instance.LookupDirector.identifiablePrefabs)
            {
                if (Identifiable.IsSlime(Identifiable.GetId(g))&&Identifiable.GetId(g)!=Identifiable.Id.GOLD_SLIME) g.AddComponent<SlimePainter>();
            }
        }
    }
}
