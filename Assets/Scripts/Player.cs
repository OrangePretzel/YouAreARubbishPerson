using System.Linq;
using UnityEngine;

namespace SSV_LudumDare40
{
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(Rigidbody))]
	public class Player : MonoBehaviour
	{
		public Animator Animator;

		public float PlayerSpeed = 3;
		public float DashDelay = 0.75f;
		public AnimationCurve DashSpeedModifier = AnimationCurve.EaseInOut(0, 2, 1, 1);

		public Collider HoldPointCollider;
		public Transform HoldPoint;
		public float GrabDelay = 0.25f;
		public float GrabTolerance = 1.0f;

		public float MinThrowForce = 6;
		public float ThrowForce = 3;
		public float MaxThrowForce = 15;
		public float ThrowUpForce = 3;
		public float ThrowPlayerVelocityInfluence = 0.2f;

		private new Rigidbody rigidbody;

		private float remainingDashDuration = 0;

		private WasteObject GrabbedObject;
		private float grabDelay = 0;

		private bool WindingUp = false;
		private float WindupDuration;

		public float SpawnVelocity = 4.0f;
		public float SpawnDuration = 2.0f;
		private float remainingSpawnDuration;

		private void OnEnable()
		{
			rigidbody = GetComponent<Rigidbody>();
			if (GrabbedObject == null)
				HoldPointCollider.enabled = false;
		}

		private void Update()
		{
			if (GameManager.Paused) return;

			if (remainingSpawnDuration > 0)
			{
				remainingSpawnDuration -= Time.deltaTime;
				return;
			}

			// Movement
			var moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

			// Dashing
			var dashDuration = DashSpeedModifier.keys[DashSpeedModifier.length - 1].time;
			if (remainingDashDuration > -DashDelay)
			{
				remainingDashDuration -= Time.deltaTime;
			}
			if (Input.GetButtonDown("Dash") && remainingDashDuration <= -DashDelay)
			{
				remainingDashDuration = dashDuration;
			}
			var dashSpeed = remainingDashDuration <= 0 ? 1 : DashSpeedModifier.Evaluate((dashDuration - remainingDashDuration) / dashDuration);

			// Velocity
			rigidbody.velocity = moveInput * PlayerSpeed * dashSpeed + new Vector3(0, rigidbody.velocity.y, 0);
			Animator.SetBool("IsMoving", moveInput.sqrMagnitude > 0);

			if (moveInput.sqrMagnitude > 0)
			{
				// TODO: Smooth rotation
				// Use -moveInput.z to correct z-axis rotation inversion
				transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(-moveInput.z, moveInput.x) * Mathf.Rad2Deg + 90, Vector3.up);
			}

			if (WindingUp)
				WindupDuration += Time.deltaTime;


			// Grab/Drop
			if (grabDelay > 0)
				grabDelay -= Time.deltaTime;


			if (Input.GetButtonDown("Grab") && grabDelay <= 0)
			{
				if (GrabbedObject != null)
				{
					DropObject();
				}
				else
				{
					GrabObject();
				}
				grabDelay = GrabDelay;
			}

			// Throw/Interact
			if (Input.GetButtonDown("Throw"))
			{
				if (GrabbedObject != null)
					StartThrowWindup();
				else
					Interact();
			}
			if (Input.GetButtonUp("Throw") && GrabbedObject != null)
			{
				ThrowObject();
			}


			// TODO: Maybe make this better?
			if (GrabbedObject != null)
			{
				if (!GrabbedObject.gameObject.activeSelf)
				{
					GrabbedObject = null;
				}
				else
					GrabbedObject.transform.position = HoldPoint.position;
			}
			Animator.SetBool("IsHoldingObject", GrabbedObject != null);
		}

		private void GrabObject()
		{
			var rayCastHits = Physics.SphereCastAll(HoldPoint.position, GrabTolerance, Vector3.up);
			if (rayCastHits.Length < 1)
				return;

			try
			{
				// Try to interact with something
				var moveableHit = rayCastHits.First(h => h.collider.tag == TAGS.WASTE_OBJECT_TAG);
				GrabObject(moveableHit.collider.gameObject);
			}
			catch { }
		}

		private void Interact()
		{
			var rayCastHits = Physics.SphereCastAll(HoldPoint.position, GrabTolerance, Vector3.up);
			if (rayCastHits.Length < 1)
				return;

			try
			{
				// Try to interact with something
				var interactableHit = rayCastHits.First(h => h.collider.tag == TAGS.INTERACTABLE_OBJECT_TAG);
				var interactable = interactableHit.collider.gameObject.GetComponent<IInteractable>();
				interactable.Interact(this);
			}
			catch { }
		}

		private void GrabObject(GameObject grabbedObject)
		{
			GrabbedObject = grabbedObject.GetComponent<WasteObject>();
			GrabbedObject.rigidbody.detectCollisions = false;
			GrabbedObject.rigidbody.velocity = Vector3.zero;
			GrabbedObject.rigidbody.angularVelocity = Vector3.zero;
			GrabbedObject.transform.rotation = Quaternion.identity;
			HoldPointCollider.enabled = true;
		}

		private WasteObject DropObject()
		{
			GrabbedObject.rigidbody.detectCollisions = true;
			HoldPointCollider.enabled = false;
			var obj = GrabbedObject;
			GrabbedObject = null;
			return obj;
		}

		private void StartThrowWindup()
		{
			WindingUp = true;
		}

		private void ThrowObject()
		{
			WindingUp = false;
			var grabbedObj = DropObject();
			grabbedObj.GetThrown(
				rigidbody.velocity * ThrowPlayerVelocityInfluence
				+ transform.forward
				* Mathf.Min(MaxThrowForce, (MinThrowForce + WindupDuration * ThrowForce))
				+ Vector3.up * ThrowUpForce);
			WindupDuration = 0;
		}

		public void SpawnAt(Transform spawnTransform)
		{
			transform.position = spawnTransform.position;
			transform.rotation = Quaternion.LookRotation(Vector3.up, -transform.position);
			rigidbody.velocity = transform.up * SpawnVelocity;
			remainingSpawnDuration = SpawnDuration;
		}
	}
}