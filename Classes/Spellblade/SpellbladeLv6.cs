using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Classes.Spellblade
{
	public class SpellbladeLv6 : BaseClass
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Class: Spellblade Lv.6");
            Tooltip.SetDefault("[c/3485c5:[Holding a magic weaponc/3485c5:]]\n" +
                               "[c/3485c5:+8.4% Magic Damage]\n" +
                               "[c/3485c5:-7.2% Mana Costs]\n" +
                               "[c/3485c5:+24 Max Mana]\n" +
                               "[c/d33838:[Holding a melee weapon[c/d33838:]]\n" +
                               "[c/d33838:+10.8% Magic Damage]\n" +
                               "[c/d33838:+6% Melee Crit Chance]\n" +
                               "[c/d33838:Melee Damage is also increased by Magic Damage]\n" +
                               "[c/d33838:Defense is increased by 40% of magic crit]\n" +
                               "-----------\n" +
                               "[Magic Blade] Base Damage: 30\n" +
                               "[Magic Blade] Base Mana Cost: 19\n" +
                               "[c/af7A6:[Path 1][c/af7A6:]]\n" +
                               "[Shokk Zone] Now attacks faster\n" +
                               "[Shokk Zone] Base cooldown reduced by 8 seconds\n" +
                               "[Shokk Zone] Base duration increased by 2 seconds\n" +
                               "(By: Apacchii)");
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
            recipe.AddIngredient(ModContent.GetInstance<Classes.Spellblade.SpellbladeLv5>(), 1);
            recipe.AddIngredient(ModContent.GetInstance<ApacchiisClassesMod.Items.Tokens.ClassTokenLv6>(), 1);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
		
		public override void UpdateAccessory (Player player, bool hideVisual)
		{
            ACCPlayer accPlayer = Main.player[player.whoAmI].GetModPlayer<ACCPlayer>();
            var acmPlayer = player.GetModPlayer<ApacchiisClassesMod.MyPlayer>();
            acmPlayer.hasEquippedClass = true;
            accPlayer.hasSpellblade = true;
            accPlayer.hasClassPath1 = true;

            if (player.HeldItem.magic)
            {
                player.magicDamage += .084f;
                player.manaCost -= .072f;
                player.statManaMax2 += 24;
            }

            if (player.HeldItem.melee)
            {
                player.magicDamage += .108f;
                player.meleeCrit += 6;
            }

            accPlayer.shokkZoneTimerBase = 35;
            accPlayer.magicBladeBaseDamage = 30;
            accPlayer.magicBladeBaseCost = 19;
        }
    }
}

