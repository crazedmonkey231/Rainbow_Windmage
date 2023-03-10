using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RGBT.EtherealUtil
{
    internal class RadialUtil
    {
        public static IEnumerable<Projectile> GetRadialProjectilesAroundCenter(IntVec3 center, Map map, float radius)
        {
            int numCells = GenRadial.NumCellsInRadius(radius);
            for (int i = 0; i < numCells; i++)
            {
                IntVec3 c = GenRadial.RadialPattern[i] + center;
                if (!c.InBounds(map))
                    continue;
                List<Thing> thingList = c.GetThingList(map);
                int count = thingList.Count;
                for (int j = 0; j < count; j++)
                {
                    if (thingList[j] is Projectile)
                        yield return thingList[j] as Projectile;
                }
            }
        }
    }
}
