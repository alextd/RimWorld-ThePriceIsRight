using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace The_Price_Is_Right
{
	[DefOf]
	public static class ThoughtDefOf
	{
		public static ThoughtDef Traveling;
	}


	public class ThoughtWorker_Traveling : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (p.Map != null && p.Map.IsPlayerHome)
			{
				return ThoughtState.Inactive;
			}
			return ThoughtState.ActiveAtStage(0);
		}
	}
}
