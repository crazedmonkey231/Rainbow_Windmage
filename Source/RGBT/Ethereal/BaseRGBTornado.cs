using RGBT.EtherealUtil;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace RGBT.Ethereal
{
    [StaticConstructorOnStartup]
    public class BaseRGBTornado : ThingWithComps
    {
        private Vector2 realPosition;
        private float direction;
        private int spawnTick;
        private int leftFadeOutTicks = -1;
        private int ticksLeftToDisappear = -1;
        private Sustainer sustainer;
        private static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();
        private static ModuleBase directionNoise;
        private static readonly IntRange DurationTicks = new IntRange(2700, 10080);
        private static readonly float ZOffsetBias = -4f;
        private List<IntVec3> removedRoofsTmp = new List<IntVec3>();
        private static List<Thing> tmpThings = new List<Thing>();
        private static readonly Material TornadoMaterial = MaterialPool.MatFrom("Things/Ethereal/Tornado", ShaderDatabase.Transparent, MapMaterialRenderQueues.Tornado);
        private static List<Color> CachedColorList = ColorCache.RGBColorCache;
        private static int cachedMax = ColorCache.RGBColorCache.Count;
        private int counter = 0;

        private float FadeInOutFactor => Mathf.Min(Mathf.Clamp01((float)(Find.TickManager.TicksGame - this.spawnTick) / 120f), this.leftFadeOutTicks < 0 ? 1f : Mathf.Min((float)this.leftFadeOutTicks / 120f, 1f));

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Vector2>(ref this.realPosition, "realPosition");
            Scribe_Values.Look<float>(ref this.direction, "direction");
            Scribe_Values.Look<int>(ref this.spawnTick, "spawnTick");
            Scribe_Values.Look<int>(ref this.leftFadeOutTicks, "leftFadeOutTicks");
            Scribe_Values.Look<int>(ref this.ticksLeftToDisappear, "ticksLeftToDisappear");
            Scribe_Values.Look<int>(ref counter, "counter");
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                Vector3 vector3Shifted = this.Position.ToVector3Shifted();
                this.realPosition = new Vector2(vector3Shifted.x, vector3Shifted.z);
                this.direction = Rand.Range(0.0f, 360f);
                this.spawnTick = Find.TickManager.TicksGame;
                this.leftFadeOutTicks = -1;
                this.ticksLeftToDisappear = DurationTicks.RandomInRange;
                CachedColorList = ColorCache.RGBColorCache;
                cachedMax = ColorCache.RGBColorCache.Count;
            }
            this.CreateSustainer();
        }

        public override void Tick()
        {
            if (!this.Spawned)
                return;
            if (this.sustainer == null)
            {
                Log.Error("Tornado sustainer is null.");
                this.CreateSustainer();
            }
            this.sustainer.Maintain();
            this.UpdateSustainerVolume();
            this.GetComp<CompWindSource>().wind = 5f * this.FadeInOutFactor;
            if (this.leftFadeOutTicks > 0)
            {
                --this.leftFadeOutTicks;
                if (this.leftFadeOutTicks != 0)
                    return;
                this.Destroy(DestroyMode.Vanish);
            }
            else
            {
                if (directionNoise == null)
                    directionNoise = (ModuleBase)new Perlin(1.0 / 500.0, 2.0, 0.5, 4, 1948573612, Verse.Noise.QualityMode.Medium);
                this.direction += (float)directionNoise.GetValue((double)Find.TickManager.TicksAbs, (double)(this.thingIDNumber % 500) * 1000.0, 0.0) * 0.78f;
                this.realPosition = this.realPosition.Moved(this.direction, 0.02833333f);
                IntVec3 intVec3 = new Vector3(this.realPosition.x, 0.0f, this.realPosition.y).ToIntVec3();

                if (intVec3.InBounds(this.Map))
                {
                    this.Position = intVec3;
                    if (this.IsHashIntervalTick(15))
                        this.DamageCloseThings();
                    if (Rand.MTBEventOccurs(15f, 1f, 1f))
                        this.DamageFarThings();
                    if (this.IsHashIntervalTick(20))
                        this.DestroyRoofs();
                    if (this.ticksLeftToDisappear > 0)
                    {
                        --this.ticksLeftToDisappear;
                        if (this.ticksLeftToDisappear == 0)
                        {
                            this.leftFadeOutTicks = 120;
                            Messages.Message((string)"MessageTornadoDissipated".Translate(), (LookTargets)new TargetInfo(this.Position, this.Map), MessageTypeDefOf.PositiveEvent);
                        }
                    }

                    if (!this.IsHashIntervalTick(4) || this.CellImmuneToDamage(this.Position))
                        return;
                    FleckMaker.ThrowTornadoDustPuff(new Vector3(this.realPosition.x, 0.0f, this.realPosition.y)
                    {
                        y = AltitudeLayer.MoteOverhead.AltitudeFor()
                    } + Vector3Utility.RandomHorizontalOffset(1.5f), this.Map, Rand.Range(1.5f, 3f), CachedColorList[counter]);

                    counter++;
                    if (counter >= cachedMax)
                        counter = 0;
                }
                else
                {
                    this.leftFadeOutTicks = 120;
                    Messages.Message((string)"MessageTornadoLeftMap".Translate(), (LookTargets)new TargetInfo(this.Position, this.Map), MessageTypeDefOf.PositiveEvent);
                }
            }
        }

        public override void Draw()
        {
            Rand.PushState();
            Rand.Seed = this.thingIDNumber;
            int offset = 15;
            int max = cachedMax - offset;
            float multiplier = max / 360.0f;
            for (int i = 0; i < max; i++)
                TornadoUtil.DrawTornadoPartClassic(TornadoMaterial, matPropertyBlock, realPosition, thingIDNumber, ZOffsetBias, Rand.Range(1f, 10f), i * multiplier, Rand.Range(0.8f, 2f), CachedColorList[i + offset]);
            Rand.PopState();
        }

        private void UpdateSustainerVolume() => this.sustainer.info.volumeFactor = this.FadeInOutFactor;

        private void CreateSustainer() => LongEventHandler.ExecuteWhenFinished((Action)(() =>
        {
            this.sustainer = SoundDefOf.Tornado.TrySpawnSustainer(SoundInfo.InMap((TargetInfo)(Thing)this, MaintenanceType.PerTick));
            this.UpdateSustainerVolume();
        }));

        private void DamageCloseThings()
        {
            int num = GenRadial.NumCellsInRadius(4.2f);
            for (int index = 0; index < num; ++index)
            {
                IntVec3 intVec3 = this.Position + GenRadial.RadialPattern[index];
                if (intVec3.InBounds(this.Map) && !this.CellImmuneToDamage(intVec3))
                {
                    Pawn firstPawn = intVec3.GetFirstPawn(this.Map);
                    if (firstPawn == null || !firstPawn.Downed || !Rand.Bool)
                    {
                        float damageFactor = GenMath.LerpDouble(0.0f, 4.2f, 1f, 0.2f, intVec3.DistanceTo(this.Position));
                        this.DoDamage(intVec3, damageFactor);
                    }
                }
            }
        }

        private void DamageFarThings()
        {
            IntVec3 c = GenRadial.RadialCellsAround(this.Position, 10f, true).Where<IntVec3>((Func<IntVec3, bool>)(x => x.InBounds(this.Map))).RandomElement<IntVec3>();
            if (this.CellImmuneToDamage(c))
                return;
            this.DoDamage(c, 0.5f);
        }

        private void DestroyRoofs()
        {
            this.removedRoofsTmp.Clear();
            foreach (IntVec3 c in GenRadial.RadialCellsAround(this.Position, 4.2f, true).Where<IntVec3>((Func<IntVec3, bool>)(x => x.InBounds(this.Map))))
            {
                if (!this.CellImmuneToDamage(c) && c.Roofed(this.Map))
                {
                    RoofDef roof = c.GetRoof(this.Map);
                    if (!roof.isThickRoof && !roof.isNatural)
                    {
                        RoofCollapserImmediate.DropRoofInCells(c, this.Map);
                        this.removedRoofsTmp.Add(c);
                    }
                }
            }
            if (this.removedRoofsTmp.Count <= 0)
                return;
            RoofCollapseCellsFinder.CheckCollapseFlyingRoofs(this.removedRoofsTmp, this.Map, true);
        }

        private bool CellImmuneToDamage(IntVec3 c)
        {
            if (c.Roofed(this.Map) && c.GetRoof(this.Map).isThickRoof)
                return true;
            Building edifice = c.GetEdifice(this.Map);
            return edifice != null && edifice.def.category == ThingCategory.Building && (edifice.def.building.isNaturalRock || edifice.def == ThingDefOf.Wall && edifice.Faction == null);
        }

        private void DoDamage(IntVec3 c, float damageFactor)
        {
            tmpThings.Clear();
            tmpThings.AddRange((IEnumerable<Thing>)c.GetThingList(this.Map));
            Vector3 vector3Shifted = c.ToVector3Shifted();
            float angle = (float)(-(double)this.realPosition.AngleTo(new Vector2(vector3Shifted.x, vector3Shifted.z)) + 180.0);
            for (int index = 0; index < tmpThings.Count; ++index)
            {
                BattleLogEntry_DamageTaken entryDamageTaken = (BattleLogEntry_DamageTaken)null;
                switch (tmpThings[index].def.category)
                {
                    case ThingCategory.Pawn:
                        Pawn tmpThing = (Pawn)tmpThings[index];
                        entryDamageTaken = new BattleLogEntry_DamageTaken(tmpThing, RulePackDefOf.DamageEvent_Tornado);
                        Find.BattleLog.Add((LogEntry)entryDamageTaken);
                        if ((double)tmpThing.RaceProps.baseHealthScale < 1.0)
                            damageFactor *= tmpThing.RaceProps.baseHealthScale;
                        if (tmpThing.RaceProps.Animal)
                            damageFactor *= 0.75f;
                        if (tmpThing.Downed)
                        {
                            damageFactor *= 0.2f;
                            break;
                        }
                        break;
                    case ThingCategory.Item:
                        damageFactor *= 0.68f;
                        break;
                    case ThingCategory.Building:
                        damageFactor *= 0.8f;
                        break;
                    case ThingCategory.Plant:
                        damageFactor *= 1.7f;
                        break;
                }
                int num = Mathf.Max(GenMath.RoundRandom(30f * damageFactor), 1);
                tmpThings[index].TakeDamage(new DamageInfo(DamageDefOf.TornadoScratch, (float)num, angle: angle, instigator: ((Thing)this))).AssociateWithLog((LogEntry_DamageResult)entryDamageTaken);
            }
            tmpThings.Clear();
        }
    }
}
