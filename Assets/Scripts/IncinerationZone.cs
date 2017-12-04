using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SSV_LudumDare40
{
	public class IncinerationZone : MonoBehaviour
	{
		public static string[] BurnableTags = new string[] { TAGS.WASTE_OBJECT_TAG };

		public PhysicsColliderTracker IncinerationArea;
		public ParticleSystem FireParticleSystem;

		private List<Collider> collidersToIncinerate;
		private bool burning = false;
		private float destroyRate;
		private float timeSinceLastDestroy;

		private void FixedUpdate()
		{
			if (!burning) return;
			if (FireParticleSystem.isPlaying)
			{
				timeSinceLastDestroy += Time.deltaTime;
				if (timeSinceLastDestroy >= destroyRate && collidersToIncinerate.Count > 0)
				{
					DestroyRandomObject();
					timeSinceLastDestroy = 0;
				}
			}
			else
				FinishIncineration();

		}

		public void TriggerIncineration()
		{
			if (burning)
				return;

			burning = true;
			FireParticleSystem.Play();

			collidersToIncinerate = IncinerationArea.RescanArea()
				.Where(c => BurnableTags.Contains(c.tag))
				.ToList();

			destroyRate = FireParticleSystem.main.duration * 0.9f / collidersToIncinerate.Count;
			timeSinceLastDestroy = 0;
		}

		public void QuickFinishBurn()
		{
			if (!burning) return;

			while (collidersToIncinerate.Count > 0)
				DestroyRandomObject();
		}

		private void DestroyRandomObject()
		{
			var choice = collidersToIncinerate[Random.Range(0, collidersToIncinerate.Count)];
			collidersToIncinerate.Remove(choice);
			if (choice.bounds.Intersects(IncinerationArea.collider.bounds))
			{
				GameManager.WastePool.ReturnObject(choice.gameObject);
				GameManager.AddScore_IncineratedWaste(choice.gameObject);
			}
		}

		private void FinishIncineration()
		{
			collidersToIncinerate.Clear();
			burning = false;
		}
	}
}