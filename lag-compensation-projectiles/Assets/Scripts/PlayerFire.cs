using System.Collections;
using FishNet.Managing.Timing;
using FishNet.Object;
using UnityEngine;

namespace VersausSurvival
{
	public class PlayerFire : NetworkBehaviour
	{
		const float MAX_PASSED_TIME = 0.4f;

		[Tooltip("Projectile to spawn."), SerializeField]
		PredictedProjectile _projectile;

		[Tooltip("Debug projectile to spawn on server."), SerializeField]
		bool _useLatencySpawnDelay;



		public override void OnStartClient()
		{
			base.OnStartClient();
			if (!IsOwner)
			{
				enabled = false;
			}
		}

		void Update()
		{
			if (Input.GetButtonDown("Fire1"))
			{
				ClientFire();
			}
		}

		void ClientFire()
		{
			var origin = Camera.main.transform;
			var position = origin.position;
			var direction = origin.forward;

			var ping = GetPing();
			var delay = (ping / 1000f) - MAX_PASSED_TIME;
			if (delay > 0)
			{
				StartCoroutine(DelayedSpawnProjectile(position, direction, delay));
			}
			else
			{
				SpawnProjectile(position, direction, 0f);
			}

			ServerFire(position, direction, TimeManager.Tick, IsServer); // Original
		}

		IEnumerator DelayedSpawnProjectile(Vector3 position, Vector3 direction, float delay)
		{
			yield return new WaitForSeconds(delay);

			SpawnProjectile(position, direction, 0f);
		}

		void SpawnProjectile(Vector3 position, Vector3 direction, float passedTime)
		{
			var pp = Instantiate(_projectile, position, Quaternion.identity);
			pp.Initialize(direction, passedTime);
		}

		long GetPing()
		{
			var ping = TimeManager.RoundTripTime;
			var deduction = (long)(TimeManager.TickDelta * 1000d);
			if (IsHost)
			{
				deduction *= 2;
			}

			ping = (long)Mathf.Max(0, ping - deduction);
			return ping;
		}

		[ServerRpc]
		void ServerFire(Vector3 position, Vector3 direction, uint tick, bool alreadyFired)
		{
			if (!alreadyFired)
			{
				var passedTime = (float)TimeManager.TimePassed(tick);
				passedTime = Mathf.Min(MAX_PASSED_TIME / 2f, passedTime);
				SpawnProjectile(position, direction, passedTime);
			}
			ObserversFire(position, direction, tick);
		}

		[ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
		void ObserversFire(Vector3 position, Vector3 direction, uint tick)
		{
			var passedTime = (float)TimeManager.TimePassed(tick);
			passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

			SpawnProjectile(position, direction, passedTime);
		}
	}
}
