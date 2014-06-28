using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuGUI : MonoBehaviour
{
    [SerializeField]
    private Transform background;
    [SerializeField]
    private TextMesh buttonText;
    [SerializeField]
    private Transform LoadingBar;

    private static MenuGUI _instance;

    private static object _lock = new object();

    public static MenuGUI Instance
    {
        get
		{
			if (applicationIsQuitting) {
				Debug.LogWarning("[Singleton] Instance '"+ typeof(MenuGUI) +
					"' already destroyed on application quit." +
					" Won't create again - returning null.");
				return null;
			}
 
			lock(_lock)
			{
				if (_instance == null)
				{
					_instance = (MenuGUI) FindObjectOfType(typeof(MenuGUI));
 
					if ( FindObjectsOfType(typeof(MenuGUI)).Length > 1 )
					{
						Debug.LogError("[Singleton] Something went really wrong " +
							" - there should never be more than 1 singleton!" +
							" Reopenning the scene might fix it.");
						return _instance;
					}
 
					if (_instance == null)
					{
						GameObject singleton = new GameObject();
						_instance = singleton.AddComponent<MenuGUI>();
						singleton.name = "(singleton) "+ typeof(MenuGUI).ToString();
 
						DontDestroyOnLoad(singleton);
 
						Debug.Log("[Singleton] An instance of " + typeof(MenuGUI) + 
							" is needed in the scene, so '" + singleton +
							"' was created with DontDestroyOnLoad.");
					} else {
						Debug.Log("[Singleton] Using instance already created: " +
							_instance.gameObject.name);
					}
				}
 
				return _instance;
			}
		}
	}
 
	private static bool applicationIsQuitting = false;
    private static string genericName = "Enter Name";

    private List<int> scenesToPlay = new List<int>();
    private List<int> scenesPlayed = new List<int>();

    private bool start;
    private bool listen;
    private bool marked;
    private string textToEdit;
    private GUIStyle textFieldStyle;
    private Vector3 backgroundPos;
    private GameObject nameTextGO;
    private TextMesh nameText;
    private Renderer loadingBarRender;

    // Use this for initialization
    void Awake()
    {
        if (FindObjectsOfType(typeof(MenuGUI)).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        start = true;
        listen = false;
        marked = false;

        DontDestroyOnLoad(this);

        for(int i = 1; i < Application.levelCount; i++)
        {
            scenesToPlay.Add(i);
        }

        buttonText.text = "Start";
        textToEdit = genericName;

        nameTextGO = Instantiate(buttonText.gameObject) as GameObject;
        nameTextGO.transform.position = transform.position + Vector3.up * transform.localScale.y;
        nameTextGO.collider.enabled = true;
        nameTextGO.transform.localScale = transform.localScale;
        nameTextGO.transform.parent = transform;
        nameText = nameTextGO.GetComponent<TextMesh>();
        nameText.text = textToEdit;
        nameText.color = Color.black;

        loadingBarRender = LoadingBar.GetComponent<Renderer>();
        loadingBarRender.gameObject.SetActive(false);
    }

    void Update()
    {
        if (start)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !listen)
            {
                Application.Quit();
            }
            if (Input.GetMouseButtonUp(0))
            {
                buttonText.color = Color.white;
            }
            if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
            {
                listen = !listen;
                marked = listen;
                if(listen)
                {
                    StartCoroutine("ListenForTextInput");
                    nameText.color = Color.blue;
                }
                else
                {
                    StopCoroutine("ListenForTextInput");
                    if(textToEdit.Length == 0)
                    {
                        textToEdit = genericName;
                        nameText.text = textToEdit;
                    }
                    nameText.color = Color.black;
                }
            }
            if (listen && !marked)
            {
                nameText.text = textToEdit;
            }
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
                    if(hit.transform == nameText.transform)
                    {
                        listen = !listen;
                        marked = listen;
                        if (listen)
                        {
                            StartCoroutine("ListenForTextInput");
                            nameText.color = Color.blue;
                        }
                        else
                        {
                            StopCoroutine("ListenForTextInput");
                            nameText.color = Color.black;
                        }
                    }
                }
            }
        }
    }

    void OnLevelWasLoaded(int level)
    {
        loadingBarRender.gameObject.SetActive(false);
        if(level == 0)
        {
            start = true;
            collider.enabled = true;
            nameTextGO.SetActive(true);
            buttonText.gameObject.SetActive(true);
            background.gameObject.SetActive(true);
            StartCoroutine("ListenForTextInput");
        }
        else if (start)
        {
            start = false;
            collider.enabled = false;
            nameTextGO.SetActive(false);
            buttonText.gameObject.SetActive(false);
            background.gameObject.SetActive(false);
        }
    }

    void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0))
        {
            buttonText.color = Color.red;   
        }
        if(Input.GetMouseButtonUp(0))
        {
            buttonText.color = Color.white;
            StopCoroutine("ListenForTextInput");
            PlayNextRandomScene();
        }
    }

    private void SwitchScene(int scene)
    {
        loadingBarRender.gameObject.SetActive(true);
        Application.LoadLevel(scene);
    }

    private IEnumerator ListenForTextInput()
    {
        while(listen)
        {
            if (Input.anyKeyDown && marked && textToEdit.Length > 0)
            {
                if (!Input.GetKeyDown(KeyCode.LeftShift) && !Input.GetKeyDown(KeyCode.RightShift) && !Input.GetMouseButtonDown(0))
                {
                    textToEdit = textToEdit.Remove(0);
                    marked = false;
                }
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                textToEdit = genericName;
            }
            if(Input.GetKeyDown(KeyCode.Backspace))
            {
                if (textToEdit.Length > 0)
                {
                    textToEdit = textToEdit.Remove(textToEdit.Length - 1);
                }
                yield return new WaitForSeconds(0.2f);
                while(Input.GetKey(KeyCode.Backspace))
                {
                    if (textToEdit.Length > 0)
                    {
                        textToEdit = textToEdit.Remove(textToEdit.Length - 1);
                    }
                    yield return new WaitForSeconds(0.1f);
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                textToEdit += " ";
            }
            #region uppercase;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if(Input.GetKeyDown(KeyCode.Q))
                {
                    textToEdit += "Q";
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    textToEdit += "W";
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    textToEdit += "E";
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    textToEdit += "R";
                }
                if (Input.GetKeyDown(KeyCode.T))
                {
                    textToEdit += "T";
                }
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    textToEdit += "Y";
                }
                if (Input.GetKeyDown(KeyCode.U))
                {
                    textToEdit += "U";
                }
                if (Input.GetKeyDown(KeyCode.I))
                {
                    textToEdit += "I";
                }
                if (Input.GetKeyDown(KeyCode.O))
                {
                    textToEdit += "O";
                }
                if (Input.GetKeyDown(KeyCode.P))
                {
                    textToEdit += "P";
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    textToEdit += "A";
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    textToEdit += "S";
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    textToEdit += "D";
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    textToEdit += "F";
                }
                if (Input.GetKeyDown(KeyCode.G))
                {
                    textToEdit += "G";
                }
                if (Input.GetKeyDown(KeyCode.H))
                {
                    textToEdit += "H";
                }
                if (Input.GetKeyDown(KeyCode.J))
                {
                    textToEdit += "J";
                }
                if (Input.GetKeyDown(KeyCode.K))
                {
                    textToEdit += "K";
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    textToEdit += "L";
                }
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    textToEdit += "Z";
                }
                if (Input.GetKeyDown(KeyCode.X))
                {
                    textToEdit += "X";
                }
                if (Input.GetKeyDown(KeyCode.C))
                {
                    textToEdit += "C";
                }
                if (Input.GetKeyDown(KeyCode.V))
                {
                    textToEdit += "V";
                }
                if (Input.GetKeyDown(KeyCode.B))
                {
                    textToEdit += "B";
                }
                if (Input.GetKeyDown(KeyCode.N))
                {
                    textToEdit += "N";
                }
                if (Input.GetKeyDown(KeyCode.M))
                {
                    textToEdit += "M";
                }
            }
            #endregion
            #region lowercase
            else
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    textToEdit += "q";
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    textToEdit += "w";
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    textToEdit += "e";
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    textToEdit += "r";
                }
                if (Input.GetKeyDown(KeyCode.T))
                {
                    textToEdit += "t";
                }
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    textToEdit += "y";
                }
                if (Input.GetKeyDown(KeyCode.U))
                {
                    textToEdit += "u";
                }
                if (Input.GetKeyDown(KeyCode.I))
                {
                    textToEdit += "i";
                }
                if (Input.GetKeyDown(KeyCode.O))
                {
                    textToEdit += "o";
                }
                if (Input.GetKeyDown(KeyCode.P))
                {
                    textToEdit += "p";
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    textToEdit += "a";
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    textToEdit += "s";
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    textToEdit += "d";
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    textToEdit += "f";
                }
                if (Input.GetKeyDown(KeyCode.G))
                {
                    textToEdit += "g";
                }
                if (Input.GetKeyDown(KeyCode.H))
                {
                    textToEdit += "h";
                }
                if (Input.GetKeyDown(KeyCode.J))
                {
                    textToEdit += "j";
                }
                if (Input.GetKeyDown(KeyCode.K))
                {
                    textToEdit += "k";
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    textToEdit += "l";
                }
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    textToEdit += "z";
                }
                if (Input.GetKeyDown(KeyCode.X))
                {
                    textToEdit += "x";
                }
                if (Input.GetKeyDown(KeyCode.C))
                {
                    textToEdit += "c";
                }
                if (Input.GetKeyDown(KeyCode.V))
                {
                    textToEdit += "v";
                }
                if (Input.GetKeyDown(KeyCode.B))
                {
                    textToEdit += "b";
                }
                if (Input.GetKeyDown(KeyCode.N))
                {
                    textToEdit += "n";
                }
                if (Input.GetKeyDown(KeyCode.M))
                {
                    textToEdit += "m";
                }
            }
            #endregion
            yield return null;
        }
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
            SwitchScene(0);
        }
        else
        {
            int index = Random.Range(0, scenesToPlay.Count);
            int scene = scenesToPlay[index];
            scenesToPlay.RemoveAt(index);
            scenesPlayed.Add(scene);
            SwitchScene(scene);
        }   
    }

    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    void OnApplicationQuit ()
    {
        applicationIsQuitting = true;
    }

    public string ParticipantName
    {
        get
        {
            return this.textToEdit;
        }
    }

}
