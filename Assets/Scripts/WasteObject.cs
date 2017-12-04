using UnityEngine;

namespace SSV_LudumDare40
{
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(Rigidbody))]
	public class WasteObject : MonoBehaviour, IPooledObject
	{
		public new Rigidbody rigidbody;
		public new Collider collider;

		private void OnEnable()
		{
			rigidbody = GetComponent<Rigidbody>();
			collider = GetComponent<Collider>();
			tag = TAGS.WASTE_OBJECT_TAG;
		}

		public void GetThrown(Vector3 velocity)
		{
			rigidbody.velocity = velocity;
		}

		public void ActivateFromPool()
		{
			gameObject.SetActive(true);
		}

		public void DeactivateToPool()
		{
			gameObject.SetActive(false);
		}

		public GameObject GetGameObject() => gameObject;
	}
}