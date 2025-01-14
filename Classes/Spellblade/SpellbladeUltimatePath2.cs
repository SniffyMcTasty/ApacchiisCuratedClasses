using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Classes.Spellblade
{
	public class SpellbladeUltimatePath2 : BaseClass
	{
        public override string Texture => "ApacchiisCuratedClasses/Classes/Explorer/ExplorerUltimate";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Class: Spellblade Ultimate [Path 2]");
            Tooltip.SetDefault("[c/3485c5:[Holding a magic weaponc/3485c5:]]\n" +
                               "[c/3485c5:+14% Magic Damage]\n" +
                               "[c/3485c5:-12% Mana Costs]\n" +
                               "[c/3485c5:+40 Max Mana]\n" +
                               "[c/d33838:[Holding a melee weapon[c/d33838:]]\n" +
                               "[c/d33838:+18% Magic Damage]\n" +
                               "[c/d33838:+10% Melee Crit Chance]\n" +
                               "[c/d33838:Melee Damage is also increased by Magic Damage]\n" +
                               "[c/d33838:Defense is increased by 40% of magic crit]\n" +
                               "-----------\n" +
                               "[Magic Blade] Base Damage: 50\n" +
                               "[Magic Blade] Base Mana Cost: 28\n" +
                               "[c/af7A6:[Path 2][c/af7A6:]]\n" +
                               "[Magic Blade] Projectile now pierces 2 more enemies\n" +
                               "[Magic Blade] Base damage increased by 10\n" +
                               "-8% Mana costs\n" +
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
            recipe.AddIngredient(ModContent.GetInstance<Classes.Spellblade.SpellbladeLv8Path2>(), 1);
            recipe.AddIngredient(ModContent.GetInstance<ApacchiisClassesMod.Items.Tokens.ClassTokenUltimate>(), 1);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
		
		public override void UpdateAccessory (Player player, bool hideVisual)
		{
            ACCPlayer accPlayer = Main.player[player.whoAmI].GetModPlayer<ACCPlayer>();
            var acmPlayer = player.GetModPlayer<ApacchiisClassesMod.MyPlayer>();
            acmPlayer.hasEquippedClass = true;
            accPlayer.hasSpellblade = true;
            accPlayer.hasClassPath2 = true;

            if (player.HeldItem.magic)
            {
                player.magicDamage += .14f;
                player.manaCost -= .12f;
                player.statManaMax2 += 40;
            }

            if (player.HeldItem.melee)
            {
                player.magicDamage += .18f;
                player.meleeCrit += 10;
            }

            accPlayer.shokkZoneTimerBase = 50;
            accPlayer.magicBladeBaseDamage = 60;
            accPlayer.magicBladeBaseCost = 28;
            player.manaCost -= .08f;
        }
    }
}

