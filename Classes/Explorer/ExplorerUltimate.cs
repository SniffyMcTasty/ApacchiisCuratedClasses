using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Classes.Explorer
{
	public class ExplorerUltimate : BaseClass
	{
        Player player = Main.player[Main.myPlayer];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Class: Explorer Ultimate [Path 1]");  // <-- Level 5 and Ultimate tokens say which path the token is, other levels do not
            Tooltip.SetDefault("+12% All Damage\n" +
                               "+10% Movement Speed\n" +
                               "+15% Mining Speed\n" +
                               "+10 Defense\n" +
                               "[c/af7A6:[Path 1][c/af7A6:]]\n" +
                               "Mining speed increased by an additional 5%\n" +
                               "Permanent night vision buff\n" +
                               "Ability 1 Base cooldown decreased from 42 to 35 seconds\n" +
                               "(By: Apacchii)");
        }

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.accessory = true;
			Item.value = 0;
			Item.rare = 9;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.GetInstance<ApacchiisClassesMod.Items.Tokens.ClassTokenUltimate>(), 1);
            recipe.AddIngredient(ModContent.GetInstance<ExplorerLv8>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
		
		public override void UpdateAccessory (Player player, bool hideVisual)
		{
            ACCPlayer accPlayer = Main.player[player.whoAmI].GetModPlayer<ACCPlayer>();
            player.GetModPlayer<ApacchiisClassesMod.MyPlayer>().hasEquippedClass = true;
            accPlayer.hasExplorer = true;
            accPlayer.hasClassPath1 = true;

            player.allDamage += .12f;
            player.moveSpeed += .1f;
            player.pickSpeed += .2f;
            player.statDefense += 10;
            player.nightVision = true;
        }
    }
}

