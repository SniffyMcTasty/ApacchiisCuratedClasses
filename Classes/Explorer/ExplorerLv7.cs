using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Classes.Explorer
{
	public class ExplorerLv7 : BaseClass
	{
        Player player = Main.player[Main.myPlayer];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Class: Explorer Lv.7");
            Tooltip.SetDefault("+8.4% All Damage\n" +
                               "+7% Movement Speed\n" +
                               "+10.5% Mining Speed\n" +
                               "+7 Defense\n" +
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
			Item.rare = 7;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.GetInstance<ApacchiisClassesMod.Items.Tokens.ClassTokenLv7>(), 1);
            recipe.AddIngredient(ModContent.GetInstance<ExplorerLv6>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
		
		public override void UpdateAccessory (Player player, bool hideVisual)
		{
            ACCPlayer accPlayer = Main.player[player.whoAmI].GetModPlayer<ACCPlayer>();
            player.GetModPlayer<ApacchiisClassesMod.MyPlayer>().hasEquippedClass = true;
            accPlayer.hasExplorer = true;
            accPlayer.hasClassPath1 = true;

            player.allDamage += .084f;
            player.moveSpeed += .07f;
            player.pickSpeed += .155f;
            player.statDefense += 7;
            player.nightVision = true;
        }
    }
}

