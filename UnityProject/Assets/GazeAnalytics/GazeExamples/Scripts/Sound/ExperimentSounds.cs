using UnityEngine;
using System.Collections;

public class ExperimentSounds : MonoBehaviour
{

    [SerializeField]
    private AudioClip goodHitClip;
    [SerializeField]
    private AudioClip badHitClip;
    [SerializeField]
    private AudioClip startClip;
    [SerializeField]
    private AudioClip endClip;
    [SerializeField]
    private AudioClip passiveSoundTrack;
    [SerializeField]
    private AudioClip activeSoundTrack;

    private AudioSource source;

    private ExperimentSpawner spawner;

    private AudioSource soundtrackSourceActive;
    private AudioSource soundtrackSourcePassive;

    // Use this for initialization
    void Awake()
    {
        spawner = GetComponent<ExperimentSpawner>();
        spawner.OnExperimentGoodTargetDisappear += OnGoodTargetHit;
        spawner.OnExperimentBadTargetDisappear += OnBadTargetHit;
        spawner.OnExperimentStarted += OnExperimentStarted;
        spawner.OnExperimentFailed += OnExperimentFailed;
        spawner.OnExperimentSucceeded += OnExperimentSucceeded;
        source = GetComponent<AudioSource>();

        //Create soundtrack gameobjects
        GameObject activeSource = new GameObject("activeSource");
        GameObject passiveSource = new GameObject("passiveSource");
        activeSource.transform.parent = transform;
        passiveSource.transform.parent = transform;
        soundtrackSourceActive = activeSource.AddComponent(typeof(AudioSource)) as AudioSource;
        soundtrackSourcePassive = passiveSource.AddComponent(typeof(AudioSource)) as AudioSource;
        soundtrackSourceActive.clip = activeSoundTrack;
        soundtrackSourcePassive.clip = passiveSoundTrack;
        soundtrackSourcePassive.loop = true;
        soundtrackSourcePassive.Play();
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
        StopCoroutine("FadeFromToSource");
        StartCoroutine(FadeFromToSource(soundtrackSourcePassive, soundtrackSourceActive));
        //source.clip = startClip;
        //source.Play();
    }

    private void OnExperimentFailed()
    {
        StopCoroutine("FadeFromToSource");
        StartCoroutine(FadeFromToSource(soundtrackSourceActive, soundtrackSourcePassive));
    }

    private void OnExperimentSucceeded()
    {
        source.clip = endClip;
        source.Play();
    }

    private IEnumerator FadeFromToSource(AudioSource from, AudioSource to)
    {
        float strength = 1f;
        to.volume = 0f;
        to.Stop();
        to.Play();
        to.loop = true;
        from.loop = false;
        while (from.volume > 0f || to.volume < 1f)
        {
            from.volume -= strength * Time.deltaTime;
            to.volume += strength * Time.deltaTime;
            yield return null;
        }
        from.Stop();
    }
}