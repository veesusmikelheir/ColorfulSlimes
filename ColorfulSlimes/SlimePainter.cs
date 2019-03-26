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

        public void Awake()
        {
            regionMember = GetComponent<RegionMember>();
        }
        public void InitData(CompoundDataPiece piece)
        {
            var comp = piece.GetCompoundPiece("slimecolors");
            comp.SetPiece("top", new Color(Random.value, Random.value, Random.value, Random.value));
            comp.SetPiece("middle", new Color(Random.value, Random.value, Random.value, Random.value));
            comp.SetPiece("bottom", new Color(Random.value, Random.value, Random.value, Random.value));
            comp.SetPiece("isdisco", false);
        }

        public bool IsDataValid(CompoundDataPiece piece)
        {
            Debug.Log("are we valid? "+piece.HasPiece("slimecolors"));
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
                slimeRenderer.material = h;
            }

        }

        public void SetColors()
        {
            SetColors(
                ourPiece.GetValue<Color>("top"),
                ourPiece.GetValue<Color>("middle"),
                ourPiece.GetValue<Color>("bottom"));
        }

        

        private static int topColorNameId = Shader.PropertyToID("_TopColor");

        private static int middleColorNameId = Shader.PropertyToID("_MiddleColor");

        private static int bottomColorNameId = Shader.PropertyToID("_BottomColor");

        public void SetData(CompoundDataPiece piece)
        {
            
            ourPiece = piece.GetCompoundPiece("slimecolors");

            SetColors();
        }

        List<GameObjectActorModelIdentifiableIndex.Entry> toyCache = new List<GameObjectActorModelIdentifiableIndex.Entry>();

        public void CheckRaving()
        {
            toyCache.Clear();
            CellDirector.GetToysNearMember(regionMember, toyCache);
            ourPiece.SetValue("isdisco",toyCache.Any((x) => x.Id == (Identifiable.Id) 9988));
        }

        public void RegistryFixedUpdate()
        {
            CheckRaving();
            if (ourPiece.GetValue<bool>("isdisco"))
            {
                ourPiece.SetValue("top", new Color(Random.value, Random.value, Random.value, Random.value));
                ourPiece.SetValue("middle", new Color(Random.value, Random.value, Random.value, Random.value));
                ourPiece.SetValue("bottom", new Color(Random.value, Random.value, Random.value, Random.value));
            }
        }
    }
}
