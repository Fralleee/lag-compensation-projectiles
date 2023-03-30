using FishNet;
using TMPro;
using UnityEngine;

namespace VersausSurvival
{
	public class PingStatus : MonoBehaviour
	{
		TextMeshProUGUI _pingText;
		void Awake()
		{
			_pingText = GetComponent<TextMeshProUGUI>();
		}

		void OnGUI()
		{
			long ping;
			var timeManager = InstanceFinder.TimeManager;
			if (timeManager == null)
			{
				ping = 0;
			}
			else
			{
				ping = timeManager.RoundTripTime;
				var deduction = (long)(timeManager.TickDelta * 1000d);

				if (InstanceFinder.IsHost)
				{
					deduction *= 2;
				}
				ping = (long)Mathf.Max(0, ping - deduction) / 2;
			}

			_pingText.text = $"Ping: {ping}ms";
		}
	}
}
