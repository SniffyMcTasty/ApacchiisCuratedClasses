using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Classes.Esper
{
	public class EsperLv5 : BaseClass
	{
        Player player = Main.player[Main.myPlayer];
        Mod EsperClassMod = ModLoader.GetMod("EsperClass");

        public override void SetStaticDefaults()
        {
            if (EsperClassMod != null)
            {
                DisplayName.SetDefault("Class: Esper Lv.5 [Path 1]");
                Tooltip.SetDefault("+17.5% Telekinetic Damage\n" +
                                   "+10% Telekinetic Crit Chance\n" +
                                   "+50% Telekinetic Velocity\n" +
                                   "[c/af7A6:[Path 1][c/af7A6:]]\n" +
                                   "+5% TK Dodge Chance\n" +
                                   "+25% Ability Damage (Affects [Telekinetic Repulsion] field size and knockback)\n" +
                                   "[Telekinetic Repulsion] Destroy enemy projectiles up to 30 damage + 50% * defense\n" +
                                   "[Telekinetic Repulsion] (60 damage + 75% * defense on expert mode)\n" +
                                   "(By: TheLoneGamer)\n" +
                                   "(Custom Sounds By: Peb)");
            }
            else
            {
                DisplayName.SetDefault("Class: Esper Lv.5 [Path 1]");
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
			Item.rare = 5;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.GetInstance<ApacchiisClassesMod.Items.Tokens.ClassTokenLv5>(), 1);
            recipe.AddIngredient(ModContent.GetInstance<EsperLv4>());
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
                tkDamage.SetValue(ECPlayer, oldtkDamage + 0.175f);

                // Telekinetic Crit
                FieldInfo tkCrit = ECPlayerType.GetField("tkCrit", BindingFlags.Instance | BindingFlags.Public);
                int oldtkCrit = (int)tkCrit.GetValue(ECPlayer);
                tkCrit.SetValue(ECPlayer, oldtkCrit + 10);

                // Telekinetic Velocity
                FieldInfo tkVel = ECPlayerType.GetField("tkVel", BindingFlags.Instance | BindingFlags.Public);
                float oldtkVel = (float)tkVel.GetValue(ECPlayer);
                tkVel.SetValue(ECPlayer, oldtkVel + 0.5f);

                // Telekinetic Dodge
                FieldInfo tkDodge = ECPlayerType.GetField("tkDodge", BindingFlags.Instance | BindingFlags.Public);
                float oldtkDodge = (float)tkDodge.GetValue(ECPlayer);
                tkDodge.SetValue(ECPlayer, oldtkDodge + 0.05f);
            }
        }
    }
}
