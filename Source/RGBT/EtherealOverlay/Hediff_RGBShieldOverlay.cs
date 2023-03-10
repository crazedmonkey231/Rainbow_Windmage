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
using Verse;
using VFECore;

namespace RGBT.EtherealOverlay
{

    [StaticConstructorOnStartup]
    public class Hediff_RGBShieldOverlay : Hediff_Overlay
    {
        public override string OverlayPath => "Other/ShieldBubble";
        private static readonly Material TornadoMaterial = MaterialPool.MatFrom("Things/Ethereal/Tornado", ShaderDatabase.Transparent, MapMaterialRenderQueues.Tornado);
        private static readonly float size = RGBDefOf.Wind_Wall_II_Omni_Wall.radius;
        private static float doubleSize = size * 2;
        private static Vector3 cachedSize = new Vector3(doubleSize, 1f, doubleSize);
        private static Material material;
        private static List<Color> CachedColorList = ColorCache.RGBColorCache;
        private static int colorMaxCount = ColorCache.RGBColorCache.Count;
        private int counter = 0;
        private Color prevColor;
        private Color newColor;

        public override void Tick()
        {
            base.Tick();
            if (pawn.Map == null)
                return;
            CheckAndDestroyProjectiles();
            UpdateCache();
        }

        private void CheckAndDestroyProjectiles()
        {
            IEnumerable<Projectile> projectilesAround = RadialUtil.GetRadialProjectilesAroundCenter(pawn.Position, pawn.Map, doubleSize);
            if (projectilesAround.Count() > 0)
                foreach (Projectile thing in projectilesAround)
                    DestroyProjectile(thing);
        }

        private void UpdateCache()
        {
            prevColor = CachedColorList[counter];
            counter++;
            if (counter >= colorMaxCount)
                counter = 0;
            newColor = CachedColorList[counter];
        }

        public override void Draw()
        {
            base.Draw();
            Matrix4x4 matrix = new Matrix4x4();
            Vector3 drawPos = pawn.DrawPos;
            drawPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            matrix.SetTRS(drawPos, Quaternion.identity, cachedSize);
            Color color = Color.Lerp(prevColor, newColor, 0.5f);
            MatPropertyBlock.SetColor(ShaderPropertyIDs.Color, color);
            TornadoUtil.DrawFlatTornadoPart(TornadoMaterial, MatPropertyBlock, drawPos, doubleSize, counter * 3.6f, 50f, color);
            TornadoUtil.DrawFlatTornadoPart(TornadoMaterial, MatPropertyBlock, drawPos, doubleSize, counter * 3.6f + 180.0f, 50f, color);
            Graphics.DrawMesh(MeshPool.plane10, matrix, OverlayMat, 0, (Camera)null, 0, MatPropertyBlock);
        }

        public new Material OverlayMat
        {
            get
            {
                if (material == null)
                    material = MaterialPool.MatFrom(OverlayPath, ShaderDatabase.MoteGlow);
                return material;
            }
        }

        private void DestroyProjectile(Projectile projectile)
        {
            Effecter effecter = new Effecter(EffecterDefOf.Deflect_General);
            effecter.Trigger(new TargetInfo(projectile.Position, pawn.Map), TargetInfo.Invalid);
            effecter.Cleanup();
            projectile.Destroy(DestroyMode.Vanish);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref counter, "counter");
            Scribe_Values.Look<Color>(ref prevColor, "prevColor");
            Scribe_Values.Look<Color>(ref newColor, "newColor");
        }
    }
}
