using System;
using System.Collections;
using Script.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject _menuCanvas;
    public GameObject menuPrefab;
    public Button settingsBtn;
    public GameObject pausePanel;
    public Slider volumeSlider;
    private void Awake()
    {
        settingsBtn.onClick.AddListener(TogglePausePanel);
        volumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
    }
    private void OnEnable()
    {
        EventSystem.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }
    private void OnDisable()
    {
        EventSystem.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        if (_menuCanvas.transform.childCount > 0)
        {
            Destroy(_menuCanvas.transform.GetChild(0).gameObject);
        }
    }

    private void Start()
    {
        _menuCanvas = GameObject.FindWithTag("MenuCanvas");
        Instantiate(menuPrefab, _menuCanvas.transform);
    }
    private void TogglePausePanel()
    {
        bool isOpen = pausePanel.activeInHierarchy;//判断当前GameObject是否在Hierarchy面板中是激活状态
        if (isOpen)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            GC.Collect();//在游戏暂停时强制进行垃圾回收,提高游戏性能
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    public void ReturnMenuCanvas()
    {
        Time.timeScale = 1;
        StartCoroutine(BackToMenu());
    }
    private IEnumerator BackToMenu()
    {
        pausePanel.SetActive(false);
        EventSystem.CallEndGameEvent();
        yield return new WaitForSeconds(1.0f);
        Instantiate(menuPrefab, _menuCanvas.transform);
    }
    public void GameEndEvent()
    {
        Application.Quit();
    }
}