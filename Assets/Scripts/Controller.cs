using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Controller : MonoBehaviour
{
    [SerializeField]
    GameObject board, timer;
    
    [HideInInspector]
    public Board b1, b2;

    [SerializeField]
    private Text p11, p12, p13, p21, p22, p23, nLives1, nLives2, timerText;

    private Timer t;

    [HideInInspector]
    public bool isTA;

    private string message;

    public AudioSource bomb, flag;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "TimeAttack")
        {
            isTA = true;
            t = Instantiate(timer, new Vector3(0, 4, 0), Quaternion.identity).GetComponent<Timer>();
            t.New(5f, timerText, this);
        }

        else isTA = false;

        b1 = Instantiate(board, new Vector3(-8, 3, 0), Quaternion.identity).GetComponent<Board>();
        b1.New(16, 16, 40, true, this, p11, p12, p13, nLives1);

        b2 = Instantiate(board, new Vector3(2, 3, 0), Quaternion.identity).GetComponent<Board>();
        b2.New(16, 16, 40, false, this, p21, p22, p23, nLives2);
        
    }

    public void GameOver(bool player, bool isTie)
    {
        b1.gameOver = true;
        b2.gameOver = true;

        if (isTA && isTie) message = "It's a tie!";
        else if (!player) message = "Player 1 wins!";
        else message = "Player 2 wins!";
        SceneManager.LoadScene("GameOver");
    }

    public void PowerUp(bool player, Power p)
    {
        if (p == Power.InvertControls)
        {
            if (!player) b1.InvertControls();
            else b2.InvertControls();
        }
        else if (p == Power.RemoveFlags)
        {
            if (!player) b1.RemoveFlags();
            else b2.RemoveFlags();
        }
        else Debug.Log("this should not exist");
    }

    private void OnDisable()
    {
        PlayerPrefs.SetString("gameovermsg", message);   
    }
}
