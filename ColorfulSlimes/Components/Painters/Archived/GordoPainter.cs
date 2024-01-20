using MonomiPark.SlimeRancher.Regions;
using SRML.SR.SaveSystem;
using SRML.SR.SaveSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ColorfulSlimes
{
    public class GordoPainter : RegisteredActorBehaviour, ExtendedData.Participant, RegistryFixedUpdateable
    {
        private float currTime;
        private Color newTop = new Color(Random.value, Random.value, Random.value);
        private Color newMid = new Color(Random.value, Random.value, Random.value);
        private Color newBot = new Color(Random.value, Random.value, Random.value);

        internal Color currTop;
        internal Color currMid;
        internal Color currBot;

        internal CompoundDataPiece dataPiece;
        List<GameObjectActorModelIdentifiableIndex.Entry> toyCache = new List<GameObjectActorModelIdentifiableIndex.Entry>();

        public void Awake()
        {
            if (GetComponent<GordoPainter>() != this)
                Destroy(this);
        }

        public new void Start()
        {
            if (!Configuration.SHOULD_RANDOMIZE_COLORS)
                return;
            SetColors();
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

        public bool IsDataValid(CompoundDataPiece piece) => piece.HasPiece("top");

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
            foreach (var gordoRenderer in GetComponentsInChildren<Renderer>(true))
            {
                var h = gordoRenderer.material;
                h.SetColor("_TopColor", top);
                h.SetColor("_MiddleColor", middle);
                h.SetColor("_BottomColor", bottom);

                currTop = top;
                currMid = middle;
                currBot = bottom;
            }
        }

        public bool CheckRaving()
        {
            if (Configuration.SHOULD_RANDOMIZE_WITH_DISCO)
            {
                toyCache.Clear();
                List<Identifiable.Id> ids = new List<Identifiable.Id>() { Identifiable.Id.DISCO_BALL_TOY };
                GetComponentInParent<CellDirector>().Get(ids, toyCache, new HashSet<GameObjectActorModelIdentifiableIndex.Entry>() { });
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
