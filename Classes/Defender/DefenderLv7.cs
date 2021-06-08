using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Classes.Defender
{
	public class DefenderLv7 : ModItem
	{
        Player player = Main.player[Main.myPlayer];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Class: Defender Lv.7");
            Tooltip.SetDefault("+21% Sentry Damage\n" +
                               "+3 Max Sentries\n" +
                               "+35% Sentry Range\n" +
                               "+17.5% Sentry Speed\n" +
                               "[Detonate Sentries] Base Damage: 3000\n" +
                               "[c/af7A6:[Path 1][c/af7A6:]]\n" +
                               "+1 Max Sentries\n" +
                               "[Detonate Sentries] Deals 20% more damage\n" +
                               "[Detonate Sentries] Base cooldown reduced from 15 to 10 seconds\n" +
                               "[Sentry Defense] Extra passive defense after using [Detonate Sentries] increased from 2 to 3\n" +
                               "(By: TheLoneGamer)");
        }

		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 30;
			item.accessory = true;
			item.value = 0;
			item.rare = 7;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.GetInstance<ApacchiisClassesMod.Items.Tokens.ClassTokenLv7>(), 1);
            recipe.AddIngredient(ModContent.GetInstance<DefenderLv6>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
		
		public override void UpdateAccessory (Player player, bool hideVisual)
		{
            ACCPlayer accPlayer = Main.player[player.whoAmI].GetModPlayer<ACCPlayer>();
            player.GetModPlayer<ApacchiisClassesMod.MyPlayer>().hasEquippedClass = true;
            accPlayer.hasDefender = true;
            accPlayer.hasClassPath1 = true;

            player.GetModPlayer<ExpandedSentries.ESPlayer>().sentryDamage += 0.21f;
            player.GetModPlayer<ExpandedSentries.ESPlayer>().sentryRange += 0.35f;
            player.GetModPlayer<ExpandedSentries.ESPlayer>().sentrySpeed += 0.175f;
            player.maxTurrets += 4;
            accPlayer.defenderAbility1Damage = 3000;
			player.GetModPlayer<ApacchiisClassesMod.MyPlayer>().abilityDamage += 0.2f;
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            if (player.GetModPlayer<ApacchiisClassesMod.MyPlayer>().hasEquippedClass == true)
                return false;

            return base.CanEquipAccessory(player, slot);
        }

        public override bool ReforgePrice(ref int reforgePrice, ref bool canApplyDiscount)
        {
            reforgePrice = 150000;
            return base.ReforgePrice(ref reforgePrice, ref canApplyDiscount);
        }
    }
}
