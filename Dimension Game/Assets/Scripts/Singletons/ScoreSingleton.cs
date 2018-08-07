using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSingleton : MonoBehaviour {

    public int maxGems = 10;
    public int collectedGems = 0;

    private Text m_gemCountText;

    private void Awake()
    {
        CheckSingleton();
        m_gemCountText = GameObject.Find("GemText").GetComponent<Text>();
    }

    private void Start()
    {
        CheckSingleton();
    }

    public void CheckSingleton()
    {
        if (Singletons.score == null)
        {
            Singletons.score = this;
        }
        else
        {
            if (Singletons.score != this)
            {
                Destroy(gameObject);
            }
        }
    }

    public void AddGem()
    {
        collectedGems++;
        m_gemCountText.text = collectedGems.ToString();
    }
}
