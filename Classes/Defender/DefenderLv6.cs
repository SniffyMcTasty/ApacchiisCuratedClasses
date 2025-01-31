using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Classes.Defender
{
	public class DefenderLv6 : BaseClass
	{
        Player player = Main.player[Main.myPlayer];
        Mod ExpSentriesMod = ModLoader.GetMod("ExpandedSentries");

        public override void SetStaticDefaults()
        {
            if (ExpSentriesMod != null)
            {
                DisplayName.SetDefault("Class: Defender Lv.6");
                Tooltip.SetDefault("+18% Sentry Damage\n" +
                                   "+3 Max Sentries\n" +
                                   "+30% Sentry Range\n" +
                                   "+15% Sentry Speed\n" +
                                   "[Detonate Sentries] Base Damage: 1500\n" +
                                   "[c/af7A6:[Path 1][c/af7A6:]]\n" +
                                   "+1 Max Sentries\n" +
                                   "[Detonate Sentries] Deals 20% more damage\n" +
                                   "[Detonate Sentries] Base cooldown reduced from 15 to 10 seconds\n" +
                                   "[Sentry Defense] Extra passive defense after using [Detonate Sentries] increased from 2 to 3\n" +
                                   "(By: TheLoneGamer)");
            }
            else
            {
                DisplayName.SetDefault("Class: Defender Lv.1");
                Tooltip.SetDefault("[c/d33838:This class requires the mod 'Expanded Sentries' to work]\n" +
                                   "[c/d33838:You can download it through the Mod Browser]");
            }
        }

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.accessory = true;
			Item.value = 0;
			Item.rare = 6;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.GetInstance<ApacchiisClassesMod.Items.Tokens.ClassTokenLv6>(), 1);
            recipe.AddIngredient(ModContent.GetInstance<DefenderLv5>());
            recipe.SetResult(this);
            recipe.AddRecipe();
		}
		
		public override void UpdateAccessory (Player player, bool hideVisual)
		{
            ACCPlayer accPlayer = Main.player[player.whoAmI].GetModPlayer<ACCPlayer>();
            if (ExpSentriesMod != null)
            {
                player.GetModPlayer<ApacchiisClassesMod.MyPlayer>().hasEquippedClass = true;
                accPlayer.hasDefender = true;
                accPlayer.hasClassPath1 = true;

                //Reflection for cross-mod compatability without hard references
                ModPlayer esPlayer = player.GetModPlayer<>(ExpSentriesMod);
                Type esPlayerType = esPlayer.GetType();

                // Sentry Damage
                FieldInfo sentryDamage = esPlayerType.GetField("sentryDamage", BindingFlags.Instance | BindingFlags.Public);
                float oldSentryDamage = (float)sentryDamage.GetValue(esPlayer);
                sentryDamage.SetValue(esPlayer, oldSentryDamage + .18f);

                // Sentry Range
                FieldInfo sentryRange = esPlayerType.GetField("sentryRange", BindingFlags.Instance | BindingFlags.Public);
                float oldSentryRange = (float)sentryRange.GetValue(esPlayer);
                sentryRange.SetValue(esPlayer, oldSentryRange + .3f);

                // Sentry Speed
                FieldInfo sentrySpeed = esPlayerType.GetField("sentrySpeed", BindingFlags.Instance | BindingFlags.Public);
                float oldSentrySpeed = (float)sentrySpeed.GetValue(esPlayer);
                sentrySpeed.SetValue(esPlayer, oldSentrySpeed + .15f);

                player.maxTurrets += 4;
                accPlayer.defenderAbility1Damage = 1500;
                player.GetModPlayer<ApacchiisClassesMod.MyPlayer>().abilityDamage += 0.2f;
            }
        }
    }
}

