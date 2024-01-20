using MonomiPark.SlimeRancher.Regions;
using SRML.SR.SaveSystem;
using SRML.SR.SaveSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ColorfulSlimes
{
    public class SlimePainter : RegisteredActorBehaviour, ExtendedData.Participant, RegistryFixedUpdateable
    {
        private RegionMember regionMember;
        private SlimeAppearanceApplicator appearanceApplicator;

        private float currTime;
        private Color newTop = new Color(Random.value, Random.value, Random.value);
        private Color newMid = new Color(Random.value, Random.value, Random.value);
        private Color newBot = new Color(Random.value, Random.value, Random.value);

        internal Color currTop;
        internal Color currMid;
        internal Color currBot;

        internal CompoundDataPiece dataPiece;
        readonly List<GameObjectActorModelIdentifiableIndex.Entry> toyCache = new List<GameObjectActorModelIdentifiableIndex.Entry>();

        public void Awake()
        {
            regionMember = GetComponent<RegionMember>();
            appearanceApplicator = GetComponent<SlimeAppearanceApplicator>();
        }

        public new void Start()
        {
            appearanceApplicator.OnAppearanceChanged += SetColors;
            if (!Configuration.SHOULD_RANDOMIZE_COLORS)
                return;
            SetColors(
                new Color(Random.value, Random.value, Random.value),
                new Color(Random.value, Random.value, Random.value),
                new Color(Random.value, Random.value, Random.value)
            );
        }

        public void ReadData(CompoundDataPiece piece)
        {
            dataPiece = piece;
            SetColors();
        }

        public void WriteData(CompoundDataPiece piece)
        {
            piece.SetPiece("top", currTop);
            piece.SetPiece("middle", currMid);
            piece.SetPiece("bottom", currBot);
        }

        public new void OnDestroy()
        {
            base.OnDestroy();
            appearanceApplicator.OnAppearanceChanged -= SetColors;
        }

        public bool IsDataValid(CompoundDataPiece piece) => piece.HasPiece("top");

        void SetColors(SlimeAppearance appearance) => SetColors();

        public void SetColors(bool isRaving = false)
        {
            if (dataPiece != null)
                SetColors(dataPiece.GetValue<Color>("top"), dataPiece.GetValue<Color>("middle"), dataPiece.GetValue<Color>("bottom"));
            else
            {
                if (!isRaving)
                {
                    SetColors(
                        new Color(Random.value, Random.value, Random.value),
                        new Color(Random.value, Random.value, Random.value),
                        new Color(Random.value, Random.value, Random.value)
                    );
                }
                else
                {
                    var delta = Time.fixedDeltaTime*2;
                    SetColors(
                        Color.Lerp(currTop, newTop, delta),
                        Color.Lerp(currMid, newMid, delta),
                        Color.Lerp(currBot, newBot, delta)
                    );
                }
            }
        }

        public void SetColors(Color top, Color middle, Color bottom)
        {
            foreach (var slimeRenderer in GetComponentsInChildren<Renderer>(true))
            {
                var h = slimeRenderer.material;
                h.SetColor("_TopColor", top);
                h.SetColor("_MiddleColor", middle);
                h.SetColor("_BottomColor", bottom);

                currTop = top;
                currMid = middle;
                currBot = bottom;
            }
        }

        // dont use it lol
        //public void SetPalette()
        //{
        //    if (dataPiece != null)
        //    {
        //        appearanceApplicator.Appearance.ColorPalette = new SlimeAppearance.Palette()
        //        {
        //            Top = dataPiece.GetValue<Color>("top"),
        //            Middle = dataPiece.GetValue<Color>("middle"),
        //            Bottom = dataPiece.GetValue<Color>("bottom"),
        //            Ammo = appearanceApplicator.Appearance.ColorPalette.Ammo
        //        };
        //    }
        //    else
        //    {
        //        var slimeRenderer = GetComponentInChildren<Renderer>(true);
        //        appearanceApplicator.Appearance.ColorPalette = new SlimeAppearance.Palette()
        //        {
        //            Top = slimeRenderer.material.GetColor("_TopColor"),
        //            Middle = slimeRenderer.material.GetColor("_MiddleColor"),
        //            Bottom = slimeRenderer.material.GetColor("_BottomColor"),
        //            Ammo = appearanceApplicator.Appearance.ColorPalette.Ammo
        //        };
        //    }
        //}

        public bool CheckRaving()
        {
            if (Configuration.SHOULD_RANDOMIZE_WITH_DISCO)
            {
                toyCache.Clear();
                CellDirector.GetToysNearMember(regionMember, toyCache);
                return toyCache.Any((x) => x.Id == Identifiable.Id.DISCO_BALL_TOY && x.gameObject != null && x.gameObject.GetComponent<Raveball>().enabled && Vector3.Distance(x.gameObject.transform.position, transform.position) < 10f);
            }
            else
                return false;
        }

        public void RegistryFixedUpdate()
        {
            if (CheckRaving())
            {
                var delta = Time.fixedDeltaTime*2;
                currTime += delta;

                if (dataPiece != null)
                {
                    dataPiece.SetValue("top", Color.Lerp(dataPiece.GetValue<Color>("top"), newTop, delta));
                    dataPiece.SetValue("middle", Color.Lerp(dataPiece.GetValue<Color>("middle"), newMid, delta));
                    dataPiece.SetValue("bottom", Color.Lerp(dataPiece.GetValue<Color>("bottom"), newBot, delta));
                }
                SetColors(true);

                if (currTime >= 1)
                {
                    List<Color> generatedColors = Main.generatedColors;
                    newTop = generatedColors[Random.Range(0, generatedColors.Count)];
                    newMid = generatedColors[Random.Range(0, generatedColors.Count)];
                    newBot = generatedColors[Random.Range(0, generatedColors.Count)];
                    currTime = 0;
                }
            }
        }
    }
}
