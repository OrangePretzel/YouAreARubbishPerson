using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SSV_LudumDare40
{
	public class Level : MonoBehaviour
	{
		public bool IsMenuLevel = false;

		public string LevelName;
		public int LevelWasteBurned;
		public float LevelScore;
		public float LevelTime;

		public int GarbageRainCount = 0;

		public bool HasTarget = false;

		public bool IsTimedLevel = false;
		public float GivenLevelTime = 60f;

		public float LevelCompleteDelay = 10.0f; // 10s delay on level complete
		public List<GameObject> wasteObjects = new List<GameObject>();

		public List<WasteSpawner> Spawners = new List<WasteSpawner>();
		public List<IncinerationZone> Incinerators = new List<IncinerationZone>();

		protected bool isLevelComplete = false;
		protected float levelCompleteDelay = 0;

		public bool WarningMode => isLevelComplete;
		public float RemainingTime;

		private void FixedUpdate()
		{
			if (!isLevelComplete)
				return;
			levelCompleteDelay += Time.deltaTime;
			// TODO: Show UI with remaining time
			//Debug.Log($"Level Ending in {Mathf.Max(0, LevelCompleteDelay - levelCompleteDelay)}");
			if (levelCompleteDelay >= LevelCompleteDelay)
			{
				GameManager.LevelComplete();
			}
		}

		private void Update()
		{
			if (GameManager.Paused) return;
			LevelTime += Time.deltaTime;

			if (IsTimedLevel)
			{
				if (!isLevelComplete)
					RemainingTime = GivenLevelTime - LevelTime;
				else
					RemainingTime = LevelCompleteDelay - levelCompleteDelay;
				if (RemainingTime <= LevelCompleteDelay && !isLevelComplete)
				{
					isLevelComplete = true;
				}
			}
			else
			{
				RemainingTime = LevelCompleteDelay - levelCompleteDelay;
			}
		}

		public virtual void Activate(int carryOverWaste)
		{
			LevelWasteBurned = 0;
			LevelScore = 0;

			foreach (var player in GameManager.Players)
			{
				GameManager.SpawnPlayer(player);
			}

			for (int i = 0; i < carryOverWaste + GarbageRainCount; i++)
			{
				var wasteObj = GameManager.WastePool.GetObject();
				wasteObj.transform.position = Vector3.up * (carryOverWaste + 15) + Vector3.left * Random.Range(-5.0f, 5.0f) + Vector3.forward * Random.Range(-5.0f, 5.0f);
				GameManager.AddWaste(wasteObj);
			}
		}

		public virtual LevelResult Cleanup()
		{
			// Remove any trash in the process of being burned
			foreach (var incinerator in Incinerators)
			{
				incinerator.QuickFinishBurn();
			}

			var remainingWaste = wasteObjects.Count;
			var unspawnedWaste = GetTotalUnspawnedWaste();
			for (int i = wasteObjects.Count - 1; i >= 0; i--)
			{
				var waste = wasteObjects[i];
				GameManager.WastePool.ReturnObject(waste);
			}

			var carryOverWaste = remainingWaste + unspawnedWaste;

			var result = new LevelResult()
			{
				ScoreDelta = LevelScore,
				LevelName = LevelName,
				LevelTime = LevelTime,
				WasteDelta = LevelWasteBurned,
				CarryOverWaste = carryOverWaste,
				WasTimedLevel = IsTimedLevel,
				GivenLevelTime = GivenLevelTime,
				GameOver = CalculateGameOver() // TODO: Determine loss condition
			};
			GameManager.DestroyCurrentLevel();
			return result;
		}

		public virtual bool CalculateGameOver()
		{
			return false;
		}

		public virtual void AddWaste(GameObject waste)
		{
			wasteObjects.Add(waste);
		}

		public virtual void AddWasteBurned(GameObject waste)
		{
			LevelWasteBurned++;
			wasteObjects.Remove(waste);
		}

		public virtual void AddScore(float score)
		{
			LevelScore += score;
		}

		public virtual void RecycleWaste(GameObject waste)
		{
			GameManager.WastePool.ReturnObject(waste);
			WasteSpawner spawner;
			try { spawner = Spawners.First(s => s.enabled); }
			catch
			{
				Debug.Log("Reactivating spawner");
				spawner = Spawners[0];
				spawner.enabled = true;
			}

			Debug.Log("Lost waste");
			spawner.TotalWasteSpawned--;
			wasteObjects.Remove(waste);
		}

		public virtual Transform GetSpawnPoint(Player player)
		{
			return Spawners[Random.Range(0, Spawners.Count)].transform;
		}

		public virtual string GetTargetString()
		{
			return "No Target";
		}

		protected int GetTotalUnspawnedWaste()
		{
			return Spawners.Sum(s => s.InfiniteWaste ? 0 : s.WasteToSpawn - s.TotalWasteSpawned);
		}

		protected void SpawnRemainingWaste()
		{
			Spawners.ForEach(s => s.SpawnRemainingWasteQuick());
		}
	}
}