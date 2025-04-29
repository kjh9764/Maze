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

    public float updateInterval = 0.5f;  // UI ���� �ֱ�
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
                // ������ ��Ƴ��� instance�� ������ �����ش�.
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
            // elapsedTime ����
            elapsedTime += updateInterval;

            // ������
            h = Mathf.FloorToInt(elapsedTime / 3600f);
            m = Mathf.FloorToInt((elapsedTime % 3600f) / 60f);
            s = Mathf.FloorToInt(elapsedTime % 60f);

            timerText.text = $"{h:00}:{m:00}:{s:00}";

            
            
            yield return new WaitForSeconds(updateInterval);
        }
        if (h != 00)
        {
            endTime = h + " �ð�";
        }
        if (m != 00)
        {
            endTime += m + " ��";
        }
        if (s != 00)
        {
            endTime += s + " ��";
        }
        if (!quitGame)
        {
            popupPanel.PopupBtnSet(1, () => StartCoroutine(LoadSceneAsync("Lobby")));
            popupPanel.Open("~Ż�� ����~\n\n�̷θ� " + endTime + "�� Ż���Ͽ����ϴ�.", "Ȯ��");
        }
       
    }

    public void StopTimer() => isRunning = false;


    //����ȯ
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
            popupPanel.Open("������ �����Ͻðڽ��ϱ�?", "��", "�ƴϿ�");
        }
        else
        {
            popupPanel.PopupBtnSet(0, () => StartCoroutine(LoadSceneAsync("Lobby")));
            popupPanel.PopupBtnSet(0, StopTimer);
            popupPanel.PopupBtnSet(0, QuitGame);
            popupPanel.Open("���� ȭ������ �̵��Ͻðڽ��ϱ�?", "��", "�ƴϿ�");
        }

    }

}