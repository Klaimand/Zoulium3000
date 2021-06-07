using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] string firstScene;
    [SerializeField] GameObject loadingCanvas;

    public static GameManager Instance = null;

    Transform player;
    HashSet<KLD_PlayerController.PowerUp> playerPups = new HashSet<KLD_PlayerController.PowerUp>();

    string curScene;

    Vector3 lastCheckpointPosition;

    //life
    int maxHealth;
    int curHealth;

    [SerializeField] CanvasGroup[] groups;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //LoadFirstScene(firstScene);
        InitialMainMenuLoad(firstScene);
    }

    public void RespawnPlayer()
    {
        StartCoroutine(IRespawnPlayer());
    }

    IEnumerator IRespawnPlayer()
    {

        loadingCanvas.SetActive(true);

        yield return new WaitForSeconds(1f);

        //save player pups
        playerPups = player.GetComponent<KLD_PlayerController>().curPowerUps;

        Vector2Int h = player.GetComponent<KLD_PlayerHealth>().GetHealth();
        maxHealth = h.x;
        curHealth = h.y;

        //unload scene
        SceneManager.UnloadSceneAsync(curScene);

        //choose intact scene
        //reload scene
        AsyncOperation op = SceneManager.LoadSceneAsync(curScene, LoadSceneMode.Additive);

        while (!op.isDone)
        {
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(curScene));

        //get player
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //give player pups
        player.GetComponent<KLD_PlayerController>().curPowerUps = playerPups;
        //change player pos
        //__not needed
        KLD_PlayerHealth playerHealth = player.GetComponent<KLD_PlayerHealth>();
        playerHealth.SetHealth(maxHealth, curHealth);
        GameEvents.Instance.ChangeScene();

        yield return new WaitForSeconds(0.1f);

        loadingCanvas.SetActive(false);
    }

    public void LoadScene(string _scene)
    {
        StartCoroutine(ILoadScene(_scene));
    }

    IEnumerator ILoadScene(string _scene)
    {
        loadingCanvas.SetActive(true);

        yield return new WaitForSeconds(1f);

        //get player health
        Vector2Int h = player.GetComponent<KLD_PlayerHealth>().GetHealth();
        maxHealth = h.x;
        curHealth = h.y;
        //unload scene
        AsyncOperation p1 = SceneManager.UnloadSceneAsync(curScene);

        while (!p1.isDone)
        {
            yield return new WaitForSeconds(0.5f);
        }

        //choose good scene (argument)
        curScene = _scene;
        //load scene
        AsyncOperation p2 = SceneManager.LoadSceneAsync(curScene, LoadSceneMode.Additive);

        while (!p2.isDone)
        {
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(curScene));

        //update player life (max and cur health)
        player = GameObject.FindGameObjectWithTag("Player").transform;
        KLD_PlayerHealth playerHealth = player.GetComponent<KLD_PlayerHealth>();
        playerHealth.SetHealth(maxHealth, curHealth);
        GameEvents.Instance.ChangeScene();

        yield return new WaitForSeconds(0.1f);

        loadingCanvas.SetActive(false);
    }



    public void LoadSceneFromMainMenu(string _scene)
    {
        StartCoroutine(ILoadSceneFromMainMenu(_scene));
    }

    IEnumerator ILoadSceneFromMainMenu(string _scene)
    {
        loadingCanvas.SetActive(true);
        HideCanvases();

        yield return new WaitForSeconds(1f);

        //unload scene
        AsyncOperation p1 = SceneManager.UnloadSceneAsync(curScene);

        while (!p1.isDone)
        {
            yield return new WaitForSeconds(0.5f);
        }

        //choose good scene (argument)
        curScene = _scene;
        //load scene
        AsyncOperation op = SceneManager.LoadSceneAsync(curScene, LoadSceneMode.Additive);

        while (!op.isDone)
        {
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(curScene));

        //update player life (max and cur health)
        player = GameObject.FindGameObjectWithTag("Player").transform;
        KLD_PlayerHealth playerHealth = player.GetComponent<KLD_PlayerHealth>();
        GameEvents.Instance.ChangeScene();

        yield return new WaitForSeconds(0.1f);

        loadingCanvas.SetActive(false);
        ShowCanvases();
    }

    public void InitialMainMenuLoad(string _scene)
    {
        StartCoroutine(IInitialMainMenuLoad(_scene));
    }

    IEnumerator IInitialMainMenuLoad(string _scene)
    {
        HideCanvases();
        loadingCanvas.SetActive(true);

        //yield return new WaitForSeconds(1f);

        //choose good scene (argument)
        curScene = _scene;
        //load scene
        AsyncOperation op = SceneManager.LoadSceneAsync(curScene, LoadSceneMode.Additive);

        while (!op.isDone)
        {
            yield return new WaitForSeconds(0.5f);
        }

        //yield return new WaitForSeconds(1f);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(curScene));

        //update player life (max and cur health)

        //yield return new WaitForSeconds(0.1f);

        loadingCanvas.SetActive(false);
    }

    public void LoadMainMenu(string _scene)
    {
        StartCoroutine(ILoadMainMenu(_scene));
    }

    IEnumerator ILoadMainMenu(string _scene)
    {
        loadingCanvas.SetActive(true);


        //unload scene
        AsyncOperation p1 = SceneManager.UnloadSceneAsync(curScene);

        while (!p1.isDone)
        {
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);

        //choose good scene (argument)
        curScene = _scene;
        //load scene
        AsyncOperation p2 = SceneManager.LoadSceneAsync(curScene, LoadSceneMode.Additive);

        while (!p2.isDone)
        {
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1f);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(curScene));

        yield return new WaitForSeconds(0.1f);

        loadingCanvas.SetActive(false);
    }

    public void HideCanvases()
    {
        foreach (var group in groups)
        {
            group.alpha = 0f;
        }
    }

    public void ShowCanvases()
    {
        foreach (var group in groups)
        {
            group.alpha = 1f;
        }
    }
}
