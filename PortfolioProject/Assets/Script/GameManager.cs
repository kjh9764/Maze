using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public int gameLevel = 0;
    [SerializeField] GameObject LoadingObj;

    public PopupPanel popupPanel;

    public float updateInterval = 0.5f;  // UI 갱신 주기
    int timer = 0;

    private float elapsedTime = 0f;
    private bool isRunning = true;
    private static GameManager instance;
    private bool isFirstInstance = true;
    private bool quitGame = false;

    string endTime;
    void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                // 이전에 살아남은 instance가 있으면 지워준다.
                Destroy(instance.gameObject);
            }
        }

        instance = this;

        if (isFirstInstance)
        {
            DontDestroyOnLoad(gameObject);
            isFirstInstance = false;
        }
    }


    public void LevelSelect(int value)
    {
        quitGame = false;
        gameLevel = value;
        StartCoroutine(LoadSceneAsync("Maze"));
    }

    public void Timer(Text texts)
    {
        endTime = "";
        StartCoroutine(UpdateTimerRoutine(texts));
    }

    IEnumerator UpdateTimerRoutine(Text timerText)
    {
        int h=0;
        int m=0;
        int s=0;

        while (isRunning)
        {
            // elapsedTime 누적
            elapsedTime += updateInterval;

            // 포맷팅
            h = Mathf.FloorToInt(elapsedTime / 3600f);
            m = Mathf.FloorToInt((elapsedTime % 3600f) / 60f);
            s = Mathf.FloorToInt(elapsedTime % 60f);

            timerText.text = $"{h:00}:{m:00}:{s:00}";

            
            
            yield return new WaitForSeconds(updateInterval);
        }
        if (h != 00)
        {
            endTime = h + " 시간";
        }
        if (m != 00)
        {
            endTime += m + " 분";
        }
        if (s != 00)
        {
            endTime += s + " 초";
        }
        if (!quitGame)
        {
            popupPanel.PopupBtnSet(1, () => StartCoroutine(LoadSceneAsync("Lobby")));
            popupPanel.Open("~탈출 성공~\n\n미로를 " + endTime + "에 탈출하였습니다.", "확인");
        }
       
    }

    public void StopTimer() => isRunning = false;


    //씬전환
    IEnumerator LoadSceneAsync(string sceneName)
    {
        LoadingObj.SetActive(true);

        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(sceneName);
        asyncOper.allowSceneActivation = false;
        while (asyncOper.progress < 0.9f)
            yield return null;

        yield return new WaitForSeconds(1f);
        asyncOper.allowSceneActivation = true;


        while (!asyncOper.isDone)
            yield return null;
        LoadingObj.SetActive(false);
    }

    public void GameEnd()
    {
        StopTimer();
       

    }

    public void QuitGame() => quitGame = true;

    public void Exit()
    {
        if (SceneManager.GetActiveScene().name.Contains("Lobby"))
        {
            popupPanel.PopupBtnSet(0, Application.Quit);
            popupPanel.Open("게임을 종료하시겠습니까?", "네", "아니오");
        }
        else
        {
            popupPanel.PopupBtnSet(0, () => StartCoroutine(LoadSceneAsync("Lobby")));
            popupPanel.PopupBtnSet(0, StopTimer);
            popupPanel.PopupBtnSet(0, QuitGame);
            popupPanel.Open("메인 화면으로 이동하시겠습니까?", "네", "아니오");
        }

    }

}