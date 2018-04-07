using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;
using TD.Utilities;

namespace The_Price_Is_Right
{
	[DefOf]
	public static class ThoughtDefOf
	{
		public static ThoughtDef Traveling;
	}


	class Settings : ModSettings
	{
		public float tradeBonus = 0.30f;
		public bool bestPrice = true;
		public bool fairPrice = false;

		public bool moodBonus = true;
		public float moodBonus0 = 25;
		public float moodBonus1 = 35;

		public static Settings Get()
		{
			return LoadedModManager.GetMod<The_Price_Is_Right.Mod>().GetSettings<Settings>();
		}

		public void DoWindowContents(Rect wrect)
		{
			var options = new Listing_Standard();
			options.Begin(wrect);

			options.SliderLabeled("SettingTradeBonus".Translate(), ref tradeBonus, "{0:P0}", .02f, .50f);
			options.Label("SettingBonusAlso".Translate());
			options.CheckboxLabeled("SettingBestPrice".Translate(), ref bestPrice, "SettingBestPriceDesc".Translate());
			fairPrice &= !bestPrice;
			options.CheckboxLabeled("SettingFairPrice".Translate(), ref fairPrice, "SettingFairPriceDesc".Translate());
			options.Label("SettingOtherwisePrice".Translate());
			bestPrice &= !fairPrice;

			options.Gap();

			options.CheckboxLabeled("SettingMoodBonus".Translate(), ref moodBonus);
			if (moodBonus)
			{
				options.SliderLabeled("SettingMoodBonusNoBeds".Translate(), ref moodBonus0, "+{0:0}", 0, 50);
				options.SliderLabeled("SettingMoodBonusBeds".Translate(), ref moodBonus1, "+{0:0}", 0, 50);
				ThoughtDefOf.Traveling.stages[0].baseMoodEffect = moodBonus0;
				ThoughtDefOf.Traveling.stages[1].baseMoodEffect = moodBonus1;
			}

			options.End();
		}
		
		public override void ExposeData()
		{
			Scribe_Values.Look(ref tradeBonus, "tradeBonus", 0.30f);
			Scribe_Values.Look(ref bestPrice, "bestPrice", true);
			Scribe_Values.Look(ref fairPrice, "fairPrice", false);

			Scribe_Values.Look(ref moodBonus, "moodBonus", true);
			Scribe_Values.Look(ref moodBonus0, "moodBonus0", 25f);
			Scribe_Values.Look(ref moodBonus1, "moodBonus1", 35f);

			if(Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				ThoughtDefOf.Traveling.stages[0].baseMoodEffect = moodBonus0;
				ThoughtDefOf.Traveling.stages[1].baseMoodEffect = moodBonus1;
			}
		}
	}
}