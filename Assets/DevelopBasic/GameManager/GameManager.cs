using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

using SimpleAudioSystem;
using SimpleSaveSystem;

//Please make sure "GameManager" is excuted before every custom script
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int targetFrameRate = 60;
[Header("Scene Transition")]
    [SerializeField] private CanvasGroup BlackScreenCanvasGroup;
    [SerializeField] private float transitionDuration = 1;
[Header("Init")]
    [SerializeField] private string InitScene;
[Header("Demo")]
    [SerializeField] private bool isDemo = true;
    [SerializeField] private bool isTesting = true;
    [SerializeField] private Text demoText;
[Header("Debug")]
    [SerializeField] private bool loadInitSceneFromGameManager = false;
    [SerializeField] private InputActionMap debugActions;

    private static bool isPaused = false;

    public bool IsSwitchingScene{get; private set;} = false;
    public string lastScene{get; private set;} = string.Empty;
    public string currentScene{get; private set;} = string.Empty;
    protected override void Awake(){
        base.Awake();
        Application.targetFrameRate = targetFrameRate;
        SaveManager.Initialize();

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        debugActions["restart"].performed += Debug_RestartLevel;
        debugActions["save"].performed += Debug_Save;
        debugActions["load"].performed += Debug_Load;

        if(isTesting) debugActions.Enable();
    #endif

    //To Do: Game Loading
    #if UNITY_EDITOR
    //Load Level
        if(loadInitSceneFromGameManager){
            BlackScreenCanvasGroup.alpha = 1;
            SwitchingScene(string.Empty, InitScene);
        }
        else {
            currentScene = SceneManager.GetActiveScene().name;
        }

    #else
    //Since we don't have the saving system yet, the initiation should be done by loading the debug progress data.
        SwitchingScene(string.Empty, InitScene);
    #endif
    }
    protected override void OnDestroy(){
        base.OnDestroy();

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        debugActions["restart"].performed -= Debug_RestartLevel;
        debugActions["save"].performed -= Debug_Save;
        debugActions["load"].performed -= Debug_Load;

        if(debugActions.enabled)debugActions.Disable();
    #endif
    }

#region GAME BASIC
    public void PauseTheGame(){
        if(isPaused) return;
        
        Time.timeScale = 0;
        AudioListener.pause = true;
        isPaused = true;
    }
    public void ResumeTheGame(){
        if(!isPaused) return;

        AudioListener.pause = false;
        Time.timeScale = 1;
        isPaused = false;
    }
    public void EndGame(){
        string currentLevel = SceneManager.GetActiveScene().name;
        StartCoroutine(EndGameCoroutine(currentLevel));
    }
    public void RestartLevel(){
        string currentLevel = SceneManager.GetActiveScene().name;
        StartCoroutine(RestartLevel(currentLevel));
    }
#endregion

#region Scene Transition
    public void SwitchingScene(string to, bool autosaveAfterTransition = true){
        string from = SceneManager.GetActiveScene().name;
        SwitchingScene(from, to, autosaveAfterTransition);
    }
    void SwitchingScene(string from, string to, bool autosaveAfterTransition = true){
        if(!IsSwitchingScene) StartCoroutine(SwitchSceneCoroutine(from, to, autosaveAfterTransition));
    }
    IEnumerator EndGameCoroutine(string level){
        StartCoroutine(FadeInBlackScreen(1f));
        StartCoroutine(new WaitForLoop(3f, (t)=>{
            AudioManager.Instance.ChangeMasterVolume(Mathf.Lerp(0, -80, EasingFunc.Easing.QuadEaseIn(t)));
        }));

        if(isDemo) {
            yield return new WaitForSeconds(1f);

            Color initCol = demoText.color;
            Color targetCol = initCol;
            initCol.a = 0;
            targetCol.a = 1;

            demoText.color = initCol;
            demoText.gameObject.SetActive(true);
            yield return new WaitForLoop(0.5f, (t)=>
                demoText.color = Color.Lerp(initCol, targetCol, EasingFunc.Easing.SmoothInOut(t))
            );
            yield return new WaitForSeconds(2f);
            yield return new WaitForLoop(0.5f, (t)=>
                demoText.color = Color.Lerp(targetCol, initCol, EasingFunc.Easing.SmoothInOut(t))
            );            
        }

        yield return new WaitForSeconds(1f);
        EventHandler.Call_BeforeUnloadScene();
        yield return SceneManager.UnloadSceneAsync(level);
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }
    IEnumerator RestartLevel(string level){
        yield return FadeInBlackScreen(3f);
        IsSwitchingScene = true;
        //TO DO: do something before the last scene is unloaded. e.g: call event of saving 
        EventHandler.Call_BeforeUnloadScene();

        yield return SceneManager.UnloadSceneAsync(level);
        yield return null;
        //TO DO: do something after the last scene is unloaded.
        yield return SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(level));
        //TO DO: do something after the next scene is loaded. e.g: call event of loading
        yield return null;
        yield return FadeOutBlackScreen(transitionDuration);
        EventHandler.Call_AfterLoadScene();
        
        IsSwitchingScene = false;
    }
    IEnumerator SwitchSceneCoroutine(string from, string to, bool autosaveAfterTransition){
        IsSwitchingScene = true;
        if(from != string.Empty){
        //TO DO: do something before the last scene is unloaded. e.g: call event of saving 
            lastScene = from;
            
            EventHandler.Call_BeforeUnloadScene();
            yield return FadeInBlackScreen(transitionDuration);
            yield return SceneManager.UnloadSceneAsync(from);
        }
    //TO DO: do something after the last scene is unloaded.
        yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(to));
        currentScene = to;

    //TO DO: do something after the next scene is loaded. e.g: call event of loading
        EventHandler.Call_AfterLoadScene();
    //AutoSave Game when transition to New Scene
        if(autosaveAfterTransition) SaveGame(0);

        yield return null;
        yield return FadeOutBlackScreen(transitionDuration);

        IsSwitchingScene = false;
    }
    public IEnumerator FadeInBlackScreen(float fadeDuration){
        float initAlpha = BlackScreenCanvasGroup.alpha;
        yield return new WaitForLoop(fadeDuration, (t)=>{
            BlackScreenCanvasGroup.alpha = Mathf.Lerp(initAlpha, 1, EasingFunc.Easing.QuadEaseOut(t));
        });
    }
    public IEnumerator FadeOutBlackScreen(float fadeDuration){
        float initAlpha = BlackScreenCanvasGroup.alpha;
        yield return new WaitForLoop(fadeDuration, (t)=>{
            BlackScreenCanvasGroup.alpha = Mathf.Lerp(initAlpha, 0, EasingFunc.Easing.QuadEaseIn(t));
        });
    }
#endregion

    public void LoadGame(int slotIndex)=>SaveManager.LoadGameState(slotIndex);
    public void SaveGame(int slotIndex)=>SaveManager.SaveGameState(slotIndex);

#region DEBUG ACTION
    void Debug_EndGame(InputAction.CallbackContext callback)=>EndGame();
    void Debug_RestartLevel(InputAction.CallbackContext callback){
        if(callback.ReadValueAsButton()){
            Debug.Log("Test Restart Level");
            RestartLevel();
        }
    }
    void Debug_Save(InputAction.CallbackContext callback)=>SaveManager.SaveGameState(0);
    void Debug_Load(InputAction.CallbackContext callback)=>SaveManager.LoadGameState(0);
#endregion
}
