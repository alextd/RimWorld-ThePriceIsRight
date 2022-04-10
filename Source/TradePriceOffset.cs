using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using RimWorld;
using HarmonyLib;
using Verse;
using RimWorld.Planet;

namespace The_Price_Is_Right
{

	[HarmonyPatch(typeof(Settlement_TraderTracker), "TradePriceImprovementOffsetForPlayer", MethodType.Getter)]
	static class TradePriceOffset
	{
		//Settlement_TraderTracker.TradePriceImprovementOffsetForPlayer.GetGetMethod
		public static void Postfix(ref float __result)
		{
			__result = Mod.settings.tradeBonus;	//not 0.02f :/
		}
	}

	[HarmonyPatch(typeof(Tradeable))]
	[HarmonyPatch("InitPriceDataIfNeeded")]
	static class BuySellCollapser
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			FieldInfo priceBuyInfo = AccessTools.Field(typeof(RimWorld.Tradeable), "pricePlayerBuy");
			FieldInfo priceSellInfo = AccessTools.Field(typeof(RimWorld.Tradeable), "pricePlayerSell");

			MethodInfo AdjustPricesInfo = AccessTools.Method(typeof(BuySellCollapser), nameof(BuySellCollapser.AdjustPrices));

			List<CodeInstruction> instList = instructions.ToList();
			for (int i = 0; i < instList.Count; i++)
			{
				CodeInstruction inst = instList[i];
				//IL_00e9: ldarg.0      // this
				//IL_00ea: ldarg.0      // this
				//IL_00eb: ldfld float32 RimWorld.Tradeable::pricePlayerBuy
				//IL_00f0: stfld float32 RimWorld.Tradeable::pricePlayerSell
				if (inst.StoresField(priceSellInfo)
					&& instList[i-1].LoadsField(priceBuyInfo))
				{
					//Tradeable this, pricePlayerBuy on stack
					yield return new CodeInstruction(OpCodes.Call, AdjustPricesInfo);//AdjustPrices(Tradeable)
				}
				else
					yield return inst;
			}
		}

		public static AccessTools.FieldRef<Tradeable, float> PricePlayerBuy =
			AccessTools.FieldRefAccess<Tradeable, float>("pricePlayerBuy");
		public static AccessTools.FieldRef<Tradeable, float> PricePlayerSell =
			AccessTools.FieldRefAccess<Tradeable, float>("pricePlayerSell");
		public static void AdjustPrices(Tradeable item, float buyPrice)
		{
			if (Mod.settings.bestPrice) return;

			float sellPrice = PricePlayerSell(item);

			if (Mod.settings.fairPrice && item.FirstThingColony == null)
				sellPrice = buyPrice;
			else if (Mod.settings.fairPrice && item.FirstThingTrader == null)
				buyPrice = sellPrice;
			else
				buyPrice = (sellPrice = (buyPrice + sellPrice) / 2);

			PricePlayerBuy(item) = buyPrice;
			PricePlayerSell(item) = sellPrice;
		}
	}
}
