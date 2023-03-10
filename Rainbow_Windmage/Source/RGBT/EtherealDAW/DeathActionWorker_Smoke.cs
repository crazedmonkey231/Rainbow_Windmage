using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace RGBT.EtherealDAW
{
    public class DeathActionWorker_Smoke : DeathActionWorker
    {
        public override void PawnDied(Corpse corpse)
        {
            if (corpse.Map == null)
                return;
            FleckMaker.Static(corpse.Position, corpse.Map, RGBDefOf.RGB_Smoke, 1.0f);
            corpse.Destroy(DestroyMode.Vanish);
        }
    }
}
