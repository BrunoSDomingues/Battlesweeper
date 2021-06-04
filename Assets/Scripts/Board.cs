using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ShowBoard cannot be called before first click, need to check
public enum Power
{
    ShowBoard = 1,
    RemoveFlags = 2,
    InvertControls = 3
};

public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject TileObject;

    public List<List<Tile>> board = new List<List<Tile>>();
    private List<Vector2Int> available = new List<Vector2Int>();
    private int width;
    private int height;
    private int nMines;

    public bool powerup = true;
    public List<Power> powers = new List<Power>();
    private bool firstClick = true;
    private bool gameOver = false;
    public int lives = 3;


    public List<Vector2Int> tiles, flags;

    public void New(int width_, int height_, int nMines_)
    {
        width = width_;
        height = height_;
        nMines = nMines_;

        // No need to place mines before first click
        powers.Add(Power.ShowBoard);
        for (int i = 0; i < 2; i++) powers.Add(Power.RemoveFlags);
        for (int i = 0; i < 3; i++) powers.Add(Power.InvertControls);
        CreateBoard();
    }

    private void SetMines(int x, int y)
    {
        var random = new System.Random();
        while (true)
        {
            // Shuffle available
            // https://www.codegrepper.com/code-examples/csharp/c%23+how+to+shuffle+a+list
            for (int i = available.Count - 1; i > 0; i--)
            {
                int idx = random.Next(i + 1);
                var temp = available[i];
                available[i] = available[idx];
                available[idx] = temp;
            }

            for (int i = 0; i < nMines; i++)
            {
                var pos = available[i];
                board[pos[0]][pos[1]].SetMine();
            }

            if (board[x][y].minesNear == 0)
            {
                break;
            }

            for (int i = 0; i < nMines; i++)
            {
                var pos = available[i];
                board[pos[0]][pos[1]].UnsetMine();
            }
        }
    }

    public void CreateBoard()
    {
        board.Clear();
        available.Clear();
        firstClick = true;

        // Instantiate and add tiles
        for (int i = 0; i < height; i++)
        {
            board.Add(new List<Tile>());
            for (int j = 0; j < width; j++)
            {
                GameObject obj = Instantiate(TileObject, transform);
                Tile t = obj.GetComponent<Tile>();
                t.New(this, i, j);
                // TODO set position

                board[i].Add(t);
                available.Add(new Vector2Int(i, j));
            }
        }
    }

    public void Click(int x, int y)
    {
        if (gameOver) return;
        if (firstClick)
        {
            SetMines(x, y);
            firstClick = false;
        }
        if (board[x][y].mine)
        {
            lives--;
            Debug.Log("lives: " + lives);
            if (lives == 0)
            {
                gameOver = true;
                RevealAll();
            }
            else board[x][y].Reveal();
        }
        else
        {
            board[x][y].Reveal();
            bool cond = true;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (!board[i][j].mine && !board[i][j].shown) cond = false;
            if (cond)
            {
                gameOver = true;
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        if (board[i][j].mine && !board[i][j].flag) board[i][j].ToggleFlag();
            }
        }
    }

    public void RevealAll()
    {
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                board[i][j].Reveal();
    }

    public void PowerUp(Power p)
    {
        if (!powerup) return;
        else if (p == Power.ShowBoard) StartCoroutine(QuickShow());
        else if (p == Power.RemoveFlags) RemoveFlags();
        // else if (p == Power.InvertControls) StartCoroutine(InvertControls());
    }

    public void RemoveFlags()
    {
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                if (board[i][j].flag)
                    board[i][j].ToggleFlag();               
    }

    //IEnumerator InvertControls()
    //{
    //}


    IEnumerator QuickShow()
    {
        tiles = new List<Vector2Int>();
        flags = new List<Vector2Int>();
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                if (board[i][j].shown)
                {
                    tiles.Add(new Vector2Int(i, j));
                }
                else if (board[i][j].flag)
                {
                    flags.Add(new Vector2Int(i, j));
                    board[i][j].flag = false;
                }

        Debug.Log("revelou");
        RevealAll();
        Debug.Log("ativou");
        yield return new WaitForSeconds(2);
        Debug.Log("acabou");

        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                if (flags.Contains(new Vector2Int(i, j)))
                {
                    board[i][j].flag = true;
                    board[i][j].Hide();
                }
                else if (!tiles.Contains(new Vector2Int(i, j)))
                {
                    board[i][j].Hide();
                }

        powerup = false;
    }

    public bool Contains(int i, int j) => (i < height && j < width);

    public bool Contains(Vector2Int pos) => Contains(pos.x, pos.y);

    private void Start()
    {
        // New(16, 30, 99);
        // New(16, 30, 10);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            Debug.Log("pressed 1");
            powerup = true;
            powers.Remove(Power.ShowBoard);
            PowerUp(Power.ShowBoard);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            Debug.Log("pressed 2");
            powerup = true;
            powers.Remove(Power.RemoveFlags);
            PowerUp(Power.RemoveFlags);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            powerup = true;
            powers.Remove(Power.InvertControls);
            PowerUp(Power.InvertControls);
        }
    }
}