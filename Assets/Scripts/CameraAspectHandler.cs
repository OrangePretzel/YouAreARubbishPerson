using UnityEngine;

namespace SSV_LudumDare40
{
	[RequireComponent(typeof(Camera))]
	public class CameraAspectHandler : MonoBehaviour
	{
		public const float TARGET_ASPECT_RATIO = 16f / 9f;

		private new Camera camera;

		private void OnEnable()
		{
			camera = GetComponent<Camera>();
			FixAspectRatio();
		}

		public void FixAspectRatio()
		{
			float sWidth = Screen.width;
			float sHeight = Screen.height;
			float sAspect = sWidth / sHeight;

			float aspectRatioRatio = sAspect / TARGET_ASPECT_RATIO;

			var rect = new Rect();
			if (aspectRatioRatio < 1)
			{
				rect.width = 1.0f;
				rect.height = aspectRatioRatio;
				rect.x = 0;
				rect.y = (1.0f - aspectRatioRatio) / 2.0f;

				camera.rect = rect;
			}
			else
			{
				float inverseRatio = 1 / aspectRatioRatio;

				rect.width = inverseRatio;
				rect.height = 1.0f;
				rect.x = (1.0f - inverseRatio) / 2.0f;
				rect.y = 0;

				camera.rect = rect;
			}
		}
	}
}