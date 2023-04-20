//using MonomiPark.SlimeRancher.Regions;
//using SRML.SR.SaveSystem;
//using SRML.SR.SaveSystem.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using Random = UnityEngine.Random;

//namespace ColorfulSlimes
//{
//    internal class SlimePainterV2 : RegisteredActorBehaviour, ExtendedData.Participant, RegistryFixedUpdateable
//    {
//        private RegionMember regionMember;
//        private SlimeAppearanceApplicator appearanceApplicator;
//        private Color[] slimeColors;

//        readonly List<GameObjectActorModelIdentifiableIndex.Entry> toyCache = new List<GameObjectActorModelIdentifiableIndex.Entry>();

//        public void Awake()
//        {
//            regionMember = GetComponent<RegionMember>();
//            appearanceApplicator = GetComponent<SlimeAppearanceApplicator>();
//        }

//        public new void Start()
//        {
//            if (!Configuration.SHOULD_RANDOMIZE_COLORS)
//                return;
//            if (slimeColors != null && slimeColors.Length > 0)
//            {
//                SetColors();
//                SetPalette();
//                appearanceApplicator.OnAppearanceChanged += SetColors;
//            }
//        }

//        public void ReadData(CompoundDataPiece piece)
//        {
//            throw new NotImplementedException();
//        }

//        public void WriteData(CompoundDataPiece piece)
//        {
//            throw new NotImplementedException();
//        }

//        void SetColors(SlimeAppearance appearance)
//        {
//            SetColors();
//            SetPalette();
//        }

//        public void SetColors() => SetColors(slimeColors[0], slimeColors[1], slimeColors[2]);

//        public void SetColors(Color top, Color middle, Color bottom)
//        {
//            foreach (var slimeRenderer in GetComponentsInChildren<Renderer>(true))
//            {
//                var h = slimeRenderer.material;
//                h.SetColor("_TopColor", top);
//                h.SetColor("_MiddleColor", middle);
//                h.SetColor("_BottomColor", bottom);
//            }
//        }

//        public void RegistryFixedUpdate()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
