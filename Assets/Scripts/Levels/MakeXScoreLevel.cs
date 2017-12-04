namespace SSV_LudumDare40
{
	public class MakeXScoreLevel : Level
	{
		public int TargetScore;

		public override void AddScore(float score)
		{
			base.AddScore(score);

			if (LevelScore >= TargetScore)
			{
				SpawnRemainingWaste();
				isLevelComplete = true;
			}
		}

		public override bool CalculateGameOver()
		{
			return TargetScore > LevelScore;
		}

		public override string GetTargetString()
		{
			return $"{LevelScore} / {TargetScore} units of waste";
		}
	}
}