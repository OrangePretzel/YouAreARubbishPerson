using System.Collections.Generic;
using UnityEngine;

namespace SSV_LudumDare40
{
	[RequireComponent(typeof(Collider))]
	public class PhysicsColliderTracker : MonoBehaviour
	{
		public new Collider collider;

		private void OnEnable()
		{
			collider = GetComponent<Collider>();
		}

		public List<Collider> RescanArea()
		{
			var intersectingColliders = new List<Collider>();
			var hits = Physics.SphereCastAll(transform.position, collider.bounds.extents.magnitude, Vector3.up);
			foreach (var hit in hits)
			{
				if (collider.bounds.Intersects(hit.collider.bounds))
					intersectingColliders.Add(hit.collider);
			}
			return intersectingColliders;
		}
	}
}