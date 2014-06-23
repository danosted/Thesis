using UnityEngine;
using System.Collections;

public class ExperimentSounds : MonoBehaviour {

    [SerializeField]
    private AudioClip goodHitClip;
    [SerializeField]
    private AudioClip badHitClip;
    [SerializeField]
    private AudioClip startClip;
    [SerializeField]
    private AudioClip endClip;

    private AudioSource source;
    
    private ExperimentSpawner spawner;
    
	// Use this for initialization
	void Awake () {
        spawner = GetComponent<ExperimentSpawner>();
        spawner.OnExperimentGoodTargetDisappear += OnGoodTargetHit;
        spawner.OnExperimentBadTargetDisappear += OnBadTargetHit;
        //spawner.OnExperimentStarted += OnExperimentStarted;
        spawner.OnExperimentEnded += OnExperimentEnded;
        source = GetComponent<AudioSource>();
	}

    private void OnGoodTargetHit(Transform target)
    {
        source.clip = goodHitClip;
        source.Play();
    }

    private void OnBadTargetHit(Transform target)
    {
        source.clip = badHitClip;
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
