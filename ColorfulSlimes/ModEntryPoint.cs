using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MonomiPark.SlimeRancher.Regions;
using SRML;
using SRML.Config.Attributes;
using SRML.SR;
using SRML.SR.SaveSystem;
using SRML.Utils.Enum;
using UnityEngine;
using static SRML.Console.Console;

namespace ColorfulSlimes
{
    [ConfigFile("ColorfulSlimesConfig")]
    internal class Configuration
    {
        public static bool SHOULD_RANDOMIZE_COLORS = true;

        public static bool SHOULD_RANDOMIZE_WITH_DISCO = true;
    }

    public class RaveballActivator : UIActivator
    {
        public override GameObject Activate()
        {
            if (!Configuration.SHOULD_RANDOMIZE_WITH_DISCO)
                return null;
            GetComponent<Raveball>().enabled = !GetComponent<Raveball>().enabled;
            return null;
        }
    }

    public class Main : ModEntryPoint
    {
        public static ConsoleInstance consoleInstance = new ConsoleInstance("ColorfulSlimes");
        public static List<Color> generatedColors = ModManager.GenerateColors(999);
        /*const string RAVEBALL_KEY = "t.rave_ball_toy";
        private const string RAVEBALL_UI_KEY = "m.toy.name.t.rave_ball_toy";
        private const string RAVEBALL_DESC_KEY = "m.toy.desc.t.rave_ball_toy";*/

        public override void PreLoad()
        {
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            RegisterCommand(new SetColorsCommand());
            SaveRegistry.RegisterDataParticipant<SlimePainter>();
        }

        public override void Load()
        {
            GameObject discoBallToy = Identifiable.Id.DISCO_BALL_TOY.GetPrefab();
            discoBallToy.AddComponent<Raveball>().enabled = true;
            discoBallToy.AddComponent<RaveballActivator>();

            SRCallbacks.OnMainMenuLoaded += delegate (MainMenuUI mainMenuUI)
            {
                foreach (Identifiable slimeIdent in UnityEngine.Object.FindObjectsOfType<Identifiable>())
                {
                    if (Identifiable.IsSlime(slimeIdent.id))
                    {
                        // for real just checking gold and tarr just in case
                        if (slimeIdent.gameObject && slimeIdent.id != Identifiable.Id.GOLD_GORDO 
                        && slimeIdent.id != Identifiable.Id.GOLD_SLIME 
                        && slimeIdent.id != Identifiable.Id.TARR_SLIME 
                        && Configuration.SHOULD_RANDOMIZE_COLORS 
                        && !slimeIdent.gameObject.GetComponent<SlimePainter>())
                            slimeIdent.gameObject.AddComponent<SlimePainter>();
                    }
                }
            };

            SRCallbacks.OnSaveGameLoaded += delegate (SceneContext sceneContext)
            {
                foreach (GordoIdentifiable gordoIdent in UnityEngine.Object.FindObjectsOfType<GordoIdentifiable>())
                {
                    if (Identifiable.IsGordo(gordoIdent.id))
                    {
                        // for real just checking gold and tarr just in case
                        if (gordoIdent.gameObject && gordoIdent.id != Identifiable.Id.GOLD_GORDO 
                        && gordoIdent.id != Identifiable.Id.GOLD_SLIME 
                        && gordoIdent.id != Identifiable.Id.TARR_SLIME 
                        && Configuration.SHOULD_RANDOMIZE_COLORS 
                        && !gordoIdent.gameObject.GetComponent<GordoPainter>())
                            gordoIdent.gameObject.AddComponent<GordoPainter>();
                    }
                }

                foreach (Identifiable slimeIdent in UnityEngine.Object.FindObjectsOfType<Identifiable>())
                {
                    if (Identifiable.IsSlime(slimeIdent.id))
                    {
                        if (slimeIdent.gameObject && slimeIdent.id != Identifiable.Id.QUICKSILVER_SLIME 
                        && slimeIdent.id != Identifiable.Id.GOLD_SLIME 
                        && slimeIdent.id != Identifiable.Id.TARR_SLIME 
                        && Configuration.SHOULD_RANDOMIZE_COLORS 
                        && !slimeIdent.gameObject.GetComponent<SlimePainter>())
                            slimeIdent.gameObject.AddComponent<SlimePainter>();
                    }
                }
            };
        }

        public override void PostLoad()
        {
            foreach (var slimeIdent in Identifiable.SLIME_CLASS)
            {
                if (slimeIdent.GetPrefab() && slimeIdent != Identifiable.Id.QUICKSILVER_SLIME 
                    && slimeIdent != Identifiable.Id.GOLD_SLIME 
                    && slimeIdent != Identifiable.Id.TARR_SLIME 
                    && Configuration.SHOULD_RANDOMIZE_COLORS 
                    && !slimeIdent.GetPrefab().GetComponent<SlimePainter>())
                    slimeIdent.GetPrefab().AddComponent<SlimePainter>();
            }

            foreach (var largoIdent in Identifiable.LARGO_CLASS)
            {
                // for real just checking gold and tarr just in case
                if (largoIdent.GetPrefab() && largoIdent != Identifiable.Id.QUICKSILVER_SLIME 
                    && largoIdent != Identifiable.Id.GOLD_SLIME 
                    && largoIdent != Identifiable.Id.TARR_SLIME 
                    && Configuration.SHOULD_RANDOMIZE_COLORS 
                    && !largoIdent.GetPrefab().GetComponent<SlimePainter>())
                    largoIdent.GetPrefab().AddComponent<SlimePainter>();
            }

            foreach (var gordoIdent in Identifiable.GORDO_CLASS)
            {
                // for real just checking gold and tarr just in case
                if (gordoIdent.GetPrefab() && gordoIdent != Identifiable.Id.GOLD_GORDO 
                    && gordoIdent != Identifiable.Id.QUICKSILVER_SLIME 
                    && gordoIdent != Identifiable.Id.GOLD_SLIME 
                    && gordoIdent != Identifiable.Id.TARR_SLIME 
                    && Configuration.SHOULD_RANDOMIZE_COLORS 
                    && !gordoIdent.GetPrefab().GetComponent<GordoPainter>())
                    gordoIdent.GetPrefab().AddComponent<GordoPainter>();
            }
        }
    }
}
