using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Classes.Esper
{
	public class EsperLv8 : BaseClass
	{
        Player player = Main.player[Main.myPlayer];
        Mod EsperClassMod = ModLoader.GetMod("EsperClass");

        public override void SetStaticDefaults()
        {
            if (EsperClassMod != null)
            {
                DisplayName.SetDefault("Class: Esper Lv.8");
                Tooltip.SetDefault("+28% Telekinetic Damage\n" +
                                   "+16% Telekinetic Crit Chance\n" +
                                   "+80% Telekinetic Velocity\n" +
                                   "[c/af7A6:[Path 1][c/af7A6:]]\n" +
                                   "+5% TK Dodge chance\n" +
                                   "+25% Ability Damage (Affects [Telekinetic Repulsion] field size and knockback)\n" +
                                   "[Telekinetic Repulsion] Destroy enemy projectiles up to 30 damage + 50% * defense\n" +
                                   "[Telekinetic Repulsion] (60 damage + 75% * defense on expert mode)\n" +
                                   "(By: TheLoneGamer)\n" +
                                   "(Custom Sounds By: Peb)");
            }
            else
            {
                DisplayName.SetDefault("Class: Esper Lv.8");
                Tooltip.SetDefault("[c/d33838:This class requires the mod 'Esper Class' to work]\n" +
                                   "[c/d33838:You can download it through the Mod Browser]");
            }
        }

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.accessory = true;
			Item.value = 0;
			Item.rare = 8;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.GetInstance<ApacchiisClassesMod.Items.Tokens.ClassTokenLv8>(), 1);
            recipe.AddIngredient(ModContent.GetInstance<EsperLv7>());
            recipe.SetResult(this);
			recipe.AddRecipe();
		}

        public override void UpdateAccessory (Player player, bool hideVisual)
		{
            ACCPlayer accPlayer = Main.player[player.whoAmI].GetModPlayer<ACCPlayer>();
            var acmPlayer = player.GetModPlayer<ApacchiisClassesMod.MyPlayer>();
            if (EsperClassMod != null)
            {
                player.GetModPlayer<ApacchiisClassesMod.MyPlayer>().hasEquippedClass = true;
                accPlayer.hasEsper = true;
                accPlayer.hasClassPath1 = true;
				acmPlayer.abilityDamage += 0.25f;

                //Reflection for cross-mod compatability without hard references
                ModPlayer ECPlayer = player.GetModPlayer<>(EsperClassMod);
                Type ECPlayerType = ECPlayer.GetType();

                // Telekinetic Damage
                FieldInfo tkDamage = ECPlayerType.GetField("tkDamage", BindingFlags.Instance | BindingFlags.Public);
                float oldtkDamage = (float)tkDamage.GetValue(ECPlayer);
                tkDamage.SetValue(ECPlayer, oldtkDamage + 0.28f);

                // Telekinetic Crit
                FieldInfo tkCrit = ECPlayerType.GetField("tkCrit", BindingFlags.Instance | BindingFlags.Public);
                int oldtkCrit = (int)tkCrit.GetValue(ECPlayer);
                tkCrit.SetValue(ECPlayer, oldtkCrit + 16);

                // Telekinetic Velocity
                FieldInfo tkVel = ECPlayerType.GetField("tkVel", BindingFlags.Instance | BindingFlags.Public);
                float oldtkVel = (float)tkVel.GetValue(ECPlayer);
                tkVel.SetValue(ECPlayer, oldtkVel + 0.8f);

                // Telekinetic Dodge
                FieldInfo tkDodge = ECPlayerType.GetField("tkDodge", BindingFlags.Instance | BindingFlags.Public);
                float oldtkDodge = (float)tkDodge.GetValue(ECPlayer);
                tkDodge.SetValue(ECPlayer, oldtkDodge + 0.05f);
            }
        }
    }
}
