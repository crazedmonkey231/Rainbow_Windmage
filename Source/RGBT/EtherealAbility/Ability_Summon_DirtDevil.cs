using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanillaPsycastsExpanded;
using Verse;

namespace RGBT.EtherealAbility
{
    public class Ability_Summon_DirtDevil : VFECore.Abilities.Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            foreach (GlobalTargetInfo target in targets)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(RGBDefOf.RGB_Tornado_Pawnkind, this.pawn.Faction);
                pawn.TryGetComp<CompBreakLink>().Pawn = this.pawn;
                pawn.training.Train(TrainableDefOf.Tameness, CasterPawn, true);
                pawn.training.Train(TrainableDefOf.Obedience, CasterPawn, true);
                pawn.training.Train(TrainableDefOf.Release, CasterPawn, true);
                pawn.training.Train(RGBDefOf.Haul, CasterPawn, true);
                pawn.training.Train(RGBDefOf.Rescue, CasterPawn, true);
                GenSpawn.Spawn(pawn, target.Cell, CasterPawn.Map, CasterPawn.Rotation);
            }
        }
    }
}
