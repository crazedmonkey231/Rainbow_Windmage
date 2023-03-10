using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RGBT
{

    [StaticConstructorOnStartup]
    public class ColorCache : IExposable
    {

        public static List<Color> RGBColorCache = new List<Color>();
        public static readonly int SIZE = 100;

        static ColorCache()
        {
            MakeRGBColor();
        }

        private static void MakeRGBColor()
        {

            for (float w = 0; w < SIZE; w++)
            {
                float R = 0;
                float G = 0;
                float B = 0;
                w %= 100;

                if (w < 17)
                {
                    R = -(w - 17.0f) / 17.0f;
                    G = 1.0f;
                }
                else if (w < 33)
                {
                    G = (w - 17.0f) / (33.0f - 17.0f);
                    B = 1.0f;
                }
                else if (w < 50)
                {
                    G = 1.0f;
                    B = -(w - 50.0f) / (50.0f - 33.0f);
                }
                else if (w < 67)
                {
                    R = (w - 50.0f) / (67.0f - 50.0f);
                    G = 1.0f;
                }
                else if (w < 83)
                {
                    R = 1.0f;
                    G = -(w - 83.0f) / (83.0f - 67.0f);
                }
                else
                {
                    R = 1.0f;
                    B = (w - 83.0f) / (100.0f - 83.0f);
                }
                Color newColor = new Color(R, G, B);
                RGBColorCache.Add(newColor);
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.Look<Color>(ref RGBColorCache, "RGBColorCache");
        }
    }
}
