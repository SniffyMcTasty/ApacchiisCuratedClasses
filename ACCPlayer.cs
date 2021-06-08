using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using ApacchiisClassesMod;
using Microsoft.Xna.Framework;

namespace ApacchiisCuratedClasses
{
	public class ACCPlayer : ModPlayer
	{
        //For weak referenced cross-mod content 
        Mod ExpSentriesMod = ModLoader.GetMod("ExpandedSentries");

        // Main mod Misc. Variables
        public bool hasEquipedClass;
        public bool hasClassPath1;
        public bool hasClassPath2;

        //Explorer
        public bool hasExplorer;
        int explorerDodgeTimer = 0;
        bool explorerDodgeHeal = false;
        int explorerPassiveTimer = 180;
        public bool explorerThrownTeleporter = false;
        public Vector2 explorerTeleporterPos; //This variable is the one updated by the "ExplorerTeleporter" projectile

        //Spellblade
        public bool hasSpellblade;
        public int magicBladeBaseCost;
        public int magicBladeBaseDamage;
        public int shokkZoneTimerBase= 50;
        public bool spellbladeToggle = false;

        //Defender
        public bool hasDefender;
        public int defenderAbility1Damage = 0; //Detonate Sentries base damage
        int defenderPassiveTimer = 0; //Passive buff timer
        int defenderPassiveBoost = 0; //How much defense is given after using A1 based on sentries detonated
        int defenderPoweredTimer = 0;

        public override void ResetEffects()
        {
            hasClassPath1 = false;
            hasClassPath2 = false;

            // Resetting class variables to their default so when the player unnequips the class the class' variables are reset
            hasExplorer = false;
            hasSpellblade = false;
            hasDefender = false;

            base.ResetEffects();
        }

        public override void PreUpdateBuffs()
        {
            #region Explorer Ability 1 Timers & Effects
            // Here we increase or decrease timers
            if (explorerDodgeTimer > 0) // So if the Dodge Timer is above 0
            {
                explorerDodgeTimer--; // Decrease it 1 by 1 each tick
                 
                player.invis = true;
                player.velocity = new Vector2(0, 0);
                player.moveSpeed = 0f;
                player.slowFall = true;
            }

            if (explorerPassiveTimer < 180) // If the passive timer is below 180 (3 seconds)
                explorerPassiveTimer++; // Increase it 1 by 1 each tick
            #endregion


            #region Defender Ability 2 Timers & Effects
            if (ExpSentriesMod != null)
            {
                if (defenderPoweredTimer > 0)
                {
                    defenderPoweredTimer--;
                    player.GetModPlayer<ExpandedSentries.ESPlayer>().sentryRange += 1f;
                    player.GetModPlayer<ExpandedSentries.ESPlayer>().sentrySpeed += 0.5f;
                }
            }
            #endregion

            base.PreUpdateBuffs();
        }

        public override void PostUpdateBuffs()
        {
            #region Explorer Purge
            if (explorerDodgeTimer > 0) // If the dodge timer is above 0
            {
                // Making the player immune to the debuffs will clear them, and wont allow them to be re-applied for as long as we are dodging
                player.buffImmune[BuffID.Bleeding] = true;
                player.buffImmune[BuffID.Poisoned] = true;
                player.buffImmune[BuffID.OnFire] = true;
                player.buffImmune[BuffID.Venom] = true;
                player.buffImmune[BuffID.Darkness] = true;
                player.buffImmune[BuffID.Blackout] = true;
                player.buffImmune[BuffID.Silenced] = true;
                player.buffImmune[BuffID.Cursed] = true;
                player.buffImmune[BuffID.Confused] = true;
                player.buffImmune[BuffID.Silenced] = true;
                player.buffImmune[BuffID.Slow] = true;
                player.buffImmune[BuffID.OgreSpit] = true;
                player.buffImmune[BuffID.Weak] = true;
                player.buffImmune[BuffID.BrokenArmor] = true;
                player.buffImmune[BuffID.WitheredArmor] = true;
                player.buffImmune[BuffID.WitheredWeapon] = true;
                player.buffImmune[BuffID.CursedInferno] = true;
                player.buffImmune[BuffID.Ichor] = true;
                player.buffImmune[BuffID.Frostburn] = true;
                player.buffImmune[BuffID.Chilled] = true;
                player.buffImmune[BuffID.Frozen] = true;
                player.buffImmune[BuffID.Webbed] = true;
                player.buffImmune[BuffID.Stoned] = true;
                player.buffImmune[BuffID.VortexDebuff] = true;
                player.buffImmune[BuffID.Electrified] = true;
            }
            #endregion
            base.PostUpdateBuffs();
        }

