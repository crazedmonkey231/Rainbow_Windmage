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

namespace RGBT.EtherealPawn
{
    [StaticConstructorOnStartup]
    public class TornadoPawn : Pawn
    {
        private static readonly Material TornadoMaterial = MaterialPool.MatFrom("Things/Ethereal/Tornado", ShaderDatabase.Mote, MapMaterialRenderQueues.Tornado);
        private static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();
        private int spawnTick = -1;
        private static List<Color> CachedSettingsColor = ColorCache.RGBColorCache;
        private static int cachedMax = ColorCache.RGBColorCache.Count;
        private int counter = 0;

        public override void Tick()
        {
            base.Tick();
            counter++;
            if (counter == cachedMax)
                counter = 0;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.spawnTick, "spawnTick");
            Scribe_Collections.Look<Color>(ref CachedSettingsColor, "CachedSettingsColor");
            Scribe_Values.Look<int>(ref cachedMax, "cachedMax");
            Scribe_Values.Look<int>(ref counter, "counter");
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                spawnTick = Find.TickManager.TicksGame;
                CachedSettingsColor = ColorCache.RGBColorCache;
                cachedMax = ColorCache.RGBColorCache.Count;
            }
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            Drawer.DrawAt(drawLoc);
            for (int i = 0; i < 10; i++)
                TornadoUtil.DrawTornadoPartWithZOffset(TornadoMaterial, matPropertyBlock, drawLoc, Rand.Range(0f, 1.5f), i * 36.0f, 1.5f, CachedSettingsColor[i * 10]);
        }
    }
}
