using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorfulSlimes.Components.Painters
{
    internal class NoRaveballPainter : Painter
    {
        public override bool IsRaveballNearby() => false;
    }
}
