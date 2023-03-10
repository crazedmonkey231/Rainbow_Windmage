using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RGBT.EtherealGraphics
{
    public class RGB_Graphic_Cluster : Graphic_Cluster
    {
        public override void Print(SectionLayer layer, Thing thing, float extraRotation)
        {
            Vector3 vector3 = thing.TrueCenter();
            Rand.PushState();
            Rand.Seed = thing.Position.GetHashCode();
            int num = thing is Filth filth ? filth.thickness : 3;
            for (int index = 0; index < num; ++index)
            {
                Material material = this.MatSingle;
                Vector3 center = vector3 + new Vector3(Rand.Range(-0.45f, 0.45f), 0.0f, Rand.Range(-0.45f, 0.45f));
                Vector2 size = new Vector2(Rand.Range(this.data.drawSize.x * 0.8f, this.data.drawSize.x * 1.2f), Rand.Range(this.data.drawSize.y * 0.8f, this.data.drawSize.y * 1.2f));
                float rot = (float)Rand.RangeInclusive(0, 360) + extraRotation;
                bool flipUv = (double)Rand.Value < 0.5;
                Vector2[] uvs;
                Graphic.TryGetTextureAtlasReplacementInfo(material, thing.def.category.ToAtlasGroup(), flipUv, true, out material, out uvs, out _);
                Printer_Plane.PrintPlane(layer, center, size, material, rot, (flipUv ? 1 : 0) != 0, uvs, new Color32[4]
                {
                    ColorCache.RGBColorCache.RandomElement(),
                    ColorCache.RGBColorCache.RandomElement(),
                    ColorCache.RGBColorCache.RandomElement(),
                    ColorCache.RGBColorCache.RandomElement()
                });

            }
            Rand.PopState();
        }

    }
}
