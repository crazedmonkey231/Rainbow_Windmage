using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using VFECore.Abilities;

namespace RGBT.EtherealAbility
{
    public class AbilityExtension_BurstProjectile : AbilityExtension_AbilityMod
    {

        public List<ThingDef> projectiles;
        public bool hasRandomOffset = false;
        
    }
}
