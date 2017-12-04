namespace SSV_LudumDare40
{
	public class ScalingMakeXScoreLevel : MakeXScoreLevel
	{
		public override void Activate(int carryOverWaste)
		{
			base.Activate(carryOverWaste);

			TargetScore = GameManager.PlayerCount * GameManager.LevelCount * TargetScore;
		}
	}
}