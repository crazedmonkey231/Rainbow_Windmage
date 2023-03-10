using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RGBT.EtherealUtil
{
    internal class TornadoUtil
    {
        public static void DrawFlatTornadoPart(Material TornadoMaterial, MaterialPropertyBlock matPropertyBlock, Vector3 position, float distanceFromCenter, float initialAngle, float speedMultiplier, Color color)
        {
            DrawPart(TornadoMaterial, matPropertyBlock, position, distanceFromCenter, initialAngle, speedMultiplier, color, 0);
        }

        public static void DrawTornadoPartWithZOffset(Material TornadoMaterial, MaterialPropertyBlock matPropertyBlock, Vector3 position, float distanceFromCenter, float initialAngle, float speedMultiplier, Color color)
        {
            DrawPart(TornadoMaterial, matPropertyBlock, position, distanceFromCenter, initialAngle, speedMultiplier, color, distanceFromCenter);
        }

        public static void DrawTornadoPartClassic(Material TornadoMaterial, MaterialPropertyBlock matPropertyBlock, Vector2 realPosition, float thingIDNumber, float ZOffsetBias, float distanceFromCenter, float initialAngle, float speedMultiplier, Color color)
        {
            int ticksGame = Find.TickManager.TicksGame;
            float num1 = 1f / distanceFromCenter;
            float num2 = 25f * speedMultiplier * num1;
            float num3 = (float)(((double)initialAngle + (double)ticksGame * (double)num2) % 360.0);
            Vector2 vector2 = realPosition.Moved(num3, AdjustedDistanceFromCenter(distanceFromCenter));
            vector2.y += distanceFromCenter * 4f;
            vector2.y += ZOffsetBias;
            Vector3 vector3_1 = new Vector3(vector2.x, AltitudeLayer.Weather.AltitudeFor() + 0.04054054f * Rand.Range(0.0f, 1f), vector2.y);
            float num4 = distanceFromCenter * 3f;
            float num6 = distanceFromCenter;
            float num7 = Mathf.InverseLerp(0.18f, 0.4f, num6);
            Vector3 vector3_2 = new Vector3(Mathf.Sin((float)ticksGame / 1000f + (float)(thingIDNumber * 10)) * 2f, 0.0f, 0.0f) * num7;
            Vector3 pos = vector3_1 + vector3_2;
            matPropertyBlock.SetColor(ShaderPropertyIDs.Color, color);
            Quaternion q = Quaternion.Euler(0.0f, num3, 0.0f);
            Vector3 s = new Vector3(num4, 1f, num4);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, q, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, TornadoMaterial, 0, (Camera)null, 0, matPropertyBlock);
        }

        private static float AdjustedDistanceFromCenter(float distanceFromCenter)
        {
            float num1 = Mathf.Min(distanceFromCenter / 8f, 1f);
            float num2 = num1 * num1;
            return distanceFromCenter * num2;
        }

        private static void DrawPart(Material TornadoMaterial, MaterialPropertyBlock matPropertyBlock, Vector3 position, float distanceFromCenter, float initialAngle, float speedMultiplier, Color color, float zOffset)
        {
            int ticksGame = Find.TickManager.TicksGame;
            float num1 = 1f / distanceFromCenter;
            float num2 = speedMultiplier * num1;
            float num3 = (float)(((double)initialAngle + (double)ticksGame * (double)num2) % 360.0);
            position.z += zOffset;
            float num4 = distanceFromCenter;
            matPropertyBlock.SetColor(ShaderPropertyIDs.Color, color);
            Quaternion q = Quaternion.Euler(0.0f, num3, 0.0f);
            Vector3 s = new Vector3(num4, 1f, num4);
            Matrix4x4 matrix = Matrix4x4.TRS(position, q, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, TornadoMaterial, 0, (Camera)null, 0, matPropertyBlock);
        }
    }
}
