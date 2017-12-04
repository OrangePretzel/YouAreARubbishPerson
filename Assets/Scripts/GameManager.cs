using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SSV_LudumDare40
{
	public class GameManager : MonoBehaviour
	{
		public bool QuickStart = false;

		public MenuManager menuManager;
		public static MenuManager MenuManager => instance.menuManager;

		public Object MainMenuLevel;
		public List<Object> TutorialLevels = new List<Object>();
		public List<Object> StandardLevels = new List<Object>();
		public List<Object> HundoGarbageLevels = new List<Object>();

		public const int INITIAL_GARBAGE_COUNT = 50;
		public Object GarbagePrefab;
		public SpawnPool wastePool;
		public static SpawnPool WastePool => instance.wastePool;

		private static GameManager instance;

		public static int PlayerCount => instance.players.Count;
		public int levelCount = 0;
		public static int LevelCount => instance.levelCount;

		public bool paused = false;
		public static bool Paused => instance.paused;

		public float score = 0;
		public static float Score => instance.score;
		public int wasteBurned = 0;
		public static int WasteBurned => instance.wasteBurned;

		public int carryOverWaste = 0;
		public static int CarryOverWaste { get { return instance.carryOverWaste; } set { instance.carryOverWaste = value; } }

		public bool hasLevelRunning = false;
		public static bool HasLevelRunning => instance.hasLevelRunning;
		public Level currentLevel;
		public static Level CurrentLevel => instance.currentLevel;
		private Queue<Object> levels;

		public List<Player> players = new List<Player>();
		public static List<Player> Players => instance.players;

		private void OnEnable()
		{
			if (instance != null && instance != this)
			{
				Destroy(this.gameObject);
				return;
			}
			else if (instance == this)
				return;

			instance = this;
			SceneManager.sceneLoaded += LevelLoaded;

			// Garbage pool
			if (wastePool == null)
			{
				var wastePoolContainer = new GameObject("WasteBin");
				wastePoolContainer.transform.parent = transform;
				wastePool = new SpawnPool(INITIAL_GARBAGE_COUNT, id =>
				{
					var gObj = Instantiate(GarbagePrefab) as GameObject;
					gObj.name = $"Garbage Bag {id}";
					var pooledObj = gObj.GetComponent<IPooledObject>();
					return pooledObj;
				}, wastePoolContainer.transform);
			}
		}

		private void Update()
		{
			if (Input.GetButtonDown("Pause"))
			{
				TogglePaused();
				if (Paused)
				{
					MenuManager.OpenMenu();
					MenuManager.GotoScreen(MenuManager.MainMenuScreen);
				}
				else
					MenuManager.CloseMenu();
			}
		}

		private void LevelLoaded(Scene scene, LoadSceneMode mode)
		{
			Debug.Log(scene);
			if (scene.name == "main" && QuickStart)
			{
				Debug.Log("QuickStart: Jumping straight into the action!");
				MenuManager.CloseMenu();
			}
		}

		public static void StartNewGame(bool tutorial, bool standard, bool challenge)
		{
			Debug.Log(tutorial);
			NewGame(tutorial, standard, challenge);
			GotoNextLevel();

			// TODO: Camera transition
			GameManager.ResumeGame();
		}

		public static void NewGame(bool tutorial, bool standard, bool challenge)
		{
			CleanupCurrentLevel();
			instance.levelCount = 0;
			instance.score = 0;
			instance.wasteBurned = 0;
			instance.carryOverWaste = 0;
			instance.levels = new Queue<Object>();

			if (tutorial)
			{
				foreach (var tutLevel in instance.TutorialLevels)
					instance.levels.Enqueue(tutLevel);
			}

			if (standard)
				foreach (var level in instance.StandardLevels)
					instance.levels.Enqueue(level);

			if (challenge)
				foreach (var level in instance.HundoGarbageLevels)
					instance.levels.Enqueue(level);
		}

		public static void GotoNextLevel()
		{
			CleanupCurrentLevel(); // Ignore scorecard
			var levelPrefab = GetNextLevelObject();
			Debug.Log(levelPrefab);
			var levelGameObject = Instantiate(levelPrefab) as GameObject;
			var level = levelGameObject.GetComponent<Level>();
			ActivateLevel(level);
		}

		public static void EndLevelWithScoreCard()
		{
			var levelResult = CleanupCurrentLevel();
			if (!levelResult.HasValue)
			{
				GotoNextLevel();
				return;
			}
			MenuManager.OpenMenu();
			MenuManager.DisplayScoreCard(levelResult.Value);
			if (!levelResult.Value.GameOver)
				CarryOverWaste = levelResult.Value.CarryOverWaste;
		}

		public static void LevelComplete()
		{
			EndLevelWithScoreCard();
		}

		private static LevelResult? CleanupCurrentLevel()
		{
			return instance.currentLevel?.Cleanup();
		}

		private static void ActivateLevel(Level newLevel)
		{
			instance.levelCount++;
			instance.currentLevel = newLevel;
			instance.currentLevel.Activate(0);
			CarryOverWaste = 0;
		}

		public static void NewMainMenuGame()
		{
			NewGame(false, false, false);
			instance.levels.Enqueue(instance.MainMenuLevel);
			GotoNextLevel();
			ResumeGame();
		}

		public static Object GetNextLevelObject()
		{
			if (instance.levels.Count > 0)
			{
				return instance.levels.Dequeue();
			}

			return instance.StandardLevels[Random.Range(0, instance.StandardLevels.Count)];
		}

		public static void AddScore_IncineratedWaste(GameObject waste)
		{
			instance.score += 10;
			instance.wasteBurned++;
			instance.currentLevel?.AddWasteBurned(waste);
			instance.currentLevel?.AddScore(10);
		}

		public static void AddWaste(GameObject waste)
		{
			instance.currentLevel?.AddWaste(waste);
		}

		public static void RecycleWaste(GameObject waste)
		{
			instance.currentLevel?.RecycleWaste(waste);
		}

		public static void SpawnPlayer(GameObject playerObj)
		{
			var player = playerObj.GetComponent<Player>();
			SpawnPlayer(player);
		}

		public static void SpawnPlayer(Player player)
		{
			if (!instance.players.Contains(player))
				instance.players.Add(player);
			player.SpawnAt(CurrentLevel.GetSpawnPoint(player));
		}

		public static void DestroyCurrentLevel()
		{
			Destroy(instance.currentLevel.gameObject);
			instance.currentLevel = null;
		}

		#region Pausing

		public static void TogglePaused()
		{
			if (Paused)
				ResumeGame();
			else
				PauseGame();
		}

		public static void PauseGame()
		{
			if (Paused)
				return;

			instance.paused = true;
			Physics.autoSimulation = false;
			Time.timeScale = 0;
		}

		public static void ResumeGame()
		{
			if (!Paused)
				return;

			instance.paused = false;
			Physics.autoSimulation = true;
			Time.timeScale = 1;
		}

		#endregion
	}
}