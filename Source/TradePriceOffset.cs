using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using RimWorld;
using Harmony;
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
			__result = Settings.Get().tradeBonus;	//not 0.02f :/
		}
	}

	[HarmonyPatch(typeof(Tradeable))]
	[HarmonyPatch("InitPriceDataIfNeeded")]
	static class BuySellCollapser
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo ErrorOnceInfo = AccessTools.Method(typeof(Verse.Log), "ErrorOnce");

			MethodInfo AdjustPricesInfo = AccessTools.Method(typeof(BuySellCollapser), nameof(BuySellCollapser.AdjustPrices));
			
			
			foreach (CodeInstruction i in instructions)
			{
				if (i.opcode == OpCodes.Ldstr)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, AdjustPricesInfo);
					yield return new CodeInstruction(OpCodes.Ret) { labels = instructions.Last().labels };
					yield break;
				}
				else
					yield return i;
			}
		}

		public static FieldInfo pricePlayerBuyInfo = AccessTools.Field(typeof(Tradeable), "pricePlayerBuy");
		public static FieldInfo pricePlayerSellInfo = AccessTools.Field(typeof(Tradeable), "pricePlayerSell");
		public static void AdjustPrices(Tradeable item)
		{
			if (Settings.Get().bestPrice) return;

			float buyPrice = (float)pricePlayerBuyInfo.GetValue(item);
			float sellPrice = (float)pricePlayerSellInfo.GetValue(item);

			if (Settings.Get().fairPrice && item.FirstThingColony == null)
				sellPrice = buyPrice;
			else if (Settings.Get().fairPrice && item.FirstThingTrader == null)
				buyPrice = sellPrice;
			else
				buyPrice = (sellPrice = (buyPrice + sellPrice) / 2);

			pricePlayerBuyInfo.SetValue(item, buyPrice);
			pricePlayerSellInfo.SetValue(item, sellPrice);
		}
	}
}
