using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorfulSlimes.Components
{
    public class RaveballActivator : UIActivator
    {
        public override GameObject Activate()
        {
            if (!Config.SHOULD_RANDOMIZE_WITH_DISCO)
                return null;
            GetComponent<Raveball>().enabled = !GetComponent<Raveball>().enabled;
            return null;
        }
    }
}
