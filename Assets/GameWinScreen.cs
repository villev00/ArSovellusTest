using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWinScreen : MonoBehaviour
{
    public GameObject GameOverScreen;
    // Start is called before the first frame update
    public void GameOver()
    {
        GameOverScreen.SetActive(true);
    }
    
}
