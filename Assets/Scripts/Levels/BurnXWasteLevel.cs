using UnityEngine;

namespace SSV_LudumDare40
{
	public class BurnXWasteLevel : Level
	{
		public int TargetWasteBurned;

		public override void AddWasteBurned(GameObject waste)
		{
			base.AddWasteBurned(waste);

			if (LevelWasteBurned >= TargetWasteBurned)
			{
				SpawnRemainingWaste();
				isLevelComplete = true;
			}
		}

		public override bool CalculateGameOver()
		{
			return TargetWasteBurned > LevelWasteBurned;
		}

		public override string GetTargetString()
		{
			return $"{LevelWasteBurned} / {TargetWasteBurned} units of waste";
		}
	}
}