using UnityEngine;

namespace SSV_LudumDare40
{
	public class WasteSpawner : MonoBehaviour
	{
		public Transform WasteSpawnPoint;

		public bool InfiniteWaste;
		public int WasteToSpawn = 20;
		public float SpawnDelay = 2.0f;

		public int TotalWasteSpawned;

		private float timeSinceLastSpawn;

		private void Start()
		{
			timeSinceLastSpawn = 0;
		}

		private void FixedUpdate()
		{
			if (GameManager.Paused) return;

			timeSinceLastSpawn += Time.deltaTime;
			if (timeSinceLastSpawn >= SpawnDelay)
			{
				SpawnWaste();
			}
		}

		private void SpawnWaste()
		{
			if (!InfiniteWaste && TotalWasteSpawned >= WasteToSpawn)
			{
				// Done spawning
				enabled = false;
				return;
			}

			timeSinceLastSpawn = 0;
			var wasteObj = GameManager.WastePool.GetObject();
			wasteObj.transform.position = WasteSpawnPoint.transform.position;

			TotalWasteSpawned++;
			GameManager.AddWaste(wasteObj);
		}

		public void SpawnRemainingWasteQuick()
		{
			if (InfiniteWaste)
				return; // Can't spawn infinite waste ;)

			var wasteToSpawn = WasteToSpawn - TotalWasteSpawned;
			for (int i = 0; i < wasteToSpawn; i++)
			{
				SpawnWaste();
			}
		}
	}
}