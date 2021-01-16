using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public GameObject imgBox;
    private Image img;
    bool playFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        img = imgBox.GetComponentInChildren<Image>();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeInOutCoroutine()
    {
        playFlag = true;

        Color col = img.color;

        while(col.a <= 1)
        {
            col.a += 0.02f;
            img.color = col;
            yield return new WaitForSeconds(0.02f);
        }
        while(col.a >= 0 )
        {
            col.a -= 0.02f;
            img.color = col;
            yield return new WaitForSeconds(0.02f);
        }
        playFlag = false;
        imgBox.SetActive(false);
        yield return null;
    }

    IEnumerator FadeOut()
    {
        imgBox.SetActive(true);
        Color col = img.color;
        
        while (col.a >= 0)
        {
            col.a -= 0.01f;
            img.color = col;
            yield return new WaitForSeconds(0.01f);
        }

        imgBox.SetActive(false);

        yield return null;
    }
}
