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

    public TileType tileType = 0;

    public bool show = false, flag = false;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickTile();
        }

        else if (Input.GetMouseButtonDown(1))
        {
            SetFlag();
        }
    }

    public void ClickTile()
    {
        RevealTile();
    }

    public void RevealTile()
    {
        if (!show)
        {
            show = true;
            sr.sprite = tileSprites[(int) tileType];
        }
    }

    public void SetFlag()
    {
        if (!flag)
        {
            flag = true;
            sr.sprite = tileSprites[(int) TileType.Flag];
        }
        else
        {
            flag = false;
            sr.sprite = tileSprites[(int)TileType.Normal];
        }
    }
}