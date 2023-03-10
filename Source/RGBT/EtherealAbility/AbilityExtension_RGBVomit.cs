using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace RGBT.EtherealAbility
{
    public class AbilityExtension_RGBVomit : AbilityExtension_AbilityMod
    {
        public override void Cast(GlobalTargetInfo[] targets, Ability ability)
        {
            foreach (GlobalTargetInfo target in targets)
            {
                if (target.Thing is Pawn)
                    (target.Thing as Pawn).jobs.StartJob(JobMaker.MakeJob(RGBDefOf.RainbowVomitJob), JobCondition.InterruptForced, resumeCurJobAfterwards: true);  
            }
        }
    }
}

