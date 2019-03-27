using SRML.SR.SaveSystem;
using SRML.SR.SaveSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ColorfulSlimes
{
    public class SlimePainter : RegisteredActorBehaviour, ExtendedData.Participant, RegistryFixedUpdateable
    {
        private CompoundDataPiece ourPiece;
        private RegionMember regionMember;
        private SplatOnImpact splatOnImpact;

        private float curTime;
        private Color newTop = new Color(Random.value, Random.value, Random.value, Random.value);
        private Color newMid = new Color(Random.value, Random.value, Random.value, Random.value);
        private Color newBot = new Color(Random.value, Random.value, Random.value, Random.value);

        private static int topColorNameId = Shader.PropertyToID("_TopColor");
        private static int middleColorNameId = Shader.PropertyToID("_MiddleColor");
        private static int bottomColorNameId = Shader.PropertyToID("_BottomColor");

        List<GameObjectActorModelIdentifiableIndex.Entry> toyCache = new List<GameObjectActorModelIdentifiableIndex.Entry>();

        public void Awake()
        {
            regionMember = GetComponent<RegionMember>();
            splatOnImpact = GetComponent<SplatOnImpact>();
        }
        public void InitData(CompoundDataPiece piece)
        {
            var comp = piece.GetCompoundPiece("slimecolors");
            comp.SetPiece("top", new Color(Random.value, Random.value, Random.value, Random.value));
            comp.SetPiece("middle", new Color(Random.value, Random.value, Random.value, Random.value));
            comp.SetPiece("bottom", new Color(Random.value, Random.value, Random.value, Random.value));
        }

        public bool IsDataValid(CompoundDataPiece piece)
        {
            return piece.HasPiece("slimecolors");
        }

        public void SetColors(Color top, Color middle, Color bottom)
        {
            foreach (var slimeRenderer in GetComponentsInChildren<Renderer>())
            {
                var h = slimeRenderer.material;
                h.SetColor(topColorNameId, top);
                h.SetColor(middleColorNameId, middle);
                h.SetColor(bottomColorNameId, bottom);
            }

            if (splatOnImpact)
            {
                splatOnImpact.topColor = top;
                splatOnImpact.midColor = middle;
                splatOnImpact.btmColor = bottom;
            }
        }

        public void SetColors()
        {
            SetColors(
                ourPiece.GetValue<Color>("top"),
                ourPiece.GetValue<Color>("middle"),
                ourPiece.GetValue<Color>("bottom"));
        }

        public void SetData(CompoundDataPiece piece)
        {
            
            ourPiece = piece.GetCompoundPiece("slimecolors");

            SetColors();
        }

        public bool CheckRaving()
        {
            toyCache.Clear();
            CellDirector.GetToysNearMember(regionMember, toyCache);
            
            return toyCache.Any((x) => x.Id == Main.RAVE_BALL_ID&&x.gameObject!=null&&Vector3.Distance(x.gameObject.transform.position,transform.position)<10f);
        }

        public void RegistryFixedUpdate()
        {
            
            if (CheckRaving())
            {
                var delta = Time.fixedDeltaTime*2;
                curTime += delta;

                ourPiece.SetValue("top", Color.Lerp(ourPiece.GetValue<Color>("top"),newTop, delta));
                ourPiece.SetValue("middle", Color.Lerp(ourPiece.GetValue<Color>("middle"), newMid, delta));
                ourPiece.SetValue("bottom", Color.Lerp(ourPiece.GetValue<Color>("bottom"), newBot, delta));
                SetColors();

                if (curTime >= 1)
                {

                    newTop = new Color(Random.value, Random.value, Random.value, Random.value);

                    newMid = new Color(Random.value, Random.value, Random.value, Random.value);

                    newBot = new Color(Random.value, Random.value, Random.value, Random.value);


                    curTime = 0;
                }
            }
        }
    }
}
