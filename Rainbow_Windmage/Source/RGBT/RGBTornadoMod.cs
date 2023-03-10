using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RGBT
{
    public class RGBTornadoMod : Mod
    {

        public static RGBTornadoModSettings settings;

        public RGBTornadoMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<RGBTornadoModSettings>();

        }
    }
}
