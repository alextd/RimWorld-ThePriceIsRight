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

			options.SliderLabeled("Your Caravan trade bonus:", ref tradeBonus, "{0:P0}", .02f, .50f);
			options.Label("Negotiator skill and game difficulty applies on top on this.");
			options.CheckboxLabeled("Use best price even if unfair", ref bestPrice, "Price bonus applies even if the sell price is higher than the buy price. I trust you not to exploit it for easy cash.");
			fairPrice &= !bestPrice;
			options.CheckboxLabeled("Use best price if only the buyer or seller has item", ref fairPrice, "Prices bonus applies if one side has the item. A fair compromise. Stil, don't re-open the trade window");
			options.Label("Otherwise, when the buy price is lower than the sell price, they are both averaged");
			bestPrice &= !fairPrice;

			options.Gap();

			options.CheckboxLabeled("Mood bonus for being in a caravan", ref moodBonus);
			if (moodBonus)
			{
				options.SliderLabeled("Mood bonus (no beds)", ref moodBonus0, "+{0:0}", 0, 50);
				options.SliderLabeled("Mood bonus (beds for everyone)", ref moodBonus1, "+{0:0}", 0, 50);
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