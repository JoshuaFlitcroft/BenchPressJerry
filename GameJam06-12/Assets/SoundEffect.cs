using UnityEngine;
using System.Collections;

public class SoundEffect : MonoBehaviour
{
	AudioSource audioComponent;

	// Use this for initialization
	void Start ()
	{
		audioComponent = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!audioComponent.isPlaying && !audioComponent.loop)
		{
			Destroy(this.gameObject);
		}
	}
}
