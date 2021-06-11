using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public int n1, n2, n3;

    public bool powerup = true;
    public List<Power> powers = new List<Power>();

    public Text p1, p2, p3, nLives;

    private bool firstClick = true;
    public bool gameOver = false;
    public int lives;
  
    private Vector2Int pos = new Vector2Int();

    private KeyCode up, down, left, right, power1, power2, power3, lclick, rclick;
    private Controller controller;

    public List<Vector2Int> tiles, flags;

    public void New(int width_, int height_, int nMines_, bool player_, Controller controller_, Text p1_, Text p2_, Text p3_, Text nLives_)
    {
        width = width_;
        height = height_;
        nMines = nMines_;
        player = player_;
        controller = controller_;

        p1 = p1_;
        p2 = p2_;
        p3 = p3_;
        nLives = nLives_;

        // No need to place mines before first click
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

            cursor = Instantiate(pcursor1, new Vector3(-8, 3, 0), Quaternion.identity);
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

            cursor = Instantiate(pcursor2, new Vector3(2, 3, 0), Quaternion.identity);
        }

        if (controller.isTA)
        {
            lives = 1;
            p1.enabled = false;
            p2.enabled = false;
            p3.enabled = false;
        }

        else
        {
            lives = 3;

            p1.enabled = true;
            p2.enabled = true;
            p3.enabled = true;

            powers.Add(Power.ShowBoard);
            n1 = 1;
            for (int i = 0; i < 2; i++) powers.Add(Power.RemoveFlags);
            n2 = 2;
            for (int i = 0; i < 3; i++) powers.Add(Power.InvertControls);
            n3 = 3;
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

    public int countShown()
    {
        int empty = 0;
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                if (board[i][j].shown) empty++;

        return empty;
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
                controller.bomb.Play();
                lives--;
                if (lives == 0)
                {
                    controller.GameOver(player, false);
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
                controller.GameOver(!player, false);
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
        bool r1, r2, r3;
        
        if (!powerup) return;
        else if (p == Power.ShowBoard)
        {
            r1 = powers.Remove(p);
            if (r1)
            {
                n1--;
                StartCoroutine(QuickShow());
            }
        }

        else if (p == Power.RemoveFlags)
        {
            r2 = powers.Remove(p);
            if (r2)
            {
                n2--;
                controller.PowerUp(player, p);
            }
        }

        else if (p == Power.InvertControls)
        {
            r3 = powers.Remove(p);
            if (r3)
            {
                n3--;
                controller.PowerUp(player, p);
            }
        }
            
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

        RevealAll();
        yield return new WaitForSeconds(2);

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

    private void Update()
    {
        nLives.text = "Lives: " + lives;

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

        else if (Input.GetKeyDown(right) && pos.x != width - 1)
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
            controller.flag.Play();
        }
        

        if (!controller.isTA)
        {
            p1.text = "Show: " + n1;
            p2.text = "Remove: " + n2;
            p3.text = "Invert: " + n3;

            if (Input.GetKeyDown(power1))
            {
                powerup = true;
                PowerUp(Power.ShowBoard);
            }

            else if (Input.GetKeyDown(power2))
            {
                powerup = true;
                PowerUp(Power.RemoveFlags);
            }

            else if (Input.GetKeyDown(power3))
            {
                powerup = true;
                PowerUp(Power.InvertControls);
            }
        }       
    }
}