using UnityEngine;
using System.Collections;

public class ExperimentSounds : MonoBehaviour {

    [SerializeField]
    private AudioClip hitclip;
    [SerializeField]
    private AudioClip startClip;
    [SerializeField]
    private AudioClip endClip;

    private AudioSource source;
    
    private ExperimentSpawner spawner;
    
	// Use this for initialization
	void Awake () {
        spawner = GetComponent<ExperimentSpawner>();
        spawner.OnExperimentTargethit += OnTargetHit;
        //spawner.OnExperimentStarted += OnExperimentStarted;
        spawner.OnExperimentEnded += OnExperimentEnded;
        source = GetComponent<AudioSource>();
	}
	
	private void OnTargetHit()
    {
        source.clip = hitclip;
        source.Play();
    }

    private void OnExperimentStarted()
    {
        source.clip = startClip;
        source.Play();
    }

    private void OnExperimentEnded()
    {
        source.clip = endClip;
        source.Play();
    }
}
