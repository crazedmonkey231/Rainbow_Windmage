using RGBT.EtherealUtil;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VanillaPsycastsExpanded;
using VanillaPsycastsExpanded.Conflagrator;
using Verse;

namespace RGBT.EtherealOverlay
{
    [StaticConstructorOnStartup]
    public class Hediff_RGBOverlay : Hediff_Overlay
    {
        private static float size = RGBDefOf.WindSlash_IV_Omni_Slash.radius;
        private static float doubleSize = size * 2;
        private static readonly Material material = MaterialPool.MatFrom("Things/Ethereal/Tornado", ShaderDatabase.Transparent, MapMaterialRenderQueues.Tornado);
        private static int cachedMax = ColorCache.RGBColorCache.Count;
        private int counter = 0;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref counter, "counter");
        }

        public override void Tick()
        {
            base.Tick();
            counter++;
            if (counter >= cachedMax)
                counter = 0;
        }

        public override void Draw()
        {
            base.Draw();
            for (int i = 0; i < 5; i++)
                TornadoUtil.DrawFlatTornadoPart(material, MatPropertyBlock, pawn.DrawPos, doubleSize, i * 60.0f, 50f, ColorCache.RGBColorCache[counter]);

        }
    }
}
