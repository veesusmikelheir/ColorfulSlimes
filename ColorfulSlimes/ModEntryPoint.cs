global using SRML;
global using UnityEngine;
global using static Utility;

using System;
using System.Collections.Generic;
using System.Linq;
using ColorfulSlimes.Commands;
using ColorfulSlimes.Components;
using ColorfulSlimes.Components.Painters;
using SRML.Config.Attributes;
using SRML.SR;
using SRML.SR.SaveSystem;
using static SRML.Console.Console;

namespace ColorfulSlimes
{
    [ConfigFile("ColorfulSlimesConfig")]
    internal static class Config
    {
        public static bool SHOULD_MODIFY_PLORT_COLORS = false;

        public static bool SHOULD_SAVE_RANDOMIZED_COLORS = true;

        public static bool SHOULD_RANDOMIZE_WITH_DISCO = true;
    }

    public class Main : ModEntryPoint
    {
        internal static readonly ConsoleInstance ModConsole = new ConsoleInstance("ColorfulSlimes");
        internal static readonly List<Color> GeneratedColors = GenerateColors(999);

        public override void PreLoad()
        {
            HarmonyInstance.PatchAll();
            RegisterCommand(new SetColors());
            RegisterCommand(new GetColors());
            RegisterCommand(new CopyColors());
            RegisterCommand(new PasteColors());

            SaveRegistry.RegisterDataParticipant<Raveball>();
            SaveRegistry.RegisterDataParticipant<Painter>();

            SaveRegistry.RegisterDataParticipant<SlimePainter>();
            SaveRegistry.RegisterDataParticipant<PlortPainter>();
            SaveRegistry.RegisterDataParticipant<NoRaveballPainter>();
        }

        public override void Load()
        {
            GameObject discoPrefab = Identifiable.Id.DISCO_BALL_TOY.GetPrefab();
            discoPrefab.AddComponent<Raveball>().enabled = false;
            discoPrefab.AddComponent<RaveballActivator>();

            SRCallbacks.OnMainMenuLoaded += delegate (MainMenuUI mainMenuUI)
            {
                foreach (Identifiable identifiable in UnityEngine.Object.FindObjectsOfType<Identifiable>())
                {
                    if (Identifiable.IsSlime(identifiable.id) && identifiable.id != Identifiable.Id.GOLD_SLIME)
                    {
                        if (!identifiable.gameObject.GetComponent<SlimePainter>())
                            identifiable.gameObject.AddComponent<SlimePainter>();
                    }
                }
            };

            SRCallbacks.OnSaveGameLoaded += delegate (SceneContext sceneContext)
            {
                foreach (GordoIdentifiable gordoIdentifiable in UnityEngine.Object.FindObjectsOfType<GordoIdentifiable>())
                {
                    if (Identifiable.IsGordo(gordoIdentifiable.id) && gordoIdentifiable.id != Identifiable.Id.GOLD_GORDO)
                    {
                        if (!gordoIdentifiable.gameObject.GetComponent<Painter>())
                            gordoIdentifiable.gameObject.AddComponent<Painter>();
                    }
                }

                foreach (Identifiable identifiable in UnityEngine.Object.FindObjectsOfType<Identifiable>())
                {
                    if (Identifiable.IsSlime(identifiable.id) && identifiable.id != Identifiable.Id.GOLD_SLIME)
                    {
                        if (!identifiable.gameObject.GetComponent<SlimePainter>())
                            identifiable.gameObject.AddComponent<SlimePainter>();
                    }

                    if (Config.SHOULD_MODIFY_PLORT_COLORS)
                    {
                        if (Identifiable.IsPlort(identifiable.id) && identifiable.id != Identifiable.Id.GOLD_PLORT)
                        {
                            if (!identifiable.gameObject.GetComponent<PlortPainter>())
                                identifiable.gameObject.AddComponent<PlortPainter>();
                        }
                    }
                }
            };
        }

        public override void PostLoad()
        {
            foreach (var prefab in GameContext.Instance.LookupDirector.identifiablePrefabs)
            {
                if (Identifiable.IsSlime(Identifiable.GetId(prefab)) && Identifiable.GetId(prefab) != Identifiable.Id.GOLD_SLIME)
                    prefab.AddComponent<SlimePainter>();

                if (Identifiable.IsGordo(Identifiable.GetId(prefab)) && Identifiable.GetId(prefab) != Identifiable.Id.GOLD_GORDO)
                    prefab.AddComponent<Painter>();

                if (Config.SHOULD_MODIFY_PLORT_COLORS)
                {
                    if (Identifiable.IsPlort(Identifiable.GetId(prefab)) && Identifiable.GetId(prefab) != Identifiable.Id.GOLD_PLORT)
                        prefab.AddComponent<PlortPainter>();
                }
            }
        }
    }
}
