using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Classes.Spellblade
{
	public class SpellbladeLv2 : BaseClass
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Class: Spellblade Lv.2");
            Tooltip.SetDefault("[c/3485c5:[Holding a magic weapon[c/3485c5:]]\n" +
                               "[c/3485c5:+2.8% Magic Damage]\n" +
                               "[c/3485c5:-2.4% Mana Costs]\n" +
                               "[c/3485c5:+8 Max Mana]\n" +
                               "[c/d33838:[Holding a melee weapon[c/d33838:]]\n" +
                               "[c/d33838:+3.6% Magic Damage]\n" +
                               "[c/d33838:+2% Melee Crit Chance]\n" +
                               "[c/d33838:Melee Damage is also increased by Magic Damage]\n" +
                               "[c/d33838:Defense is increased by 40% of magic crit]\n" +
                               "-----------\n" +
                               "[Magic Blade] Base Damage: 10\n" +
                               "[Magic Blade] Base Mana Cost: 7\n" +
                               "(By: Apacchii)");
        }

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.accessory = true;
			Item.value = 0;
			Item.rare = 2;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.GetInstance<Classes.Spellblade.SpellbladeLv1>(), 1);
            recipe.AddIngredient(ModContent.GetInstance<ApacchiisClassesMod.Items.Tokens.ClassTokenLv2>(), 1);
            recipe.SetResult(this);
			recipe.AddRecipe();
		}
		
		public override void UpdateAccessory (Player player, bool hideVisual)
		{
            ACCPlayer accPlayer = Main.player[player.whoAmI].GetModPlayer<ACCPlayer>();
            var acmPlayer = player.GetModPlayer<ApacchiisClassesMod.MyPlayer>();
            acmPlayer.hasEquippedClass = true;
            accPlayer.hasSpellblade = true;

            if (player.HeldItem.magic)
            {
                player.magicDamage += .028f;
                player.manaCost -= .024f;
                player.statManaMax2 += 8;
            }

            if (player.HeldItem.melee)
            {
                player.magicDamage += .036f;
                player.meleeCrit += 2;
            }

            accPlayer.shokkZoneTimerBase = 50;
            accPlayer.magicBladeBaseDamage = 10;
            accPlayer.magicBladeBaseCost = 7;
        }
    }
}

