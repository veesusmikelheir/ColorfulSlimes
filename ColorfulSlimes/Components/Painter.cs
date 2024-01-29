using MonomiPark.SlimeRancher.Regions;
using SRML.SR.SaveSystem;
using SRML.SR.SaveSystem.Data;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using static ColorfulSlimes.Main;

namespace ColorfulSlimes.Components
{
    internal class Painter : RegisteredActorBehaviour, ExtendedData.Participant, RegistryFixedUpdateable
    {
        internal readonly List<GameObjectActorModelIdentifiableIndex.Entry> _nearbyToysCache = new();
        internal float currTime;

        internal CompoundDataPiece dataPiece;
        internal Color currTop, currMid, currBot;
        internal Color raveTop, raveMid, raveBot = new Color(Random.value, Random.value, Random.value);

        public virtual void Awake()
        {
            if (GetComponent<Painter>() != this)
                Destroy(this);
        }

        public virtual new void Start() => Paint();

        public virtual void ReadData(CompoundDataPiece piece)
        {
            if (!Config.SHOULD_SAVE_RANDOMIZED_COLORS)
                return;
            dataPiece = piece;
        }

        public virtual void WriteData(CompoundDataPiece piece)
        {
            if (!Config.SHOULD_SAVE_RANDOMIZED_COLORS)
                return;
            piece.SetPiece("top", currTop);
            piece.SetPiece("middle", currMid);
            piece.SetPiece("bottom", currBot);
        }

        public virtual bool IsDataValid() => dataPiece != null ? dataPiece.HasPiece("top") : false;

        public virtual bool IsRaveballNearby()
        {
            if (Config.SHOULD_RANDOMIZE_WITH_DISCO)
            {
                _nearbyToysCache.Clear();
                GetComponentInParent<CellDirector>().Get(new List<Identifiable.Id>() { Identifiable.Id.DISCO_BALL_TOY }, _nearbyToysCache, new());

                bool result = _nearbyToysCache.Any(
                    x => x.gameObject != null && x.Id == Identifiable.Id.DISCO_BALL_TOY &&
                    x.gameObject.GetComponent<Raveball>().enabled && Vector3.Distance(x.gameObject.transform.position, transform.position) < 10
                );

                return result;
            }
            return false;
        }

        public virtual void Paint()
        {
            if (IsDataValid() && Config.SHOULD_SAVE_RANDOMIZED_COLORS)
                Paint(dataPiece.GetValue<Color>("top"), dataPiece.GetValue<Color>("middle"), dataPiece.GetValue<Color>("bottom"));
            else
            {
                Paint(
                    new Color(Random.value, Random.value, Random.value),
                    new Color(Random.value, Random.value, Random.value),
                    new Color(Random.value, Random.value, Random.value)
                );
            }
        }

        public virtual void Paint(Color top, Color middle, Color bottom)
        {
            foreach (var renderer in GetComponentsInChildren<Renderer>(true))
            {
                var material = renderer.material;
                material.SetColor("_TopColor", top);
                material.SetColor("_MiddleColor", middle);
                material.SetColor("_BottomColor", bottom);

                // rad/fire stuff
                material.SetColor("_ColorOutside", middle);
                material.SetColor("_ColorInside", bottom);
                material.SetColor("_EdgeColor", bottom);
                material.color = middle;

                currTop = top;
                currMid = middle;
                currBot = bottom;
            }
        }

        public virtual void RegistryFixedUpdate()
        {
            if (IsRaveballNearby())
            {
                var delta = Time.fixedDeltaTime*2;
                currTime += delta;

                if (IsDataValid() && Config.SHOULD_SAVE_RANDOMIZED_COLORS)
                {
                    dataPiece.SetValue("top", Color.Lerp(dataPiece.GetValue<Color>("top"), raveTop, delta));
                    dataPiece.SetValue("middle", Color.Lerp(dataPiece.GetValue<Color>("middle"), raveMid, delta));
                    dataPiece.SetValue("bottom", Color.Lerp(dataPiece.GetValue<Color>("bottom"), raveBot, delta));
                    Paint();
                }
                else
                    Paint(Color.Lerp(currTop, raveTop, delta), Color.Lerp(currMid, raveMid, delta), Color.Lerp(currBot, raveBot, delta));

                if (currTime >= 1)
                {
                    raveTop = GeneratedColors[Random.Range(0, GeneratedColors.Count)];
                    raveMid = GeneratedColors[Random.Range(0, GeneratedColors.Count)];
                    raveBot = GeneratedColors[Random.Range(0, GeneratedColors.Count)];
                    currTime = 0;
                }
            }
        }
    }
}
