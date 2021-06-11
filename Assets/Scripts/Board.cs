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

    [SerializeField]
    private GameObject pcursor1, pcursor2;

    private GameObject cursor;

    public List<List<Tile>> board = new List<List<Tile>>();
    private List<Vector2Int> available = new List<Vector2Int>();
    private int width;
    private int height;
    private int nMines;
    private bool player;

    public bool powerup = true;
    public List<Power> powers = new List<Power>();
    private bool firstClick = true;
    public bool gameOver = false;
    public int lives = 3;

    private Vector2Int pos = new Vector2Int();

    private KeyCode up, down, left, right, power1, power2, power3, lclick, rclick;
    private Controller controller;

    public List<Vector2Int> tiles, flags;

    public void New(int width_, int height_, int nMines_, bool player_, Controller controller_)
    {
        width = width_;
        height = height_;
        nMines = nMines_;
        player = player_;
        controller = controller_;

        // No need to place mines before first click
        powers.Add(Power.ShowBoard);
        for (int i = 0; i < 2; i++) powers.Add(Power.RemoveFlags);
        for (int i = 0; i < 3; i++) powers.Add(Power.InvertControls);
        CreateBoard();

        if (player)
        {
            up = KeyCode.W;
            down = KeyCode.S;
            left = KeyCode.A;
            right = KeyCode.D;
            power1 = KeyCode.Alpha1;
            power2 = KeyCode.Alpha2;
            power3 = KeyCode.Alpha3;
            lclick = KeyCode.Q;
            rclick = KeyCode.E;

            cursor = Instantiate(pcursor1, new Vector3(-7, 3, 0), Quaternion.identity);
        }
        else
        {
            up = KeyCode.I;
            down = KeyCode.K;
            left = KeyCode.J;
            right = KeyCode.L;
            power1 = KeyCode.Alpha8;
            power2 = KeyCode.Alpha9;
            power3 = KeyCode.Alpha0;
            lclick = KeyCode.U;
            rclick = KeyCode.O;

            cursor = Instantiate(pcursor2, new Vector3(1, 3, 0), Quaternion.identity);
        }
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
            if (!board[x][y].shown)
            {
                lives--;
                Debug.Log("lives: " + lives);
                if (lives == 0)
                {
                    controller.GameOver(player);
                    RevealAll();
                }
                else board[x][y].Reveal();
            }
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
                controller.GameOver(!player);
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
        else if (p == Power.RemoveFlags) controller.PowerUp(player, p);
        else if (p == Power.InvertControls) controller.PowerUp(player, p);
    }

    public void RemoveFlags()
    {
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                if (board[i][j].flag)
                    board[i][j].ToggleFlag();               
    }

    public void InvertControls()
    {
        StartCoroutine(InvertControlsEnum());
    }

    IEnumerator InvertControlsEnum()
    {
        KeyCode htmp = left, htmp2 = right;
        left = right;
        right = htmp;

        KeyCode vtmp = up, vtmp2 = down;
        up = down;
        down = vtmp;

        yield return new WaitForSeconds(5);

        left = right;
        right = htmp2;
        up = down;
        down = vtmp2;
    }


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
        if (gameOver) return;

        if (Input.GetKeyDown(up) && pos.y != 0)
        {
            pos.y -= 1;
            cursor.transform.position = transform.position + new Vector3(pos.x, -pos.y, 0) * 0.4f;
        }

        else if (Input.GetKeyDown(down) && pos.y != height - 1)
        {
            pos.y += 1;
            cursor.transform.position = transform.position + new Vector3(pos.x, -pos.y, 0) * 0.4f;
        }

        else if (Input.GetKeyDown(left) && pos.x != 0)
        {
            pos.x -= 1;
            cursor.transform.position = transform.position + new Vector3(pos.x, -pos.y, 0) * 0.4f;
        }

        else if (Input.GetKeyDown(right) && pos.x != -width + 1)
        {
            pos.x += 1;
            cursor.transform.position = transform.position + new Vector3(pos.x, -pos.y, 0) * 0.4f;
        }

        else if (Input.GetKeyDown(lclick))
        {
            Click(pos.x, pos.y);
        }

        else if (Input.GetKeyDown(rclick))
        {
            board[pos.x][pos.y].ToggleFlag();
        }
        

        if (Input.GetKeyDown(power1))
        {
            Debug.Log("pressed power1");
            powerup = true;
            powers.Remove(Power.ShowBoard);
            PowerUp(Power.ShowBoard);
        }

        else if (Input.GetKeyDown(power2))
        {
            Debug.Log("pressed power2");
            powerup = true;
            powers.Remove(Power.RemoveFlags);
            PowerUp(Power.RemoveFlags);
        }

        else if (Input.GetKeyDown(power3))
        {
            Debug.Log("pressed power3");
            powerup = true;
            powers.Remove(Power.InvertControls);
            PowerUp(Power.InvertControls);
        }
    }
}