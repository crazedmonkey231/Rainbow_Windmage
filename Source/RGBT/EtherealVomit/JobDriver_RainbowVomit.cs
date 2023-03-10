using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

namespace RGBT.EtherealVomit
{
    internal class JobDriver_RainbowVomit : JobDriver
    {
        private int ticksLeft;

        public override void SetInitialPosture()
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksLeft, "ticksLeft", 0);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil toil = new Toil
            {
                initAction = delegate
            {
                ticksLeft = Rand.Range(300, 900);
                int num = 0;
                IntVec3 intVec;
                do
                {
                    intVec = pawn.Position + GenAdj.AdjacentCellsAndInside[Rand.Range(0, 9)];
                    num++;
                    if (num > 12)
                    {
                        intVec = pawn.Position;
                        break;
                    }
                }
                while (!intVec.InBounds(pawn.Map) || !intVec.Standable(pawn.Map));
                job.targetA = intVec;
                pawn.pather.StopDead();
            },
                tickAction = delegate
                {
                    if (ticksLeft % 150 == 149)
                    {
                        FilthMaker.TryMakeFilth(job.targetA.Cell, base.Map, RGBDefOf.Rainbow_Filth_Vomit, pawn.LabelIndefinite());
                        if (pawn.needs.food.CurLevelPercentage > 0.1f)
                        {
                            pawn.needs.food.CurLevel -= pawn.needs.food.MaxLevel * 0.04f;
                        }
                    }

                    ticksLeft--;
                    if (ticksLeft <= 0)
                    {
                        ReadyForNextToil();
                        TaleRecorder.RecordTale(TaleDefOf.Vomited, pawn);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Never
            };
            toil.WithEffect(RGBDefOf.RainbowVomitEffect, TargetIndex.A);
            toil.PlaySustainerOrSound(() => SoundDefOf.Vomit);
            yield return toil;
        }
    }
}
