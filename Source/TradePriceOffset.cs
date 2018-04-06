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

	[HarmonyPatch(typeof(Settlement_TraderTracker), "get_TradePriceImprovementOffsetForPlayer")]
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
			FieldInfo pricePlayerBuyInfo = AccessTools.Field(typeof(Tradeable), "pricePlayerBuy");
			FieldInfo pricePlayerSellInfo = AccessTools.Field(typeof(Tradeable), "pricePlayerSell");
			
			foreach (CodeInstruction i in instructions)
			{
				if (i.opcode == OpCodes.Call && i.operand == ErrorOnceInfo)
				{
					yield return new CodeInstruction(OpCodes.Pop);//Remove args to ErrorOnce
					yield return new CodeInstruction(OpCodes.Pop);

					//Average instead of sell = buy
					//TODO: Use sell price if nothing to buy, vice versa
					yield return new CodeInstruction(OpCodes.Ldarg_0);//this (for lvalue)
					yield return new CodeInstruction(OpCodes.Ldarg_0);//this
					yield return new CodeInstruction(OpCodes.Ldfld, pricePlayerBuyInfo);//this.Buy
					yield return new CodeInstruction(OpCodes.Ldarg_0);//this
					yield return new CodeInstruction(OpCodes.Ldfld, pricePlayerSellInfo);//this.Sell
					yield return new CodeInstruction(OpCodes.Add);//this.Buy + this.Sell
					yield return new CodeInstruction(OpCodes.Ldc_R4, 2.0f);//2
					yield return new CodeInstruction(OpCodes.Div);//(this.Buy + this.Sell) / 2 aka average
					yield return new CodeInstruction(OpCodes.Stfld, pricePlayerBuyInfo);

					//if (FirstThingColony == null) sell = buy
					//if (FirstThingTrader == null) buy = sell
				}
				else
					yield return i;
			}
		}
	}
}
