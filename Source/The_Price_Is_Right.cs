﻿using System.Reflection;
using Verse;
using UnityEngine;
using Harmony;
using RimWorld;

namespace The_Price_Is_Right
{
	public class Mod : Verse.Mod
	{
		public Mod(ModContentPack content) : base(content)
		{
			// initialize settings
			// GetSettings<Settings>();
#if DEBUG
			HarmonyInstance.DEBUG = true;
#endif
			HarmonyInstance harmony = HarmonyInstance.Create("Uuugggg.rimworld.The_Price_Is_Right.main");
			
			//Turn off DefOf warning since harmony patches trigger it.
			MethodInfo DefOfHelperInfo = AccessTools.Method(typeof(DefOfHelper), "EnsureInitializedInCtor");
			if (!harmony.GetPatchedMethods().Contains(DefOfHelperInfo))
				harmony.Patch(DefOfHelperInfo, new HarmonyMethod(typeof(Mod), "EnsureInitializedInCtorPrefix"), null);
			
			harmony.PatchAll();
		}

		public static bool EnsureInitializedInCtorPrefix()
		{
			//No need to display this warning.
			return false; 
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			base.DoSettingsWindowContents(inRect);
			GetSettings<Settings>().DoWindowContents(inRect);
		}

		public override string SettingsCategory()
		{
			return "TD.ThePriceIsRight".Translate();
		}
	}
}