using UnityEngine;
using UnityEngine.UI;

namespace SSV_LudumDare40
{
	public class HUDManager : MonoBehaviour
	{
		public GameObject ScorePanel;
		public Text LevelScore;
		public Text LevelWaste;

		public GameObject WarningTimerPanel;
		public GameObject TimerPanel;
		public Text WarningTimer;
		public Text Timer;

		public GameObject TargetPanel;
		public Text TargetString;

		private void FixedUpdate()
		{
			if (GameManager.CurrentLevel != null && !GameManager.CurrentLevel.IsMenuLevel)
			{
				if (!ScorePanel.activeSelf)
					ScorePanel.SetActive(true);

				if (!GameManager.CurrentLevel.IsTimedLevel)
				{
					if (!TimerPanel.activeSelf)
						TimerPanel.SetActive(true);
					if (WarningTimerPanel.activeSelf && !GameManager.CurrentLevel.WarningMode)
						WarningTimerPanel.SetActive(false);
				}
				else
				{
					if (TimerPanel.activeSelf)
						TimerPanel.SetActive(false);
					if (!WarningTimerPanel.activeSelf)
						WarningTimerPanel.SetActive(true);
				}

				if (GameManager.CurrentLevel.HasTarget)
				{
					if (!TargetPanel.activeSelf)
						TargetPanel.SetActive(true);
					TargetString.text = GameManager.CurrentLevel.GetTargetString();
				}
				else
				{
					if (TargetPanel.activeSelf)
						TargetPanel.SetActive(false);
				}

				LevelScore.text = $"{GameManager.CurrentLevel.LevelScore} pts";
				LevelWaste.text = $"{GameManager.CurrentLevel.LevelWasteBurned} units";

				Timer.text = $"{(int)(GameManager.CurrentLevel.LevelTime / 60):00}:{(int)(GameManager.CurrentLevel.LevelTime % 60):00}";
				if (GameManager.CurrentLevel.WarningMode || GameManager.CurrentLevel.IsTimedLevel)
				{
					if (!WarningTimerPanel.activeSelf)
						WarningTimerPanel.SetActive(true);
					WarningTimer.text = $"! {(int)(GameManager.CurrentLevel.RemainingTime / 60):00}:{(int)(GameManager.CurrentLevel.RemainingTime % 60):00} !";
					WarningTimer.fontSize = GameManager.CurrentLevel.RemainingTime > 5 ? 24 : 34;
				}
			}
			else
			{
				if (ScorePanel.activeSelf)
					ScorePanel.SetActive(false);
				if (TimerPanel.activeSelf)
					TimerPanel.SetActive(false);
				if (WarningTimerPanel.activeSelf)
					WarningTimerPanel.SetActive(false);
				if (TargetPanel.activeSelf)
					TargetPanel.SetActive(false);
			}
		}
	}
}