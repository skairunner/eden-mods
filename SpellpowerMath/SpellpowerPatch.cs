using System;
using Clarity;
using HarmonyLib;
using UnityEngine;

namespace SpellpowerMath
{
    public static class SpellpowerMathPatch
    {
        public static float CalcDamage(float baseDamage, float spellpower)
        {
            var flatBoost = spellpower;
            var multBoost = baseDamage * .01f * spellpower;
            Log.WriteLine($"Base dmg {baseDamage}. Flat boost {spellpower}. Mult boost {multBoost}");
            return flatBoost > multBoost ? flatBoost : multBoost;
        }
        
        [HarmonyPatch(typeof(BC), "GetAmount", 
            typeof(AmountApp), typeof(float), typeof(SpellObject), typeof(ArtifactObject), typeof(PactObject), typeof(bool))]
        public static class BC_GetAmount
        {
            public static float Postfix(float __result, BC __instance, AmountApp amtApp, SpellObject spellObj, ArtifactObject artObj, PactObject pactObj)
            {
                if (amtApp.type != AmountType.Damage || amtApp.type != AmountType.ParentDamage)
                {
                    return __result;
                }
                
                // Implement our own damage formula
                ItemObject itemObject = (ItemObject) spellObj ?? artObj ?? pactObj;

                float baseAmount = amtApp.type == AmountType.Damage ? 
                    __instance.GetAmount(spellObj.damageType, spellObj.damage, spellObj) :
                    __instance.GetAmount(spellObj.parentSpell.damageType, spellObj.parentSpell.damage, spellObj);
                
                float amount1 = baseAmount;
                
                if ((bool) itemObject.being.player)
                {
                    amount1 += CalcDamage(baseAmount, itemObject.being.player.spellPower);
                }

                var num1 = amount1 + CalcDamage(amount1, itemObject.being.GetAmount(Status.SpellPower));

                if (amtApp.type == AmountType.Damage)
                {
                    num1 += spellObj.tempDamage + spellObj.permDamage;
                } 

                return num1 * amtApp.multiplier + amtApp.initial;
            }
        }

        [HarmonyPatch(typeof(ProjectileFactory), "CreateProjectile")]
        public static class ProjectileFactory_CreateProjectile
        {
            public static Projectile Postfix(Projectile __result, ProjectileFactory __instance, SpellObject spellObj)
            {
                var proj = __result;
                var being = spellObj.being;
                
                if (!spellObj.tags.Contains(Tag.Weapon) && !spellObj.tags.Contains(Tag.Drone))
                {
                    if (proj.damage > 0)
                    {
                        var totalSpellpower = being.GetAmount(Status.SpellPower);
                        if ((bool) being.player)
                            totalSpellpower += being.player.spellPower;
                        // unadjust the damage
                        proj.damage -= totalSpellpower;
                        proj.damage += (int)CalcDamage(proj.damage, totalSpellpower);
                        if (proj.damage < 1)
                            proj.damage = 1;
                    }
                }

                return proj;
            }
        }
    }
}
