using MonomiPark.SlimeRancher.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorfulSlimes.Components.Painters
{
    internal class PlortPainter : Painter
    {
        private RegionMember regionMember;

        public event Action<PlortPainter> OnLateStart;

        public override void Awake()
        {
            base.Awake();
            regionMember = GetComponent<RegionMember>();
        }

        public override void Start()
        {
            base.Start();
            OnLateStart?.Invoke(this);
            OnLateStart -= OnLateStart;
        }

        public override bool IsRaveballNearby()
        {
            if (Config.SHOULD_RANDOMIZE_WITH_DISCO)
            {
                _nearbyToysCache.Clear();
                CellDirector.GetToysNearMember(regionMember, _nearbyToysCache);

                bool result = _nearbyToysCache.Any(
                    x => x.gameObject != null && x.Id == Identifiable.Id.DISCO_BALL_TOY &&
                    x.gameObject.GetComponent<Raveball>().enabled && Vector3.Distance(x.gameObject.transform.position, transform.position) < 10
                );

                return result;
            }
            return false;
        }
    }
}
