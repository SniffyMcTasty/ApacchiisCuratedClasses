using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Projectiles.Esper
{
	public class EsperRepel : ModProjectile
	{
		SoundEffectInstance loopSound;
		SoundEffectInstance repelSound;
		public override void SetDefaults()
		{
			Projectile.width = 256;
			Projectile.height = 256;
            Projectile.alpha = 255;
			Projectile.friendly = true;
			Projectile.hide = true;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
		}

		public override bool? CanCutTiles()
		{
			return false;
		}

		public override bool? CanHitNPC(NPC npc)
		{
			return false;
		}

		public override void Kill(int timeLeft)
		{
			if (loopSound != null)
			{
				loopSound.Stop(true);
			}
			Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/Esper/EsperRepelEnd"));
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
            var acmPlayer = player.GetModPlayer<ApacchiisClassesMod.MyPlayer>();
            var accPlayer = player.GetModPlayer<ACCPlayer>();
			if (!player.active || player.dead || Projectile.hostile)
			{
				Projectile.Kill();
				return;
			}
			if ((loopSound == null || loopSound.State != SoundState.Playing) && Projectile.ai[0] >= 15)
			{
				loopSound = Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/Esper/EsperRepelLoop"));
			}
			if (Projectile.ai[0] <= 0)
				Projectile.ai[0] = 1;
			Projectile.velocity = Vector2.Zero;
			float fieldSize = (Projectile.ai[0] / 15) * 256 * acmPlayer.abilityDamage;
			Projectile.width = (int)fieldSize;
			Projectile.height = (int)fieldSize;
			Projectile.Center = player.Center;
			for (int i = 0; i < 60; i++)
			{
				Vector2 dustPos = Projectile.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / 60 * i)) * ((Projectile.width + Projectile.height) / 4);
				int dustIndex = Dust.NewDust(dustPos, 1, 1, 86, 0, 0, 150, default(Color), 0.7f);
				Main.dust[dustIndex].noGravity = true;
				Main.dust[dustIndex].velocity = Vector2.Zero;
			}
			for (int l = 0; l < Main.npc.Length; l++)
			{
				NPC target = Main.npc[l];
				float distanceCheck = (fieldSize / 2);
				if (Vector2.Distance(target.Center, Projectile.Center) <= distanceCheck)
				{
					if (repelSound == null || repelSound.State != SoundState.Playing)
						repelSound = Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/Esper/EsperRepelHit"));
					if (target.lifeMax == 1)
					{
						target.life = 0;
					}
					if (target.knockBackResist > 0f && !target.townNPC)
					{
						if (target.Center.X < Projectile.Center.X && target.velocity.X > -Projectile.knockBack)
						{
							target.velocity.X = -Projectile.knockBack * target.knockBackResist;
						}
						else if (target.velocity.X < Projectile.knockBack)
						{
							target.velocity.X = Projectile.knockBack * target.knockBackResist;
						}
						if (target.Center.Y < Projectile.Center.Y && target.velocity.Y > -Projectile.knockBack)
						{
							target.velocity.Y = -Projectile.knockBack * target.knockBackResist;
						}
						else if (target.velocity.Y < Projectile.knockBack)
						{
							target.velocity.Y = Projectile.knockBack * target.knockBackResist;
						}
					}
				}
			}
			if (accPlayer.hasClassPath1)
			{
				int damageBlock;
				if (!Main.expertMode)
					damageBlock = 30 + (int)(player.statDefense * 0.5f);
				else
					damageBlock = 60 + (int)(player.statDefense * 0.75f);
				for (int j = 0; j < Main.Projectile.Length; j++)
				{
					Projectile proj = Main.Projectile[j];
					float distanceCheck2 = (fieldSize / 2);
					if (Vector2.Distance(proj.Center, Projectile.Center) <= distanceCheck2)
					{
						if (proj.hostile && !proj.friendly && proj.damage > 0 && proj.damage <= damageBlock)
						{
							if (repelSound == null || repelSound.State != SoundState.Playing)
								repelSound = Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/Esper/EsperRepelHit"));
							proj.Kill();
						}
					}
				}
			}
			if (Projectile.ai[0] < 15)
			{
				Projectile.ai[0]++;
				if (Projectile.ai[0] == 15)
					Projectile.knockBack *= 0.25f;
			}
		}
	}
}
