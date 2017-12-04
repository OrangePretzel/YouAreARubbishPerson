using UnityEngine;

namespace SSV_LudumDare40
{
	public interface IPooledObject
	{
		void ActivateFromPool();
		void DeactivateToPool();
		GameObject GetGameObject();
	}
}