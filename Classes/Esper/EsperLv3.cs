using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Classes.Esper
{
	public class EsperLv3 : BaseClass
	{
        Player player = Main.player[Main.myPlayer];
        Mod EsperClassMod = ModLoader.GetMod("EsperClass");

        public override void SetStaticDefaults()
        {
            if(EsperClassMod != null)
            {
                DisplayName.SetDefault("Class: Esper Lv.3");
                Tooltip.SetDefault("+10.5% Telekinetic Damage\n" +
                                   "+6% Telekinetic Crit Chance\n" +
                                   "+30% Telekinetic Velocity\n" +
                                   "(By: TheLoneGamer)\n" +
                                   "(Custom Sounds By: Peb)");
            }
            else
            {
                DisplayName.SetDefault("Class: Esper Lv.3");
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
			Item.rare = 3;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.GetInstance<ApacchiisClassesMod.Items.Tokens.ClassTokenLv3>(), 1);
            recipe.AddIngredient(ModContent.GetInstance<EsperLv2>());
            recipe.SetResult(this);
			recipe.AddRecipe();
		}

        public override void UpdateAccessory (Player player, bool hideVisual)
		{
            ACCPlayer accPlayer = Main.player[player.whoAmI].GetModPlayer<ACCPlayer>();
            if (EsperClassMod != null)
            {
                player.GetModPlayer<ApacchiisClassesMod.MyPlayer>().hasEquippedClass = true;
                accPlayer.hasEsper = true;

                //Reflection for cross-mod compatability without hard references
                ModPlayer ECPlayer = player.GetModPlayer<>(EsperClassMod);
                Type ECPlayerType = ECPlayer.GetType();

                // Telekinetic Damage
                FieldInfo tkDamage = ECPlayerType.GetField("tkDamage", BindingFlags.Instance | BindingFlags.Public);
                float oldtkDamage = (float)tkDamage.GetValue(ECPlayer);
                tkDamage.SetValue(ECPlayer, oldtkDamage + 0.105f);

                // Telekinetic Crit
                FieldInfo tkCrit = ECPlayerType.GetField("tkCrit", BindingFlags.Instance | BindingFlags.Public);
                int oldtkCrit = (int)tkCrit.GetValue(ECPlayer);
                tkCrit.SetValue(ECPlayer, oldtkCrit + 6);

                // Telekinetic Velocity
                FieldInfo tkVel = ECPlayerType.GetField("tkVel", BindingFlags.Instance | BindingFlags.Public);
                float oldtkVel = (float)tkVel.GetValue(ECPlayer);
                tkVel.SetValue(ECPlayer, oldtkVel + 0.3f);
            }
        }
    }
}
