using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Classes.Defender
{
	public class DefenderLv1 : BaseClass
	{
        Player player = Main.player[Main.myPlayer];
        Mod ExpSentriesMod = ModLoader.GetMod("ExpandedSentries");

        public override void SetStaticDefaults()
        {
            if(ExpSentriesMod != null)
            {
                DisplayName.SetDefault("Class: Defender Lv.1");
                Tooltip.SetDefault("+3% Sentry Damage\n" +
                                   "+1 Max Sentries\n" +
                                   "+5% Sentry Range\n" +
                                   "+2.5% Sentry Speed\n" +
                                   "[Detonate Sentries] Base Damage: 100\n" +
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
            Item.rare = 1;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.GetInstance<ApacchiisClassesMod.Items.ClassPicker>(), 1);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

        public override void OnCraft(Recipe recipe)
        {
            if (ExpSentriesMod == null)
            {
                player.QuickSpawnItem(ModContent.ItemType<ApacchiisClassesMod.Items.ClassPicker>(), 1);
                Main.NewText("This class required the mod 'Expanded Sentries' to work, you can download it through the Mod Browser");
                Main.NewText("[Class Picker returned]");
            }
            
            base.OnCraft(recipe);
        }

        public override void UpdateAccessory (Player player, bool hideVisual)
		{
            ACCPlayer accPlayer = Main.player[player.whoAmI].GetModPlayer<ACCPlayer>();
            if(ExpSentriesMod != null)
            {
                player.GetModPlayer<ApacchiisClassesMod.MyPlayer>().hasEquippedClass = true;
                accPlayer.hasDefender = true;

                //Reflection for cross-mod compatability without hard references
                ModPlayer esPlayer = player.GetModPlayer<>(ExpSentriesMod);
                Type esPlayerType = esPlayer.GetType();

                // Sentry Damage
                FieldInfo sentryDamage = esPlayerType.GetField("sentryDamage", BindingFlags.Instance | BindingFlags.Public);
                float oldSentryDamage = (float)sentryDamage.GetValue(esPlayer);
                sentryDamage.SetValue(esPlayer, oldSentryDamage + .03f);

                // Sentry Range
                FieldInfo sentryRange = esPlayerType.GetField("sentryRange", BindingFlags.Instance | BindingFlags.Public);
                float oldSentryRange = (float)sentryRange.GetValue(esPlayer);
                sentryRange.SetValue(esPlayer, oldSentryRange + .05f);

                // Sentry Speed
                FieldInfo sentrySpeed = esPlayerType.GetField("sentrySpeed", BindingFlags.Instance | BindingFlags.Public);
                float oldSentrySpeed = (float)sentrySpeed.GetValue(esPlayer);
                sentrySpeed.SetValue(esPlayer, oldSentrySpeed + .025f);

                player.maxTurrets++;
                accPlayer.defenderAbility1Damage = 100;
            }
        }
    }
}
