using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Controller : MonoBehaviour
{
    [SerializeField]
    GameObject board;

    private Board b1, b2;
    public string message;

    // Start is called before the first frame update
    void Start()
    {
        b1 = Instantiate(board, new Vector3(-7, 3, 0), Quaternion.identity).GetComponent<Board>();
        b1.New(16, 16, 40, true, this);

        b2 = Instantiate(board, new Vector3(1, 3, 0), Quaternion.identity).GetComponent<Board>();
        b2.New(16, 16, 40, false, this);

        Debug.Log(SceneManager.GetActiveScene().name);
    }

    public void GameOver(bool player)
    {
        b1.gameOver = true;
        b2.gameOver = true;

        if (!player) message = "Player 1 wins!";
        else message = "Player 2 wins!";
        Debug.Log(message);
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
}
