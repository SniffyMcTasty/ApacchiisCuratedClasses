using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Classes.Spellblade
{
	public class SpellbladeLv7 : BaseClass
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Class: Spellblade Lv.7");
            Tooltip.SetDefault("[c/3485c5:[Holding a magic weaponc/3485c5:]]\n" +
                               "[c/3485c5:+9.8% Magic Damage]\n" +
                               "[c/3485c5:-8.6% Mana Costs]\n" +
                               "[c/3485c5:+28 Max Mana]\n" +
                               "[c/d33838:[Holding a melee weapon[c/d33838:]]\n" +
                               "[c/d33838:+12.6% Magic Damage]\n" +
                               "[c/d33838:+7% Melee Crit Chance]\n" +
                               "[c/d33838:Melee Damage is also increased by Magic Damage]\n" +
                               "[c/d33838:Defense is increased by 40% of magic crit]\n" +
                               "-----------\n" +
                               "[Magic Blade] Base Damage: 35\n" +
                               "[Magic Blade] Base Mana Cost: 22\n" +
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
			Item.rare = 7;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.GetInstance<Classes.Spellblade.SpellbladeLv6>(), 1);
            recipe.AddIngredient(ModContent.GetInstance<ApacchiisClassesMod.Items.Tokens.ClassTokenLv7>(), 1);
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
                player.magicDamage += .098f;
                player.manaCost -= .086f;
                player.statManaMax2 += 28;
            }

            if (player.HeldItem.melee)
            {
                player.magicDamage += .126f;
                player.meleeCrit += 7;
            }

            accPlayer.shokkZoneTimerBase = 35;
            accPlayer.magicBladeBaseDamage = 35;
            accPlayer.magicBladeBaseCost = 22;
        }
    }
}

