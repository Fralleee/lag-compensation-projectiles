using UnityEngine;

namespace LagCompensationProjectiles
{
	public class Health : MonoBehaviour
	{
		public int MaxHealth = 100;
		public int CurrentHealth;

		void OnValidate()
		{
			CurrentHealth = MaxHealth;
		}

		public void TakeDamage(int damage)
		{
			CurrentHealth -= damage;
			if (CurrentHealth < 0)
			{
				CurrentHealth = MaxHealth;
			}
		}
	}
}