        public override void PostUpdateEquips()
        {
            #region Spellblade Ability 1
            if(hasSpellblade)
            {
                if (player.HeldItem.melee)
                    player.statDefense += (int)(player.magicCrit * .4f);
                
                player.meleeDamage += player.magicDamage;

                if(spellbladeToggle)
                {
                    player.manaRegen = 0;
                    player.manaRegenBonus = 0;
                    player.manaRegenCount = 0;
                    player.manaRegenDelayBonus = 0;

                    if (player.HeldItem.melee)
                        player.HeldItem.mana = magicBladeBaseCost;
                }
                else
                {
                    if (player.HeldItem.melee)
                        player.HeldItem.mana = 0;
                }
            }
            #endregion
            base.PostUpdateEquips();
        }

        //Giving the player defense didn't work in the above functions
        public override void PostUpdate()
        {
            #region Defender Passive
            if (ExpSentriesMod != null)
            {
                //Passive to increase defense either based on active turrets or how many turrets were detonated using A1
                if (hasDefender)
                {
                    if (defenderPassiveTimer > 0) //Scale only with this stat if A1 was used recently
                    {
                        defenderPassiveTimer--;
                        player.statDefense += defenderPassiveBoost;
                        if (defenderPassiveTimer <= 0)
                        {
                            defenderPassiveBoost = 0;
                        }
                    }
                    else //Otherwise, scale with active sentries
                    {
                        int turretCount = 0;
                        for (int j = 0; j < 1000; j++)
                        {
                            if (Main.projectile[j].active && Main.projectile[j].owner == player.whoAmI && Main.projectile[j].sentry)
                            {
                                turretCount++;
                            }
                        }
                        if (turretCount > 0)
                        {
                            player.statDefense += turretCount;
                        }
                    }
                }
            }
            #endregion
            base.PostUpdate();
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            var acmPlayer = player.GetModPlayer<ApacchiisClassesMod.MyPlayer>();

            #region Explorer Dodge Heal
            if (explorerDodgeTimer > 0 && !explorerDodgeHeal) // If the timer is above 0, and we have not yet healed from dodging
            {
                damage = 0; // Set the damage taken to 0 (this will actually be 1, since damage taken cannot be below 1)
                player.statLife += (int)(player.statLifeMax2 * .06f * acmPlayer.abilityDamage + 1 ); // Heal for 4% of max health * ability power, +1 for the damage we took when dodging
                player.HealEffect((int)(player.statLifeMax2 * .06f * acmPlayer.abilityDamage + 1)); // Display the Heal Effect for the same value (green numbers above the player's head when healed)
                explorerDodgeHeal = true; // We have now healed from dodging, so this can no longer happen until we re-use the ability
            }
            #endregion
            base.ModifyHitByNPC(npc, ref damage, ref crit);
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            var acmPlayer = player.GetModPlayer<ApacchiisClassesMod.MyPlayer>();

            #region Explorer Dodge Heal
            if (explorerDodgeTimer > 0 && !explorerDodgeHeal) // If the timer is above 0, and we have not yet healed from dodging
            {
                damage = 0; // Set the damage taken to 0 (this will actually be 1, since damage taken cannot be below 1)
                player.statLife += (int)(player.statLifeMax2 * .04f * acmPlayer.abilityDamage + 1); // Heal for 4% of max health * ability power, +1 for the damage we took when dodging
                player.HealEffect((int)(player.statLifeMax2 * .04f * acmPlayer.abilityDamage + 1)); // Display the Heal Effect for the same value (green numbers above the player's head when healed)
                explorerDodgeHeal = true; // We have now healed from dodging, so this can no longer happen until we re-use the ability
            }
            #endregion
            base.ModifyHitByProjectile(proj, ref damage, ref crit);
        }

