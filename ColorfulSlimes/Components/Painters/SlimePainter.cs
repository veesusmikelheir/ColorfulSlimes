using MonomiPark.SlimeRancher.Regions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorfulSlimes.Components.Painters
{
    internal class SlimePainter : Painter
    {
        private SlimeAppearanceApplicator slimeAppearanceApplicator;
        private RegionMember regionMember;

        public override void Awake()
        {
            base.Awake();
            slimeAppearanceApplicator = GetComponent<SlimeAppearanceApplicator>();
            regionMember = GetComponent<RegionMember>();
        }

        public override void Start()
        {
            /* mainly for no randomization config
            var baseMaterial = slimeAppearanceApplicator.Appearance.Structures[0].DefaultMaterials[0];
            currTop = baseMaterial.GetColor("_TopColor");
            currMid = baseMaterial.GetColor("_MiddleColor");
            currBot = baseMaterial.GetColor("_BottomColor");*/

            base.Start();
            slimeAppearanceApplicator.OnAppearanceChanged += Paint;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            slimeAppearanceApplicator.OnAppearanceChanged -= Paint;
        }

        void Paint(SlimeAppearance appearance) => Paint();

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
