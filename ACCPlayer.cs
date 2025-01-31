using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using ApacchiisClassesMod;
using Microsoft.Xna.Framework;
using System;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using Microsoft.Xna.Framework.Audio;
using Terraria.Audio;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ApacchiisCuratedClasses
{
	public class ACCPlayer : ModPlayer
	{
        //For weak referenced cross-mod content 
        Mod ExpSentriesMod = ModLoader.GetMod("ExpandedSentries");
        Mod EsperClassMod = ModLoader.GetMod("EsperClass");

        // Main mod Misc. Variables
        public bool hasEquipedClass;
        public bool hasClassPath1;
        public bool hasClassPath2;

        //Explorer
        public bool hasExplorer;
        bool explorerDodgeHeal = false;
        int explorerDodgeTimer = 0;
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

        //Esper
        public bool hasEsper;
        int esperRepulsionTimer = 0;
        bool isEsperHover;
        SoundEffectInstance esperHoverStartSound;
        SoundEffectInstance esperHoverLoopSound;

        public override void ResetEffects()
        {
            hasClassPath1 = false;
            hasClassPath2 = false;

            // Resetting class variables to their default so when the player unnequips the class the class' variables are reset
            hasExplorer = false;
            hasSpellblade = false;
            hasDefender = false;
            hasEsper = false;
            base.ResetEffects();
        }

        public override void PreUpdateBuffs()
        {
            #region Explorer Ability 1 Timers & Effects
            // Here we increase or decrease timers
            if (explorerDodgeTimer > 0) // So if the Dodge Timer is above 0
            {
                explorerDodgeTimer--; // Decrease it 1 by 1 each tick
                 
                Player.invis = true;
                Player.velocity = new Vector2(0, 0);
                Player.moveSpeed = 0f;
                Player.slowFall = true;
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

                    //Reflection for cross-mod compatability without hard references
                    ModPlayer esPlayer = Player.GetModPlayer<>(ExpSentriesMod);
                    Type esPlayerType = esPlayer.GetType();

                    // Sentry Range
                    FieldInfo sentryRange = esPlayerType.GetField("sentryRange", BindingFlags.Instance | BindingFlags.Public);
                    float oldSentryRange = (float)sentryRange.GetValue(esPlayer);
                    sentryRange.SetValue(esPlayer, oldSentryRange + 1f);

                    // Sentry Speed
                    FieldInfo sentrySpeed = esPlayerType.GetField("sentrySpeed", BindingFlags.Instance | BindingFlags.Public);
                    float oldSentrySpeed = (float)sentrySpeed.GetValue(esPlayer);
                    sentrySpeed.SetValue(esPlayer, oldSentrySpeed + .5f);
                }
            }
            #endregion

            #region Esper Ability Timers
            if (EsperClassMod != null)
            {
                if (esperRepulsionTimer > 0)
                {
                    esperRepulsionTimer--;
                    //if (!hasEsper)
                    //	esperRepulsionTimer = 0;
                    if (esperRepulsionTimer <= 0)
                    {
                        for (int i = 0; i < Main.projectile.Length; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].type == mod.ProjectileType("EsperRepel") && Main.projectile[i].owner == Player.whoAmI)
                            {
                                Main.projectile[i].Kill();
                                break;
                            }
                        }
                    }
                }
                if (isEsperHover)
                {
                    if ((esperHoverLoopSound == null || esperHoverLoopSound.State != SoundState.Playing)
                    && (esperHoverStartSound == null || esperHoverStartSound.State != SoundState.Playing))
                    {
                        if (hasClassPath2)
                            esperHoverLoopSound = Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/Esper/EsperHoverLoop2"));
                        else
                            esperHoverLoopSound = Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/Esper/EsperHoverLoop"));
                    }
                    if (Player.mount.Active || Player.pulley || Player.HasBuff(EsperClassMod.BuffType("PsychedOut"))
                    || Player.grappling[0] != -1 /*|| !hasEsper*/)
                    {
                        isEsperHover = false;
                        if (esperHoverLoopSound != null)
                            esperHoverLoopSound.Stop(true);
                        if (hasClassPath2)
                            Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/Esper/EsperHoverEnd2"));
                        else
                            Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/Esper/EsperHoverEnd"));
                    }
                    else
                    {
                        Player.gravity = 0;
                        Player.wingTime = 0;
                        Player.rocketTime = 0;
                        Player.canJumpAgain_Cloud = false;
                        Player.canJumpAgain_Sandstorm = false;
                        Player.canJumpAgain_Blizzard = false;
                        Player.canJumpAgain_Fart = false;
                        Player.canJumpAgain_Sail = false;
                        Player.canJumpAgain_Unicorn = false;
                        Player.canCarpet = false;
                        Player.carpetTime = 0;
                        Player.fallStart = (int)(Player.position.Y / 16f);
                        for (int i = -1; i < 2; i++)
                        {
                            if (i != 0)
                            {
                                int hoverDust = Dust.NewDust(Player.Center, 0, 0, 272, 0f, 0f, 100, default(Color), 0.5f);
                                Main.dust[hoverDust].noGravity = true;
                                Main.dust[hoverDust].velocity.X = (2 + Player.velocity.X) * i;
                                Main.dust[hoverDust].velocity.Y = Player.velocity.Y;
                                Main.dust[hoverDust].noLight = true;
                                Main.dust[hoverDust].position.X = Player.Center.X;
                                if (Player.gravDir == 1f)
                                    Main.dust[hoverDust].position.Y = Player.position.Y + 44;
                                else
                                    Main.dust[hoverDust].position.Y = Player.position.Y;
                            }
                        }
                    }
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
                Player.buffImmune[BuffID.Bleeding] = true;
                Player.buffImmune[BuffID.Poisoned] = true;
                Player.buffImmune[BuffID.OnFire] = true;
                Player.buffImmune[BuffID.Venom] = true;
                Player.buffImmune[BuffID.Darkness] = true;
                Player.buffImmune[BuffID.Blackout] = true;
                Player.buffImmune[BuffID.Silenced] = true;
                Player.buffImmune[BuffID.Cursed] = true;
                Player.buffImmune[BuffID.Confused] = true;
                Player.buffImmune[BuffID.Silenced] = true;
                Player.buffImmune[BuffID.Slow] = true;
                Player.buffImmune[BuffID.OgreSpit] = true;
                Player.buffImmune[BuffID.Weak] = true;
                Player.buffImmune[BuffID.BrokenArmor] = true;
                Player.buffImmune[BuffID.WitheredArmor] = true;
                Player.buffImmune[BuffID.WitheredWeapon] = true;
                Player.buffImmune[BuffID.CursedInferno] = true;
                Player.buffImmune[BuffID.Ichor] = true;
                Player.buffImmune[BuffID.Frostburn] = true;
                Player.buffImmune[BuffID.Chilled] = true;
                Player.buffImmune[BuffID.Frozen] = true;
                Player.buffImmune[BuffID.Webbed] = true;
                Player.buffImmune[BuffID.Stoned] = true;
                Player.buffImmune[BuffID.VortexDebuff] = true;
                Player.buffImmune[BuffID.Electrified] = true;
            }
            #endregion

            #region Esper Hover
            if (EsperClassMod != null)
            {
                if (isEsperHover)
                {
                    Player.buffImmune[BuffID.VortexDebuff] = true;
                }
            }
            #endregion
            base.PostUpdateBuffs();
        }

        public override void PostUpdateEquips()
        {
            #region Spellblade Ability 1
            if(hasSpellblade)
            {
                if (!Player.HeldItem.noMelee)
                    Player.statDefense += (int)(Player.GetCritChance(DamageClass.Magic) * .4f);
                
                // Player.meleeDamage += Player.magicDamage;
                // Add player magic damage to melee damage with Player.Get

                if(spellbladeToggle)
                {
                    Player.manaRegen = 0;
                    Player.manaRegenBonus = 0;
                    Player.manaRegenCount = 0;
                    Player.manaRegenDelayBonus = 0;

                    if (!Player.HeldItem.noMelee)
                        Player.HeldItem.mana = magicBladeBaseCost;
                }
                else
                {
                    if (!Player.HeldItem.noMelee)
                        Player.HeldItem.mana = 0;
                }

                
            }
            #endregion

            #region Esper Hover
            if (EsperClassMod != null)
            {
                if (isEsperHover)
                {
                    if (hasClassPath2)
                    {
                        Player.moveSpeed *= 1.5f;
                        Player.accRunSpeed *= 1.5f;
                        Player.maxRunSpeed *= 1.5f;
                    }
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
                        Player.statDefense += defenderPassiveBoost;
                        if (defenderPassiveTimer <= 0)
                            defenderPassiveBoost = 0;
                    }
                    else //Otherwise, scale with active sentries
                    {
                        int turretCount = 0;
                        for (int j = 0; j < 1000; j++)
                        {
                            if (Main.projectile[j].active && Main.projectile[j].owner == Player.whoAmI && Main.projectile[j].sentry)
                                turretCount++;
                        }

                        if (turretCount > 0)
                            Player.statDefense += turretCount;
                    }
                }
            }
            #endregion

            #region Esper Passive and Ability 2
            if (EsperClassMod != null)
            {
                if (hasEsper)
                {
                    float regenBoost = 5f;
                    if (!Player.HasBuff(EsperClassMod.BuffType("PsychedOut")))
                    {
                        ModPlayer ECPlayer = Player.GetModPlayer<>(EsperClassMod);
                        Type ECPlayerType = ECPlayer.GetType();
                        FieldInfo psychosis = ECPlayerType.GetField("psychosis", BindingFlags.Instance | BindingFlags.Public);
                        float oldpsychosis = (float)psychosis.GetValue(ECPlayer);

                        MethodInfo TotalPsychosis = ECPlayerType.GetMethod("TotalPsychosis", BindingFlags.Instance | BindingFlags.Public);
                        int totalAmount = (int)TotalPsychosis.Invoke(ECPlayer, new object[] { });
                        if (oldpsychosis > 0f)
                            regenBoost = (oldpsychosis / totalAmount) * 5f;
                        else
                            regenBoost = 0;
                    }
                    Player.lifeRegen += (int)regenBoost;
                }

                if (isEsperHover)
                {
                    // Psychosis Drain
                    int drainAmount;
                    if (hasClassPath2)
                    {
                        drainAmount = 1;
                        Player.armorEffectDrawOutlines = true;
                    }
                    else
                        drainAmount = 2;
                    ModPlayer ECPlayer = Player.GetModPlayer<>(EsperClassMod);
                    Type ECPlayerType = ECPlayer.GetType();
                    MethodInfo PsychosisDrain = ECPlayerType.GetMethod("PsychosisDrain", BindingFlags.Instance | BindingFlags.Public);
                    PsychosisDrain.Invoke(ECPlayer, new object[] { drainAmount, Missing.Value, Missing.Value });
                    float hoverY = 0;
                    if (Player.controlUp || Player.controlJump)
                    {
                        hoverY = -5f;
                    }
                    else if (Player.controlDown)
                    {
                        hoverY = 5f;
                        if (Player.gravDir == 1f) //Can't move through platforms without a method like this
                        {
                            int x1 = (int)(Player.position.X / 16f);
                            int x2 = (int)((Player.position.X + Player.width) / 16f);
                            int y1 = (int)((Player.position.Y + Player.height + 1) / 16f);
                            int y2 = (int)((Player.position.Y + Player.height + 17) / 16f);
                            if (x1 < 0)
                                x1 = 0;
                            if (x2 > Main.maxTilesX)
                                x2 = Main.maxTilesX;
                            if (y1 < 0)
                                y1 = 0;
                            if (y2 > Main.maxTilesY)
                                y2 = Main.maxTilesY;
                            for (int i = x1; i < x2; i++)
                            {
                                for (int j = y1; j < y2; j++)
                                {
                                    if (Main.tile[i, j].active && !Main.tile[i, j].inActive() && Main.tileSolidTop[(int)Main.tile[i, j].type])
                                    {
                                        Player.position.Y += 2;
                                    }
                                }
                            }
                        }
                    }
                    if (hasClassPath2)
                        hoverY *= 2f;
                    hoverY *= Player.gravDir;
                    Player.velocity.Y = hoverY;
                }
            }
            #endregion
            base.PostUpdate();
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            var acmPlayer = Player.GetModPlayer<ApacchiisClassesMod.MyPlayer>();

            #region Explorer Dodge Heal
            if (explorerDodgeTimer > 0 && !explorerDodgeHeal) // If the timer is above 0, and we have not yet healed from dodging
            {
                damage = 0; // Set the damage taken to 0 (this will actually be 1, since damage taken cannot be below 1)
                Player.statLife += (int)(Player.statLifeMax2 * .06f * acmPlayer.abilityDamage + 1 ); // Heal for 4% of max health * ability power, +1 for the damage we took when dodging
                Player.HealEffect((int)(Player.statLifeMax2 * .06f * acmPlayer.abilityDamage + 1)); // Display the Heal Effect for the same value (green numbers above the player's head when healed)
                explorerDodgeHeal = true; // We have now healed from dodging, so this can no longer happen until we re-use the ability
            }
            #endregion
            base.ModifyHitByNPC(npc, ref damage, ref crit);
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            var acmPlayer = Player.GetModPlayer<ApacchiisClassesMod.MyPlayer>();

            #region Explorer Dodge Heal
            if (explorerDodgeTimer > 0 && !explorerDodgeHeal) // If the timer is above 0, and we have not yet healed from dodging
            {
                damage = 0; // Set the damage taken to 0 (this will actually be 1, since damage taken cannot be below 1)
                Player.statLife += (int)(Player.statLifeMax2 * .04f * acmPlayer.abilityDamage + 1); // Heal for 4% of max health * ability power, +1 for the damage we took when dodging
                Player.HealEffect((int)(Player.statLifeMax2 * .04f * acmPlayer.abilityDamage + 1)); // Display the Heal Effect for the same value (green numbers above the player's head when healed)
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
                    CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y - 25, Player.width, Player.height), new Color(255, 255, 255), "!", true); // Small visual feedback that the passive was used
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
                    CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y - 25, Player.width, Player.height), new Color(255, 255, 255), "!", true); // Small visual feedback that the passive was used
                }                                                                                                                                                             // Displays a white "!" on top of the player
            }
            #endregion

            #region Esper Passive
            if (EsperClassMod != null)
            {
                if (hasEsper)
                {
                    Assembly esperClass = EsperClassMod.Code;
                    Type ECProjectileType = esperClass.GetType("EsperClass.ECProjectile");
                    if (crit && (proj.modProjectile?.GetType().IsSubclassOf(ECProjectileType) == true || proj.type == ProjectileID.FlyingKnife))
                    {
                        double damageBoost = 0.25f;
                        if (!Player.HasBuff(EsperClassMod.BuffType("PsychedOut")))
                        {
                            ModPlayer ECPlayer = Player.GetModPlayer<>(EsperClassMod);
                            Type ECPlayerType = ECPlayer.GetType();
                            FieldInfo psychosis = ECPlayerType.GetField("psychosis", BindingFlags.Instance | BindingFlags.Public);
                            float oldpsychosis = (float)psychosis.GetValue(ECPlayer);

                            MethodInfo TotalPsychosis = ECPlayerType.GetMethod("TotalPsychosis", BindingFlags.Instance | BindingFlags.Public);
                            int totalAmount = (int)TotalPsychosis.Invoke(ECPlayer, new object[] { });
                            if (oldpsychosis > 0f)
                                damageBoost = (0.25f / totalAmount) * (totalAmount - oldpsychosis);
                            else
                                damageBoost = 0.25f;
                        }
                        damage = (int)(damage * (damageBoost + 1));
                    }
                }
            }
            #endregion
            base.ModifyHitNPCWithProj(proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
        }

        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            #region Spellblade
            if (hasSpellblade && spellbladeToggle && !item.noMelee)
                item.mana = magicBladeBaseCost;
            #endregion
            base.ModifyManaCost(item, ref reduce, ref mult);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            var acmPlayer = Player.GetModPlayer<ApacchiisClassesMod.MyPlayer>();

            /* <NOTES>
             * - acmPlayer.abilityDamage is how Ability Power is named internally, its not just for damage.
             */

            // If the main mod's ability 1 cooldown debuff is NOT currently active, run all the code below this line
            if (ACM.ClassAbility1.JustPressed && Player.FindBuffIndex(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown1>()) == -1 && Main.myPlayer == Player.whoAmI)
            {
                #region Explorer
                if (hasExplorer) // If our currently equipped class is Explorer
                {
                    if (!explorerThrownTeleporter) // And we haven't thrown out our teleporter
                    {
                        float APValue = .4f; // Percentage of Ability Power scaling, 0.4f = 40%, meaning the ability will only scale with 40% of the player's Ability Power
                        float velMultiplier = (acmPlayer.abilityDamage - 1f) * APValue + 1f; // You can just copy/paste these 2 lines and change APValue to whatever you want for your class

                        // Grab mouse position compared to the player's position and normalize velocity
                        Vector2 vel = Main.MouseWorld - Player.position;
                        vel.Normalize();
                        vel *= 2 * velMultiplier; // The lower this value, the lower the speed at which the projectile moves

                        // Spawn the teleporter projectile towards mouse position                   V      V                                                            V
                        var tp = Projectile.NewProjectile(Player.Center.X, Player.position.Y - 10, vel.X, vel.Y, ModContent.ProjectileType<Projectiles.Explorer.ExplorerTeleporter>(), 0, 0, Player.whoAmI);
                        explorerThrownTeleporter = true; // We have now thrown our teleporter
                    }
                    else // If we have already thrown out our teleporter
                    {
                        if (hasClassPath1) // And we have the class' Path 1
                            AddAbilityCooldown(1, 35); // 1 is the ability number, 35 is the cooldown value in seconds
                        else
                            AddAbilityCooldown(1, 42); // 1 is the ability number, 42 is the cooldown value in seconds

                        Player.Teleport(explorerTeleporterPos); // Set the player's position to a Vector2 variable thats updated every tick by the "ExplorerTeleporter" projectile.
                        explorerThrownTeleporter = false; // Resetting this variable will allow the player to throw out the teleporter again and re-use the ability

                        // See "ExplorerTeleporter.cs" for more
                    }
                }
                #endregion

                #region Spellblade
                if (hasSpellblade)
                {
                    Player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown1>(), 60); // 1 second cooldown unaffected by CDR (abilityCooldown)

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
                            && Main.projectile[i].owner == Player.whoAmI)
                            {
                                Projectile.NewProjectile(Main.projectile[i].Center, Vector2.Zero, mod.ProjectileType("SentryDetonate"), (int)(defenderAbility1Damage * acmPlayer.abilityDamage), 8, Player.whoAmI);
                                Main.projectile[i].Kill();
                                turretCount++;
                            }
                        }
                        if (turretCount > 0) //Make sure to find any active sentries before triggering cooldown
                        {
                            if (hasClassPath1)
                            {
                                Player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown1>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * 10));
                                defenderPassiveBoost = turretCount * 3;
                            }
                            else
                            {
                                Player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown1>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * 15));
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

                #region Esper
                if (EsperClassMod != null)
                {
                    if (hasEsper)
                    {
                        Player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown1>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * 60));
                        esperRepulsionTimer = (int)(900 * acmPlayer.abilityDuration);
                        Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/Esper/EsperRepelStart"));
                        for (int i = 0; i < Main.projectile.Length; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].type == mod.ProjectileType("EsperRepel") && Main.projectile[i].owner == Player.whoAmI)
                            {
                                Main.projectile[i].Kill();
                                break;
                            }
                        }
                        Projectile.NewProjectile(Player.Center, Vector2.Zero, mod.ProjectileType("EsperRepel"), 0, 16 * acmPlayer.abilityDamage, Player.whoAmI);
                    }
                }
                #endregion
            }

            // If the main mod's ability 2 cooldown debuff is NOT currently active, run all the code below this line
            if (ACM.ClassAbility2.JustPressed && Player.FindBuffIndex(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown2>()) == -1 && Main.myPlayer == Player.whoAmI)
            {
                #region Explorer
                if (hasExplorer) // If our currently equipped class is Explorer
                {
                    if (hasClassPath2)
                        AddAbilityCooldown(2, 16); // 2 is the ability number, 16 is the cooldown value in seconds
                    else
                        AddAbilityCooldown(2, 18); // 2 is the ability number, 18 is the cooldown value in seconds

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

                    Player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown2>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * bCooldown));

                    if(Main.hardMode)
                        Projectile.NewProjectile(Main.MouseWorld.X, Main.MouseWorld.Y, 0, 0, ModContent.ProjectileType<Projectiles.Spellblade.ShokkZone>(), (int)(baseDamageHardmode * acmPlayer.abilityDamage), 1, Player.whoAmI);
                    else
                        Projectile.NewProjectile(Main.MouseWorld.X, Main.MouseWorld.Y, 0, 0, ModContent.ProjectileType<Projectiles.Spellblade.ShokkZone>(), (int)(baseDamage * acmPlayer.abilityDamage), 1, Player.whoAmI);
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
                            && Main.projectile[i].owner == Player.whoAmI)
                            {
                                hasSentry = true;
                                break;
                            }
                        }
                        if (hasSentry) //Use the ability as intended
                        {
                            if (hasClassPath2)
                                Player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown2>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * 50));
                            else
                                Player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown2>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * 60));
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

                #region Esper
                if (EsperClassMod != null)
                {
                    if (hasEsper)
                    {
                        if (!isEsperHover)
                        {
                            if (!Player.mount.Active && !Player.pulley && !Player.HasBuff(EsperClassMod.BuffType("PsychedOut"))
                            && Player.grappling[0] == -1)
                            {
                                ModPlayer ECPlayer = Player.GetModPlayer(EsperClassMod, "ECPlayer");
                                Type ECPlayerType = ECPlayer.GetType();

                                FieldInfo psychosis = ECPlayerType.GetField("psychosis", BindingFlags.Instance | BindingFlags.Public);
                                float oldpsychosis = (float)psychosis.GetValue(ECPlayer);
                                psychosis.SetValue(ECPlayer, oldpsychosis - 3f);
                                isEsperHover = true;
                                if (esperHoverStartSound != null)
                                    esperHoverStartSound.Stop(true);
                                esperHoverStartSound = Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/Esper/EsperHoverStart"));
                                Player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown2>(), 60);
                            }
                            else
                                Main.PlaySound(SoundID.MenuClose);
                        }
                        else
                        {
                            isEsperHover = false;
                            if (esperHoverLoopSound != null)
                                esperHoverLoopSound.Stop(true);
                            if (hasClassPath2)
                                Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/Esper/EsperHoverEnd2"));
                            else
                                Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/Esper/EsperHoverEnd"));
                        }
                    }
                }
                #endregion
            }
            base.ProcessTriggers(triggersSet);
        }

        // To easily apply ability cooldowns just call this (Examples in the Explorer class' abilities)
        /// <summary>
        /// Applies class ability cooldowns.
        /// Ability number is if the ability is either Ability 1 or Ability 2.
        /// </summary>
        public void AddAbilityCooldown(int abilityNumber, int cooldownInSeconds)
        {
            

            // If the player has the class' second path, apply a 16s cooldown, otherwise, apply a 18s cooldown.
            // baseCooldown is 60 (1 second) by default, but can be changed through the main mod's config to decrease the overall cooldown abilities have
            // cooldownReduction is how much % of, well, cooldown reduction the players has, reducing the ability's cooldown

            var acmPlayer = Player.GetModPlayer<ApacchiisClassesMod.MyPlayer>();

            if (abilityNumber == 1)
                Player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown1>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * cooldownInSeconds));
            if(abilityNumber == 2)
                Player.AddBuff(ModContent.BuffType<ApacchiisClassesMod.Buffs.ActiveCooldown2>(), (int)(acmPlayer.baseCooldown * acmPlayer.cooldownReduction * cooldownInSeconds));
            if (abilityNumber > 2 || abilityNumber <= 0)
                Main.NewText("ERROR: 'AddAbilityCooldown' abilityNumber parameter isn't either 1 nor 2, make it either 1 for Ability 1 or 2 for Ability 2");
        }
    }
}