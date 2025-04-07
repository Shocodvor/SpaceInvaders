using System.Collections;
using UnityEngine;
using TMPro;

public class Points : MonoBehaviour
{
    TMP_Text pointText;
    [SerializeField] TMP_FontAsset[] fontAsset;
    [SerializeField] float delayPerColor;
    [SerializeField] int cycle;

    void Start()
    {
        pointText = GetComponentInChildren<TMP_Text>();
        StartCoroutine(TextAnimation());
    }

    IEnumerator TextAnimation()
    {
        for (int i = 0; i < cycle; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                pointText.font = fontAsset[j];
                yield return new WaitForSeconds(delayPerColor);
            }
        }
        Destroy(gameObject);
    }


}
