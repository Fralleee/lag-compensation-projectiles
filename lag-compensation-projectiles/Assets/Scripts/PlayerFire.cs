using System.Collections;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace LagCompensationProjectiles
{
	public class PlayerFire : NetworkBehaviour
	{
		const float MaxPassedTime = 0.4f;

		int nextProjectileId = 0;
		int GetNextProjectileId => nextProjectileId++;

		[Tooltip("Projectile to spawn."), SerializeField]
		Projectile _projectile;

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
			var projectileId = GetNextProjectileId;
			var position = origin.position;
			var direction = origin.forward;

			var ping = GetPing();
			var delay = (ping / 1000f) - MaxPassedTime;
			if (delay > 0)
			{
				StartCoroutine(DelayedSpawnProjectile(projectileId, position, direction, delay));
			}
			else
			{
				SpawnProjectile(projectileId, position, direction, 0f, this);
			}

			ServerFire(projectileId, position, direction, TimeManager.Tick, IsServer); // Original
		}

		IEnumerator DelayedSpawnProjectile(int projectileId, Vector3 position, Vector3 direction, float delay)
		{
			yield return new WaitForSeconds(delay);

			SpawnProjectile(projectileId, position, direction, 0f, this);
		}

		void SpawnProjectile(int projectileId, Vector3 position, Vector3 direction, float passedTime, PlayerFire owner = null)
		{
			var pp = Instantiate(_projectile, position, Quaternion.identity);
			pp.Initialize(projectileId, direction, passedTime, owner);

			CombatManager.Instance.RegisterProjectile(projectileId, pp);
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

		[ServerRpc(RequireOwnership = false)]
		void ServerFire(int projectileId, Vector3 position, Vector3 direction, uint tick, bool alreadyFired)
		{
			//if (!alreadyFired)
			//{
			//	var passedTime = (float)TimeManager.TimePassed(tick);
			//	passedTime = Mathf.Min(MaxPassedTime / 2f, passedTime);
			//	SpawnProjectile(projectileId, position, direction, passedTime);
			//}
			ObserversFire(projectileId, position, direction, tick);
		}

		[ObserversRpc(ExcludeOwner = true)]
		void ObserversFire(int projectileId, Vector3 position, Vector3 direction, uint tick)
		{
			var passedTime = (float)TimeManager.TimePassed(tick);
			passedTime = Mathf.Min(MaxPassedTime, passedTime);

			SpawnProjectile(projectileId, position, direction, passedTime);
		}

		[TargetRpc]
		public void SuccessfulHit(NetworkConnection networkConnection)
		{
			Debug.Log("WOOOHOOO HIT!");
		}
	}
}
