using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using VFECore.Abilities;

namespace RGBT.EtherealAbility
{
    public class Ability_ShootBurstProjectile : Ability
    {
        public AbilityExtension_BurstProjectile Props => def.GetModExtension<AbilityExtension_BurstProjectile>();

        public override void Cast(params GlobalTargetInfo[] targets)
        {
            foreach (GlobalTargetInfo target in targets)
            {
                    ShootProjectiles(target);
            }
        }

        protected virtual List<Projectile> ShootProjectiles(GlobalTargetInfo target)
        {
            List <Projectile> projectiles = new List <Projectile>();
            int count = Props.projectiles.Count;
            for(int i = 0; i < count; i++)
            {
                Projectile projectile = GenSpawn.Spawn(Props.projectiles[i], pawn.Position, pawn.Map) as Projectile;
                AbilityProjectile abilityProjectile = projectile as AbilityProjectile;
                if (abilityProjectile != null)
                {
                    abilityProjectile.ability = this;
                    projectiles.Add(abilityProjectile);
                }

                Vector3 randOffset = new Vector3();
                if (Props.hasRandomOffset)
                {
                    randOffset.x = Rand.Range(-1f, 1f);
                    randOffset.y = Rand.Range(-1f, 1f);
                    randOffset.z = Rand.Range(-1f, 1f);
                }

                if (target.HasThing)
                    projectile?.Launch(pawn, pawn.DrawPos + randOffset, target.Thing, target.Thing, ProjectileHitFlags.IntendedTarget);
                else
                    projectile?.Launch(pawn, pawn.DrawPos + randOffset, target.Cell, target.Cell, ProjectileHitFlags.IntendedTarget);
            }
            return projectiles;
        }
        public override void CheckCastEffects(GlobalTargetInfo[] targetInfos, out bool cast, out bool target, out bool hediffApply)
        {
            base.CheckCastEffects(targetInfos, out cast, out var _, out var _);
            target = false;
            hediffApply = false;
        }
    }
}
