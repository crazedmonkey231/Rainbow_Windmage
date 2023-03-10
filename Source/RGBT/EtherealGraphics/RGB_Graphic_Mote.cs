using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RGBT.EtherealGraphics
{
    public class RGB_Graphic_Mote : Graphic_Mote
    {
        private int counter = 0;

        protected override bool ForcePropertyBlock => true;

        public override void DrawWorker(
          Vector3 loc,
          Rot4 rot,
          ThingDef thingDef,
          Thing thing,
          float extraRotation)
        {
            int? ticksGame = Current.Game?.tickManager?.TicksGame;
            float num = ticksGame.HasValue ? ticksGame.GetValueOrDefault() : 0.0f;
            Log.Message("counter {0}".Formatted(counter));
            CheckCounter((RGBGraphicData)data, num);
            DoMote(thing, ColorCache.RGBColorCache[counter]);
        }

        private void CheckCounter(RGBGraphicData data, float num)
        {
            if (num % data.ticksPerFrame == 0)
            {
                counter++;
                if (counter == ColorCache.SIZE)
                    counter = 0;
            }
        }

        private void DoMote(Thing thing, Color color)
        {
            Mote mote = (Mote)thing;
            float alpha = mote.Alpha;
            if (!(alpha <= 0f))
            {
                color.a *= alpha;
                Vector3 exactScale = mote.exactScale;
                exactScale.x *= data.drawSize.x;
                exactScale.z *= data.drawSize.y;
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(mote.DrawPos, Quaternion.AngleAxis(mote.exactRotation, Vector3.up), exactScale);
                Material matSingle = MatSingle;
                if (!ForcePropertyBlock && color.IndistinguishableFrom(matSingle.color))
                {
                    Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, 0, null, 0);
                    return;
                }

                propertyBlock.SetColor(ShaderPropertyIDs.Color, color);
                Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, 0, null, 0, propertyBlock);
            }
        }
    }
}
