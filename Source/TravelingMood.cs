using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace The_Price_Is_Right
{
	public class ThoughtWorker_Traveling : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn pawn)
		{
			if (!Settings.Get().moodBonus || (pawn.Map != null && pawn.Map.IsPlayerHome))
			{
				return ThoughtState.Inactive;
			}
			if(pawn.GetCaravan() is Caravan caravan)
			{
				List<Pawn> carvanPawns = caravan.PawnsListForReading;
				int bedCount = carvanPawns.Select(p => CountBeds(p)).Sum();
				int colonistCount = carvanPawns.FindAll(p => p.IsColonist).Count;
				if (bedCount >= colonistCount)
					return ThoughtState.ActiveAtStage(1);

			}
			return ThoughtState.ActiveAtStage(0);
		}

		public static int CountBeds(Pawn pawn)
		{
			return pawn.inventory.innerContainer.Where(t => t.GetInnerIfMinified() is Building_Bed).Count();
		}
	}
}
