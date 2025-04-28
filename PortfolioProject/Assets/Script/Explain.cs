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
    string[] explainStr = new string[3] {"�ش� ������\n�̷θ� Ż���ϴ�\n�����Դϴ�"
   ,"ȭ�鿡 ���̴� ���� ��տ� �����ϸ� ������ ����˴ϴ�","���� ����� wasd Ȥ�� ����Ű�� �̿��Ͽ� ĳ���͸� �����̰�, ���콺 ���⿡ ���� ȭ���� ȸ���մϴ�. "};


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


        // ��ǥ ��ġ (ȭ�� �߾�)
        targetPosition = Vector2.zero;

        // ���� ��ġ (ȭ�� �Ʒ��� ������)
        Vector2 startPosition = new Vector2(0, -Screen.height);
        ExplainPopup.anchoredPosition = startPosition;

        // �ڷ�ƾ ����
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
        float duration = 1.0f; // �� �̵� �ð�
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

        // ������ ��ġ ����
        ExplainPopup.anchoredPosition = targetPosition;
    }

    IEnumerator MoveDown()
    {
        float duration = 1.0f; // �� �̵� �ð�
        float elapsed = 0f;

        // ���� ��ġ: ȭ�� �߾�
        Vector2 startingPos = Vector2.zero;
        ExplainPopup.anchoredPosition = startingPos;

        // ��ǥ ��ġ: ȭ�� �Ʒ��� �ٱ�
        Vector2 endPos = new Vector2(0, -Screen.height);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // �ε巴�� �����ϴ� ��¡ ó�� (EaseOut)
            t = 1f - Mathf.Pow(1f - t, 3);

            ExplainPopup.anchoredPosition = Vector2.Lerp(startingPos, endPos, t);

            yield return null;
        }

        // ������ ��ġ ����
        ExplainPopup.anchoredPosition = endPos;

        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);

        mazeGenerator.GameStart();
    }
}
