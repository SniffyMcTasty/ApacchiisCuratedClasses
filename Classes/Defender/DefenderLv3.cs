using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Classes.Defender
{
	public class DefenderLv3 : BaseClass
	{
        Player player = Main.player[Main.myPlayer];
        Mod ExpSentriesMod = ModLoader.GetMod("ExpandedSentries");

        public override void SetStaticDefaults()
        {
            if (ExpSentriesMod != null)
            {
                DisplayName.SetDefault("Class: Defender Lv.3");
                Tooltip.SetDefault("+9% Sentry Damage\n" +
                                   "+2 Max Sentries\n" +
                                   "+15% Sentry Range\n" +
                                   "+7.5% Sentry Speed\n" +
                                   "[Detonate Sentries] Base Damage: 250\n" +
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
			Item.rare = 3;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.GetInstance<ApacchiisClassesMod.Items.Tokens.ClassTokenLv3>(), 1);
            recipe.AddIngredient(ModContent.GetInstance<DefenderLv2>());
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

                //Reflection for cross-mod compatability without hard references
                ModPlayer esPlayer = player.GetModPlayer<>(ExpSentriesMod);
                Type esPlayerType = esPlayer.GetType();

                // Sentry Damage
                FieldInfo sentryDamage = esPlayerType.GetField("sentryDamage", BindingFlags.Instance | BindingFlags.Public);
                float oldSentryDamage = (float)sentryDamage.GetValue(esPlayer);
                sentryDamage.SetValue(esPlayer, oldSentryDamage + .09f);

                // Sentry Range
                FieldInfo sentryRange = esPlayerType.GetField("sentryRange", BindingFlags.Instance | BindingFlags.Public);
                float oldSentryRange = (float)sentryRange.GetValue(esPlayer);
                sentryRange.SetValue(esPlayer, oldSentryRange + .15f);

                // Sentry Speed
                FieldInfo sentrySpeed = esPlayerType.GetField("sentrySpeed", BindingFlags.Instance | BindingFlags.Public);
                float oldSentrySpeed = (float)sentrySpeed.GetValue(esPlayer);
                sentrySpeed.SetValue(esPlayer, oldSentrySpeed + .075f);

                player.maxTurrets += 2;
                accPlayer.defenderAbility1Damage = 250;
            }
        }
    }
}

