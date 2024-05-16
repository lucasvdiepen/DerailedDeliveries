using System.Collections.Generic;
using UnityEngine;

namespace FishNet.Discovery
{
	public sealed class NetworkDiscoveryHud : MonoBehaviour
	{
		[SerializeField]
		private NetworkDiscovery networkDiscovery;

		private readonly HashSet<string> _addresses = new HashSet<string>();

		private Vector2 _serversListScrollVector;

		private void Start()
		{
			if (networkDiscovery == null) networkDiscovery = FindObjectOfType<NetworkDiscovery>();

			networkDiscovery.ServerFoundCallback += endPoint => _addresses.Add(endPoint.Address.ToString());
		}

		private void OnGUI()
		{
			GUILayoutOption buttonHeight = GUILayout.Height(30.0f);

			GUILayout.BeginArea(new Rect(Screen.width - 240.0f - 10.0f, 10.0f, 240.0f, Screen.height - 20.0f));

			GUILayout.Box("Server");

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Start", buttonHeight)) InstanceFinder.ServerManager.StartConnection();

			if (GUILayout.Button("Stop", buttonHeight)) InstanceFinder.ServerManager.StopConnection(true);

			GUILayout.EndHorizontal();

			GUILayout.Box("Advertising");

			GUILayout.BeginHorizontal();

			if (networkDiscovery.IsAdvertising)
			{
				if (GUILayout.Button("Stop", buttonHeight)) networkDiscovery.StopSearchingOrAdvertising();
			}
			else
			{
				if (GUILayout.Button("Start", buttonHeight)) networkDiscovery.AdvertiseServer();
			}

			GUILayout.EndHorizontal();

			GUILayout.Box("Searching");

			GUILayout.BeginHorizontal();

			if (networkDiscovery.IsSearching)
			{
				if (GUILayout.Button("Stop", buttonHeight)) networkDiscovery.StopSearchingOrAdvertising();
			}
			else
			{
				if (GUILayout.Button("Start", buttonHeight)) networkDiscovery.SearchForServers();
			}

			GUILayout.EndHorizontal();

			if (_addresses.Count < 1)
			{
				GUILayout.EndArea();

				return;
			}

			GUILayout.Box("Servers");

			_serversListScrollVector = GUILayout.BeginScrollView(_serversListScrollVector);

			foreach (string address in _addresses)
			{
				if (GUILayout.Button(address))
				{
					networkDiscovery.StopSearchingOrAdvertising();

					InstanceFinder.ClientManager.StartConnection(address);
				}
			}

			GUILayout.EndScrollView();

			GUILayout.EndArea();
		}
	}
}
