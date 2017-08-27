using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

[RequireComponent(typeof(PlayableDirector))]
public class UIManager : MonoBehaviour 
{
	[SerializeField]
	private PlayableAsset fadeIn, fadeOut;

	[SerializeField]
	private Text messageText;

	[SerializeField]
	private Text timer;

	private PlayableDirector timeline;

	private static UIManager instance;

	private void Awake()
	{
		if (instance != null) 
		{
			Destroy (this);
			return;
		}

		instance = this;

		timeline = GetComponent<PlayableDirector> ();
	}

	private void OnDestroy()
	{
		if (instance == this) 
		{
			instance = null;
		}
	}

	public static void ShowMessage(string message, float fadeOutDelay)
	{
		instance.CancelInvoke ();

		instance.timeline.playableAsset = instance.fadeIn;
		instance.timeline.Play ();

		instance.messageText.text = message;

		instance.Invoke ("FadeOutMessage", (float)instance.fadeIn.duration + fadeOutDelay);
	}

	private void FadeOutMessage()
	{
		Debug.Log ("Invoked");

		timeline.playableAsset = fadeOut;
		timeline.Play ();
	}

	public static void UpdateTimer(float time, bool visible = true)
	{
		instance.timer.text = time.ToString ("00");

		instance.timer.gameObject.SetActive (visible);
	}
}
