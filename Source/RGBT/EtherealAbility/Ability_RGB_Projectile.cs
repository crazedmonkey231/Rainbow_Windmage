using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using VFECore.Abilities;

namespace RGBT.EtherealAbility
{
    internal class Ability_RGB_Projectile : Ability_ShootProjectile
    {
        public AbilityExtension_Projectile Props => def.GetModExtension<AbilityExtension_Projectile>();
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
        }

        protected override Projectile ShootProjectile(GlobalTargetInfo target)
        {
            Projectile projectile = GenSpawn.Spawn(def.GetModExtension<AbilityExtension_Projectile>().projectile, this.pawn.Position, this.pawn.Map) as Projectile;
            if (projectile is AbilityProjectile abilityProjectile)
                abilityProjectile.ability = (Ability)this;
            if (target.HasThing)
                projectile?.Launch((Thing)this.pawn, this.pawn.DrawPos, (LocalTargetInfo)target.Thing, (LocalTargetInfo)target.Thing, ProjectileHitFlags.IntendedTarget);
            else
                projectile?.Launch((Thing)this.pawn, this.pawn.DrawPos, (LocalTargetInfo)target.Cell, (LocalTargetInfo)target.Cell, ProjectileHitFlags.IntendedTarget);
            return projectile;
        }  
    }
}
