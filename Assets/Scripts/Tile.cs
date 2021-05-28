using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Normal = 0,
    Empty = 1,
    Flag = 2,
    Mine = 3,
    MineExplode = 4,
    WrongFlag = 5,
    N1 = 6,
    N2 = 7,
    N3 = 8,
    N4 = 9,
    N5 = 10,
    N6 = 11,
    N7 = 12,
    N8 = 13
}

public class Tile : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> tileSprites;

    private Board board;
    private List<Vector2Int> nearPos;

    public bool shown = false, flag = false;

    public bool mine = false;
    public int minesNear = 0;
    private int x, y;

    private SpriteRenderer sr;

    public void New(Board b, int x_, int y_)
    {
        board = b;
        x = x_;
        y = y_;

        transform.position = new Vector3(x * sr.bounds.size.x, -y * sr.bounds.size.y, 0);
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!flag)
                Click();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ToggleFlag();
        }
    }

    public void Click()
    {
        board.Click(x, y);
    }

    public void Reveal()
    {
        if (shown || flag)
            return;

        shown = true;
        if (mine)
        {
            sr.sprite = tileSprites[(int)TileType.MineExplode];
        }
        else
        {
            if (minesNear > 0)
            {
                sr.sprite = tileSprites[(int)TileType.N1 - 1 + minesNear];
            }
            else
            {
                sr.sprite = tileSprites[(int)TileType.Empty];
                SetNear();
                nearPos.ForEach(p => board.board[p[0]][p[1]].Reveal());
            }
        }
    }

    public void ToggleFlag()
    {
        if (shown)
            return;

        if (!flag)
        {
            flag = true;
            sr.sprite = tileSprites[(int)TileType.Flag];
        }
        else
        {
            flag = false;
            sr.sprite = tileSprites[(int)TileType.Normal];
        }
    }

    public void Hide()
    {
        shown = false;
        if (flag) sr.sprite = tileSprites[(int)TileType.Flag];
        else sr.sprite = tileSprites[(int)TileType.Normal];
    }

    public void SetMine()
    {
        mine = true;
        SetNear();
        nearPos.ForEach(p => board.board[p[0]][p[1]].minesNear++);
    }

    public void UnsetMine()
    {
        mine = false;
        SetNear();
        nearPos.ForEach(p => board.board[p[0]][p[1]].minesNear--);
    }

    private void SetNear()
    {
        if (nearPos != null)
            return;

        nearPos = new List<Vector2Int>();

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i < 0 || j < 0)
                    continue;
                if (board.Contains(i, j))
                {
                    nearPos.Add(new Vector2Int(i, j));
                }
            }
        }
    }
}