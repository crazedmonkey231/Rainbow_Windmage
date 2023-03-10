using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RGBT.EtherealGraphics
{
    public class RGB_Graphic_Fleck : Graphic_Fleck
    {

        private int counter = 0;

        public override void DrawFleck(FleckDrawData drawData, DrawBatch batch)
        {
            RGBGraphicData data = (RGBGraphicData)this.data;
            int? ticksGame = Current.Game?.tickManager?.TicksGame;
            float num = ticksGame.HasValue ? (float)ticksGame.GetValueOrDefault() : 0.0f;
            DoFleck(data, drawData, batch, ColorCache.RGBColorCache[counter]);
            CheckCounter(data, num);
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

        private void DoFleck(RGBGraphicData data, FleckDrawData drawData, DrawBatch batch, Color color)
        {
            color *= drawData.color;
            color.a *= data.opacity;
            Vector3 scale = drawData.scale;
            scale.x *= this.data.drawSize.x;
            scale.z *= this.data.drawSize.y;
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(drawData.pos, Quaternion.AngleAxis(drawData.rotation, Vector3.up), scale);
            Material matSingle = this.MatSingle;
            batch.DrawMesh(MeshPool.plane10, matrix, matSingle, drawData.drawLayer, color, this.data.renderInstanced && this.AllowInstancing, drawData.propertyBlock);
        }
    }
}
