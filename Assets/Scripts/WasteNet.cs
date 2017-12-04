using UnityEngine;

namespace SSV_LudumDare40
{
	[RequireComponent(typeof(Collider))]
	public class WasteNet : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			if (other.tag == TAGS.WASTE_OBJECT_TAG)
			{
				GameManager.RecycleWaste(other.gameObject);
			}
			else if(other.tag == TAGS.PLAYER_OBJECT_TAG)
			{
				GameManager.SpawnPlayer(other.gameObject);
			}
		}
	}
}