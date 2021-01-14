using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Title;
    [SerializeField] [Range(0f, 10f)]
    private float speed = 1f;
    [SerializeField] [Range(0f, 10f)]
    private float length = 1f;

    private float runningTime = 0f;
    private float yPos = 0f;
    private float xPos;
    // Start is called before the first frame update
    void Start()
    {
        xPos = Title.transform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        TitleMovement();
    }

    public void ButtonNewGame()
    {
        LoadingSceneManager.LoadScene(1);
    }

    public void ButtonLoadGame()
    {

    }

    public void ButtonOption()
    {

    }

    public void ButtonExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit() // 어플리케이션 종료
#endif
    }

    public void TitleMovement()
    {
        runningTime += Time.deltaTime * speed;
        yPos = Mathf.Sin(runningTime) * length;
        Title.transform.localPosition = new Vector2(xPos, yPos); ;
    }
}
