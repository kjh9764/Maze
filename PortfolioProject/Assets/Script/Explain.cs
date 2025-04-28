using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Explain : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] RectTransform ExplainPopup;
    private Vector2 targetPosition;

    [SerializeField] Button left, right, confirm;
    [SerializeField] Text explainText;
    [SerializeField] Image explainImg;

    int progress = 0;

    [SerializeField] Sprite[] explainSprites;
    [SerializeField]
    string[] explainStr = new string[3] {"해당 게임은\n미로를 탈출하는\n게임입니다"
   ,"화면에 보이는 빨간 기둥에 도착하면 게임이 종료됩니다","조작 방법은 wasd 혹은 방향키를 이용하여 캐릭터를 움직이고, 마우스 방향에 따라 화면이 회전합니다. "};


    public void Start()
    {
        ExplainOn();
    }


    public void ExplainOn()
    {
        progress = 0;
        explainText.text = explainStr[progress];
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(true);
        confirm.gameObject.SetActive(false);

        gameObject.SetActive(true);


        // 목표 위치 (화면 중앙)
        targetPosition = Vector2.zero;

        // 시작 위치 (화면 아래쪽 밖으로)
        Vector2 startPosition = new Vector2(0, -Screen.height);
        ExplainPopup.anchoredPosition = startPosition;

        // 코루틴 시작
        StartCoroutine(MoveUp());

    }


    public void ExplainShow(bool LR)
    {

        if (LR)
        {
            progress++;
        }
        else
        {
            progress--;
        }


        if (progress == 0)
        {
            left.gameObject.SetActive(false);
        }
        else
        {
            left.gameObject.SetActive(true);
        }

        if (progress == explainStr.Length-1)
        {
            right.gameObject.SetActive(false);
            confirm.gameObject.SetActive(true);
        }
        else
        {
            right.gameObject.SetActive(true);
            confirm.gameObject.SetActive(false);
        }


        explainText.text = explainStr[progress];
        if (explainSprites[progress] == null)
        {
            explainImg.gameObject.SetActive(false);
        }
        else
        {
            explainImg.sprite = explainSprites[progress];
            explainImg.SetNativeSize();
            explainImg.gameObject.SetActive(true);
        }
    }

    public void ExplainClose()
    {
        StartCoroutine(MoveDown());
    }

    IEnumerator MoveUp()
    {
        float duration = 1.0f; // 총 이동 시간
        float elapsed = 0f;

        Vector2 startingPos = ExplainPopup.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = 1f - Mathf.Pow(1f - t, 3); 

            ExplainPopup.anchoredPosition = Vector2.Lerp(startingPos, targetPosition, t);

            yield return null;
        }

        // 마지막 위치 보정
        ExplainPopup.anchoredPosition = targetPosition;
    }

    IEnumerator MoveDown()
    {
        float duration = 1.0f; // 총 이동 시간
        float elapsed = 0f;

        // 시작 위치: 화면 중앙
        Vector2 startingPos = Vector2.zero;
        ExplainPopup.anchoredPosition = startingPos;

        // 목표 위치: 화면 아래쪽 바깥
        Vector2 endPos = new Vector2(0, -Screen.height);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 부드럽게 감속하는 이징 처리 (EaseOut)
            t = 1f - Mathf.Pow(1f - t, 3);

            ExplainPopup.anchoredPosition = Vector2.Lerp(startingPos, endPos, t);

            yield return null;
        }

        // 마지막 위치 보정
        ExplainPopup.anchoredPosition = endPos;

        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);

        mazeGenerator.GameStart();
    }
}