        // Modify what happens right before we hit an NPC
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            #region Explorer Passive
            if (hasExplorer) // If our currently equipped class is Explorer
            {
                if(explorerPassiveTimer == 180) // If the timer is at 180 ticks (3 seconds)
                {
                    explorerPassiveTimer = 0; // Reset the timer back to 0
                    damage = (int)(damage * 1.15f); // Multiply the damage by 15%
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 25, player.width, player.height), new Color(255, 255, 255), "!", true); // Small visual feedback that the passive was used
                }                                                                                                                                                             // Displays a white "!" on top of the player
            }
            #endregion
            base.ModifyHitNPC(item, target, ref damage, ref knockback, ref crit);
        }

        // Modify what happens right before we hit an NPC with a Projectile
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            #region Explorer Passive
            if (hasExplorer) // If our currently equipped class is Explorer
            {
                if (explorerPassiveTimer == 180) // If the timer is at 180 ticks (3 seconds)
                {
                    explorerPassiveTimer = 0; // Reset the timer back to 0
                    damage = (int)(damage * 1.15f); // Multiply the damage by 15%
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 25, player.width, player.height), new Color(255, 255, 255), "!", true); // Small visual feedback that the passive was used
                }                                                                                                                                                             // Displays a white "!" on top of the player
            }
            #endregion
            base.ModifyHitNPCWithProj(proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
        }

        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            #region Spellblade
            if (hasSpellblade && spellbladeToggle && item.melee)
                item.mana = 10;
            #endregion
            base.ModifyManaCost(item, ref reduce, ref mult);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            var acmPlayer = player.GetModPlayer<ApacchiisClassesMod.MyPlayer>();

            /* <-NOTES->
             * - acmPlayer.abilityDamage is how Ability Power is named internally, its not just for damage, it affects everything.
             * 
             * - acmPlayer.baseCooldown is 60 ticks by default but can be changed via Mod Configs.
            */

            // If the main mod's ability 1 cooldown debuff is NOT currently active, run all the code below this line
            if (ACM.ClassAbility1.JustPressed && player.FindBuffIndex(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown1>()) == -1 && Main.myPlayer == player.whoAmI)
            {
                #region Explorer
                if (hasExplorer) // If our currently equipped class is Explorer
                {
                    if (!explorerThrownTeleporter) // And we haven't thrown out our teleporter
                    {
                        float APValue = .4f; // Percentage of Ability Power scaling, 0.4f = 40%, meaning the ability will only scale with 40% of the player's Ability Power
                        float velMultiplier = (acmPlayer.abilityDamage - 1f) * APValue + 1f; // You can just copy/paste these 2 lines and change APValue to whatever you want for your class

                        // Grab mouse position compared to the player's position and normalize velocity
                        Vector2 vel = Main.MouseWorld - player.position;
                        vel.Normalize();
                        vel *= 2 * velMultiplier; // The lower this value, the lower the speed at which the projectile moves

                        

                        // Spawn the teleporter projectile towards mouse position                   V      V                                                            V
                        var tp = Projectile.NewProjectile(player.Center.X, player.position.Y - 10, vel.X, vel.Y, ModContent.ProjectileType<Projectiles.Explorer.ExplorerTeleporter>(), 0, 0, player.whoAmI);
                        explorerThrownTeleporter = true; // We have now thrown our teleporter
                    }
                    else // If we have already thrown out our teleporter
                    {
                        if (hasClassPath1) // And we have the class' Path 1
                            player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown1>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * 35)); // Add a 35 second cooldown
                        else                                                                                                                                                    //       Otherwise
                            player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown1>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * 42)); // Add a 42 second cooldown

                        player.Teleport(explorerTeleporterPos); // Set the player's position to a Vector2 variable thats updated every tick by the "ExplorerTeleporter" projectile.
                        explorerThrownTeleporter = false; // Resetting this variable will allow the player to throw out the teleporter again and re-use the ability

                        // See "ExplorerTeleporter.cs" for more
                    }
                }
                #endregion

                #region Spellblade
                if (hasSpellblade)
                {
                    player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown1>(), 60); // 1 second cooldown unaffected by CDR (abilityCooldown)

                    if (!spellbladeToggle)
                        spellbladeToggle = true;
                    else
                        spellbladeToggle = false;
                }
                #endregion

                #region Defender
                if (ExpSentriesMod != null)
                {
                    if (hasDefender)
                    {
                        int turretCount = 0; //Count how many sentries the player has active to use for later
                        for (int i = 0; i < 1000; i++)
                        {
                            if (Main.projectile[i].active && (ProjectileID.Sets.IsADD2Turret[Main.projectile[i].type] || Main.projectile[i].sentry)
                            && Main.projectile[i].owner == player.whoAmI)
                            {
                                Projectile.NewProjectile(Main.projectile[i].Center, Vector2.Zero, mod.ProjectileType("SentryDetonate"), (int)(defenderAbility1Damage * acmPlayer.abilityDamage), 8, player.whoAmI);
                                Main.projectile[i].Kill();
                                turretCount++;
                            }
                        }
                        if (turretCount > 0) //Make sure to find any active sentries before triggering cooldown
                        {
                            if (hasClassPath1)
                            {
                                player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown1>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * 10));
                                defenderPassiveBoost = turretCount * 3;
                            }
                            else
                            {
                                player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown1>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * 15));
                                defenderPassiveBoost = turretCount * 2;
                            }
                            defenderPassiveTimer = (int)(600 * acmPlayer.abilityDuration);
                            Main.PlaySound(SoundID.Mech);
                        }
                        else //Otherwise, the ability effectively fails to use and can intantly used again
                        {
                            Main.PlaySound(SoundID.MenuClose);
                        }
                    }
                }
                #endregion
            }

            // If the main mod's ability 2 cooldown debuff is NOT currently active, run all the code below this line
            if (ACM.ClassAbility2.JustPressed && player.FindBuffIndex(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown2>()) == -1 && Main.myPlayer == player.whoAmI)
            {
                #region Explorer
                if (hasExplorer) // If our currently equipped class is Explorer
                {
                    // If the player has the class' second path, apply a 16s cooldown, otherwise, apply a 18s cooldown.
                    // baseCooldown is 60 (1 second) by default, but can be changed through the main mod's config to decrease the overall cooldown abilities have
                    // cooldownReduction is how much % of, well, cooldown reduction the players has, reducing the ability's cooldown
                    if (hasClassPath2)
                        player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown2>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * 16)); // <-- 16 is the cooldown in seconds
                    else
                        player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown2>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * 18)); // <-- 18 is the cooldown in seconds

                    explorerDodgeTimer = (int)(20 * acmPlayer.abilityDuration); // Set the timer to 20 ticks, multiplied by abilityDuration so it scales with it.
                    explorerDodgeHeal = false; // Setting a variable to false will allow us to mamke the player to heal only once when hit by an NPC/Projectile

                    Main.PlaySound(SoundID.MenuClose); // Play a sound
                }
                #endregion
                
                #region Spellblade
                if (hasSpellblade)
                {
                    int bCooldown = 42;
                    int baseDamage = 35;
                    int baseDamageHardmode = 75;

                    if (hasClassPath1)
                        bCooldown = 36;

                    player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown2>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * bCooldown));

                    if(Main.hardMode)
                        Projectile.NewProjectile(Main.MouseWorld.X, Main.MouseWorld.Y, 0, 0, ModContent.ProjectileType<Projectiles.Spellblade.ShokkZone>(), (int)(baseDamageHardmode * acmPlayer.abilityDamage), 1, player.whoAmI);
                    else
                        Projectile.NewProjectile(Main.MouseWorld.X, Main.MouseWorld.Y, 0, 0, ModContent.ProjectileType<Projectiles.Spellblade.ShokkZone>(), (int)(baseDamage * acmPlayer.abilityDamage), 1, player.whoAmI);
                }
                #endregion

                #region Defender
                if (ExpSentriesMod != null)
                {
                    if (hasDefender)
                    {
                        bool hasSentry = false; //Like turretCount, but this time, check if the player has any active sentries, rather than get the exact amount
                        for (int i = 0; i < 1000; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].sentry
                            && Main.projectile[i].owner == player.whoAmI)
                            {
                                hasSentry = true;
                                break;
                            }
                        }
                        if (hasSentry) //Use the ability as intended
                        {
                            if (hasClassPath2)
                                player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown2>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * 50));
                            else
                                player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown2>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * 60));
                            if (hasClassPath2)
                                defenderPoweredTimer = (int)(900 * acmPlayer.abilityDuration);
                            else
                                defenderPoweredTimer = (int)(600 * acmPlayer.abilityDuration);
                            Main.PlaySound(SoundID.Item37);
                        }
                        else //Failsafe otherwise
                        {
                            Main.PlaySound(SoundID.MenuClose);
                        }
                    }
                }
                #endregion
            }
            base.ProcessTriggers(triggersSet);
        }
    }
}