using Terraria;
using Terraria.ModLoader;

namespace ApacchiisCuratedClasses.Classes;

public class BaseClass : ModItem
{
    public override bool CanEquipAccessory(Player player, int slot, bool modded)
    {
        return !player.GetModPlayer<ApacchiisClassesMod.MyPlayer>().hasEquippedClass && base.CanEquipAccessory(player, slot, true);
    }
    
    public override bool ReforgePrice(ref int reforgePrice, ref bool canApplyDiscount)
    {
        reforgePrice = 150000;
        return base.ReforgePrice(ref reforgePrice, ref canApplyDiscount);
    }
}