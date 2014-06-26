using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuGUI : MonoBehaviour
{
    [SerializeField]
    private Transform background;
    [SerializeField]
    private TextMesh text;

    private static MenuGUI instance;

    public static MenuGUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (MenuGUI)FindObjectOfType(typeof(MenuGUI));

                if (FindObjectsOfType(typeof(MenuGUI)).Length > 1)
                {
                    Debug.LogError("[Singleton] Something went really wrong " +
                        " - there should never be more than 1 singleton!" +
                        " Reopenning the scene might fix it.");
                    return instance;
                }
                GameObject menuGO = new GameObject("MenuGUI");
                MenuGUI menu = menuGO.AddComponent(typeof(MenuGUI)) as MenuGUI;
                instance = menu;
            }
            return instance;
        }
    }

    private List<int> scenesToPlay = new List<int>();
    private List<int> scenesPlayed = new List<int>();

    private bool start;

    // Use this for initialization
    void Awake()
    {
        if (FindObjectsOfType(typeof(MenuGUI)).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        start = true;

        DontDestroyOnLoad(this);

        for(int i = 0; i < Application.levelCount-1; i++)
        {
            scenesToPlay.Add(i);
        }

        text.text = "Start";
    }

    void OnLevelWasLoaded(int level)
    {
        if(level == Application.levelCount-1)
        {
            start = true;
            collider.enabled = true;
            text.gameObject.SetActive(true);
            background.gameObject.SetActive(true);
        }
        else if (start)
        {
            start = false;
            collider.enabled = false;
            text.gameObject.SetActive(false);
            background.gameObject.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        transform.localScale = Vector3.one * 0.8f;
    }

    void OnMouseUp()
    {
        transform.localScale = Vector3.one;
        PlayNextRandomScene();
    }

    public void PlayNextRandomScene()
    {
        if (scenesToPlay.Count == 0)
        {
            foreach(int i in scenesPlayed)
            {
                scenesToPlay.Add(i);
            }
            scenesPlayed.Clear();
            Application.LoadLevel(Application.levelCount-1);
            collider.enabled = true;
            text.gameObject.SetActive(true);
            background.gameObject.SetActive(true);
        }
        else
        {
            int index = Random.Range(0, scenesToPlay.Count);
            int scene = scenesToPlay[index];
            scenesToPlay.RemoveAt(index);
            scenesPlayed.Add(scene);
            Application.LoadLevel(scene);
        }   
    }

}
