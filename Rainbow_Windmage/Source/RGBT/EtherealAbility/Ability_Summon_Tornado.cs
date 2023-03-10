using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanillaPsycastsExpanded;
using Verse;
using Verse.Noise;

namespace RGBT.EtherealAbility
{
    internal class Ability_Summon_Tornado : VFECore.Abilities.Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            foreach (GlobalTargetInfo target in targets)
            {
                GenSpawn.Spawn(RGBDefOf.RGB_Massive_Tornado, target.Cell, CasterPawn.Map);
            }
        }
    }
}
