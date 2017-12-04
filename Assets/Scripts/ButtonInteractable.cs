using UnityEngine;
using UnityEngine.Events;

namespace SSV_LudumDare40
{
	public class ButtonInteractable : MonoBehaviour, IInteractable
	{
		public UnityEvent OnInteractEvent;

		private void OnEnable()
		{
			tag = TAGS.INTERACTABLE_OBJECT_TAG;
		}

		public void Interact(Player player)
		{
			OnInteractEvent?.Invoke();
		}
	}
}