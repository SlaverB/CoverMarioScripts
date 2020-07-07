using CameraCont;
using Components.Popups;
using Level;
using Runner;
using System;
using Units.Hero;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _back;
    [SerializeField] private GameObject _menuWindow;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private InfinityTrackBuilder _infinityTrackBuilder;
    [SerializeField] private InfinityTrackBuilder _startPlatform;
    [SerializeField] private UIController _uiController;
    [SerializeField] private Button _chooseLevelButton;
    [SerializeField] private Button _exitGameButton;


    private readonly string _menuPath = "Prefabs/Menu";
    private readonly string _gameoverPopupPath = "Prefabs/Popups/GameOverPopup";
    private readonly string _completeLevelPopupPath = "Prefabs/Popups/CompleteLevelPopUp";
    private readonly string _chooseLevelPopUpPath = "Prefabs/Popups/LevelListPopUp";
    private readonly string _heroPrefabPath = "Prefabs/Hero/Hero";
    private HeroView _hero;
    private GameObject _chooseLevelPopUp;

    public static Action StartBuildLevel;

    public HeroView GetHero => _hero;

    private void Start()
    {
        HeroController.HeroDie += ShowGameOverPopup;
        LevelTrackBuilder.LevelCompleted += CompleteLevel;
        LevelTrackBuilder.ClickStartLevel += StartLevel;
    }

    private void OnEnable()
    {
        _chooseLevelButton.onClick.AddListener(ClickOnChosseLevelBut);
        _exitGameButton.onClick.AddListener(ExitGame);
    }

    private void CreateHero()
    {
        PlayerPrefs.DeleteKey("NumberOfCoins");
        _hero = Instantiate(Resources.Load(_heroPrefabPath, typeof(HeroView)), _startPlatform.transform) as HeroView;
        _hero.transform.localScale = Vector3.one;
        _hero.gameObject.SetActive(true);
        _cameraController.SetTarget(_hero.transform);
        _infinityTrackBuilder.SetHero(_hero);
        _uiController.GetHero = _hero;
    }

    private void ClickOnChosseLevelBut()
    {
        _chooseLevelPopUp = Instantiate(Resources.Load(_chooseLevelPopUpPath, typeof(GameObject)), _back.transform) as GameObject;

        if (_chooseLevelPopUp.GetComponent<ChooseLevelPopUp>().GetLevelNumber % 50 == 0)
        {
            _chooseLevelPopUp.GetComponent<ChooseLevelPopUp>().CreateListOfLevel();
        }

    }

    public void ShowGameOverPopup()
    {
        _cameraController.StopTracking();
        _infinityTrackBuilder.RestartGame();
        GameObject _gameoverPopup = Instantiate(Resources.Load(_gameoverPopupPath, typeof(GameObject)), _back.transform) as GameObject;
        _gameoverPopup.transform.localScale = Vector3.one;
        //_gameoverPopup.transform.GetComponent<GameOverPopUp>().OnRestart += RestartGame;
        GameOverPopUp.OnRestart += RestartGame;
    }

    private void CompleteLevel()
    {
        _cameraController.StopTracking();
        _hero.Stop();
        GameObject _completeLevelPopup = Instantiate(Resources.Load(_completeLevelPopupPath, typeof(GameObject)), _back.transform) as GameObject;
        _completeLevelPopup.transform.localScale = Vector3.one;
        //_completeLevelPopup.transform.GetComponent<CompleteLevelPopUp>().OnRestart += RestartGame;
        CompleteLevelPopUp.OnRestart += RestartGame;
    }


    public void StartArcade()// OnInspector
    {
        _menuWindow.SetActive(false);
        CreateHero();
        _startPlatform.CreateRoad();
    }

    public void StartLevel()
    {
        //_uiController.StartLevel();
        _menuWindow.SetActive(false);
        //_chooseLevelPopUp.SetActive(false);
        CreateHero();
        StartBuildLevel?.Invoke();
    }

    public void RestartGame()
    {
        Destroy(_hero.gameObject);
        _menuWindow.SetActive(true);
    }
    private void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }
}
