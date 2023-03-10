using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VanillaPsycastsExpanded.Skipmaster;
using Verse;
using Verse.Noise;

namespace RGBT.EtherealAbility
{
    internal class Ability_RGBWallraise : VFECore.Abilities.Ability
    {
        public AbilityExtension_RGBWallRaise Props => this.def.GetModExtension<AbilityExtension_RGBWallRaise>();

        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            foreach (GlobalTargetInfo target1 in targets)
            {
                Map map = target1.Map;
                LocalTargetInfo target2 = target1.HasThing ? new LocalTargetInfo(target1.Thing) : new LocalTargetInfo(target1.Cell);
                List<Thing> thingList = new List<Thing>();
                thingList.AddRange(this.Props.AffectedCells(target2, map).SelectMany(c => c.GetThingList(map).Where(t => t.def.category == ThingCategory.Item)));
                foreach (Entity entity in thingList)
                    entity.DeSpawn();
                foreach (IntVec3 affectedCell in this.Props.AffectedCells(target2, map))
                {
                    GenSpawn.Spawn(Props.wallDef, affectedCell, map);
                    FleckMaker.ThrowDustPuffThick(affectedCell.ToVector3Shifted(), map, Rand.Range(1.5f, 3f), CompAbilityEffect_Wallraise.DustColor);
                }
                foreach (Thing thing in thingList)
                {
                    IntVec3 loc = IntVec3.Invalid;
                    for (int index = 0; index < 9; ++index)
                    {
                        IntVec3 c = thing.Position + GenRadial.RadialPattern[index];
                        if (c.InBounds(map) && c.Walkable(map) && map.thingGrid.ThingsListAtFast(c).Count <= 0)
                        {
                            loc = c;
                            break;
                        }
                    }
                    if (loc != IntVec3.Invalid)
                        GenSpawn.Spawn(thing, loc, map);
                    else
                        GenPlace.TryPlaceThing(thing, thing.Position, map, ThingPlaceMode.Near);
                }
            }
        }

        public override void DrawHighlight(LocalTargetInfo target)
        {
            base.DrawHighlight(target);
            GenDraw.DrawFieldEdges(this.Props.AffectedCells(target, this.pawn.Map).ToList<IntVec3>(), this.ValidateTarget(target, false) ? Color.white : Color.red);
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = false)
        {
            if (this.Props.AffectedCells(target, this.pawn.Map).Any<IntVec3>((Func<IntVec3, bool>)(c => c.Filled(this.pawn.Map))))
            {
                if (showMessages)
                    Messages.Message((string)"AbilityOccupiedCells".Translate((NamedArgument)this.def.LabelCap), (LookTargets)target.ToTargetInfo(this.pawn.Map), MessageTypeDefOf.RejectInput, false);
                return false;
            }
            if (!this.Props.AffectedCells(target, this.pawn.Map).Any<IntVec3>((Func<IntVec3, bool>)(c => !c.Standable(this.pawn.Map))))
                return true;
            if (showMessages)
                Messages.Message((string)"AbilityUnwalkable".Translate((NamedArgument)this.def.LabelCap), (LookTargets)target.ToTargetInfo(this.pawn.Map), MessageTypeDefOf.RejectInput, false);
            return false;
        }
    }
}
