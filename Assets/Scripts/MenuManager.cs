using UnityEngine;
using UnityEngine.UI;

namespace SSV_LudumDare40
{
	public class MenuManager : MonoBehaviour
	{
		public HUDManager HUDManager;

		public GameObject UIRoot;

		public GameObject MainMenuScreen;
		public Text QuitButtonText;

		public GameObject OptionsScreen;

		public GameObject ScoreCardScreen;
		public Text ScoreBoard_Title;
		public Text ScoreBoard_SubTitle;
		public Text ScoreBoard_LevelScore;
		public Text ScoreBoard_LevelWaste;
		public Text ScoreBoard_TotalScore;
		public Text ScoreBoard_TotalWaste;
		public Text ScoreBoard_LevelTime;
		public GameObject ScoreBoard_NextLevelButton;


		public GameObject CurrentScreen;

		private bool closed = false;

		public void OpenMenu()
		{
			if (!closed)
				return;
			closed = false;
			UIRoot.SetActive(true);

			if (GameManager.CurrentLevel != null && !GameManager.CurrentLevel.IsMenuLevel)
			{
				QuitButtonText.text = "Quit to Main Menu";
			}
			else
			{
				QuitButtonText.text = "Quit to Desktop";
			}
		}

		public void CloseMenu()
		{
			if (closed)
				return;
			closed = true;
			UIRoot.SetActive(false);
		}

		public void GotoScreen(GameObject screen)
		{
			CurrentScreen?.SetActive(false);
			CurrentScreen = screen;
			CurrentScreen.SetActive(true);
		}

		public void ContinueGame()
		{
			GameManager.TogglePaused();
			CloseMenu();
		}

		public void NewGame(bool tutorial = false)
		{
			GameManager.StartNewGame(tutorial, true, false);
			CloseMenu();
		}

		public void NewChallengeGame()
		{
			GameManager.StartNewGame(false, false, true);
			CloseMenu();
		}

		public void NewMainMenuGame()
		{
			GameManager.NewMainMenuGame();
			CloseMenu();
		}

		public void QuitGame()
		{
			if (GameManager.CurrentLevel != null && !GameManager.CurrentLevel.IsMenuLevel)
			{
				NewMainMenuGame();
				return;
			}

			if (Application.isEditor)
			{
				Debug.Log("No quitting for you buddy. You're trash!");
				return;
			}

			Application.Quit();
		}

		public void DisplayScoreCard(LevelResult result)
		{
			GameManager.PauseGame();
			OpenMenu();
			GotoScreen(ScoreCardScreen);
			ScoreBoard_Title.text = result.GameOver ? "GAME OVER" : result.LevelName;
			ScoreBoard_SubTitle.text = result.GameOver ? "You are a rubbish person!" : "Level Complete!";
			ScoreBoard_LevelScore.text = $"{result.ScoreDelta} pts";
			ScoreBoard_LevelWaste.text = $"{result.WasteDelta} units";
			ScoreBoard_TotalScore.text = $"{GameManager.Score} pts";
			ScoreBoard_TotalWaste.text = $"{GameManager.WasteBurned} units";
			ScoreBoard_LevelTime.text = $"{(int)(result.LevelTime / 60)}:{(int)(result.LevelTime % 60)} min";
			ScoreBoard_NextLevelButton.SetActive(!result.GameOver);
			Debug.Log($@"
Level [{result.LevelName}] cleaned up.
Final Score: [{result.ScoreDelta}].
Waste Burned: [{result.WasteDelta}].
Remaining Waste: [{result.CarryOverWaste}]
Total Time: [{result.LevelTime}]
Game Over: [{result.GameOver}]
");
		}

		public void CloseScoreCard_NextLevel()
		{
			GameManager.GotoNextLevel();
			CloseMenu();
			GameManager.ResumeGame();
		}

		public void CloseScoreCard_MainMenu()
		{
			NewMainMenuGame();
		}
	}
}