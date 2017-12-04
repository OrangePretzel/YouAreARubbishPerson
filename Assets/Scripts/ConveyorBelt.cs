using System.Collections.Generic;
using UnityEngine;

namespace SSV_LudumDare40
{
	public class ConveyorBelt : MonoBehaviour
	{
		public float ConveyorSpeed = -2.0f;
		public List<GameObject> ConveyorParts = new List<GameObject>();
		public Transform StartPoint;
		public Transform EndPoint;

		private List<GameObject> ObjectsOnBelt = new List<GameObject>();

		private void Update()
		{
			if (GameManager.Paused) return;

			var offset = new Vector3(0, Time.deltaTime * ConveyorSpeed, 0);

			foreach (var convPart in ConveyorParts)
			{
				var newPos = convPart.transform.position + offset;
				if (newPos.y <= EndPoint.position.y)
				{
					newPos.y = StartPoint.position.y + (newPos.y - EndPoint.position.y);
				}
				convPart.transform.position = newPos;
			}

			foreach (var convObj in ObjectsOnBelt)
			{
				var newPos = convObj.transform.position + offset;
				convObj.transform.position = newPos;
			}
		}

		private void OnTriggerEnter2D(Collider2D collider)
		{
			if (collider.tag == TAGS.WASTE_OBJECT_TAG)
				ObjectsOnBelt.Add(collider.gameObject);
		}

		private void OnTriggerExit2D(Collider2D collider)
		{
			if (ObjectsOnBelt.Contains(collider.gameObject))
				ObjectsOnBelt.Remove(collider.gameObject);
		}
	}
}