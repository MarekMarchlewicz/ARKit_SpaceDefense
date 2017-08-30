using System;
using System.Collections.Generic;

namespace UnityEngine.XR.iOS
{
	public class UnityARGeneratePlane : MonoBehaviour
	{
		public GameObject planePrefab;
        private UnityARAnchorManager unityARAnchorManager;

		private void Start () 
		{
            unityARAnchorManager = new UnityARAnchorManager();
			UnityARUtility.InitializePlanePrefab (planePrefab);
		}

		private void OnDestroy()
        {
            unityARAnchorManager.Destroy ();
        }

		private void Update()
		{
			List<ARPlaneAnchorGameObject> arpags = unityARAnchorManager.GetCurrentPlaneAnchors ();
			if (arpags.Count >= 1) 
			{
				Debug.Log ("Detected planes: " + arpags.Count.ToString ());
			}
		}
	}
}

