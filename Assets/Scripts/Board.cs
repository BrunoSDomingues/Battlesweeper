using System.Collections.Generic;
using UnityEngine;


public class Board : MonoBehaviour {
    [SerializeField]
    private GameObject TileObject;

    public List<List<Tile>> board = new List<List<Tile>>();
    private List<Vector2Int> available = new List<Vector2Int>();
    private int width;
    private int height;
    private int nMines;

    private bool firstClick = true;

    public void New(int width_, int height_, int nMines_) {
        width = width_;
        height = height_;
        nMines = nMines_;

        // No need to place mines before first click
        CreateBoard();
    }

    private void SetMines(int x, int y) {
        var random = new System.Random();
        while (true) {
            Debug.Log("while");
            // Shuffle available
            // https://www.codegrepper.com/code-examples/csharp/c%23+how+to+shuffle+a+list
            for (int i = available.Count - 1; i > 0; i--) {
                int idx = random.Next(i + 1);
                var temp = available[i];
                available[i] = available[idx];
                available[idx] = temp;
            }

            for (int i = 0; i < nMines; i++) {
                var pos = available[i];
                board[pos[0]][pos[1]].SetMine();
            }

            if (board[x][y].minesNear == 0) {
                break;
            }

            for (int i = 0; i < nMines; i++) {
                var pos = available[i];
                board[pos[0]][pos[1]].UnsetMine();
            }

        }
    }

    public void CreateBoard() {
        board.Clear();
        available.Clear();
        firstClick = true;

        // Instantiate and add tiles
        for (int i = 0; i < height; i++) {
            board.Add(new List<Tile>());
            for (int j = 0; j < width; j++) {
                GameObject obj = Instantiate(TileObject);
                Tile t = obj.GetComponent<Tile>();
                t.New(this, i, j);
                // TODO set position

                board[i].Add(t);
                available.Add(new Vector2Int(i, j));
            }
        }
    }

    public void Click(int x, int y) {
        if (firstClick)
            SetMines(x, y);
        else
            firstClick = false;
    }

    public bool Contains(int i, int j) => (i < height && j < width);
    public bool Contains(Vector2Int pos) => Contains(pos.x, pos.y);

    void Start() {
        New(16, 30, 99);
        var s = TileObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        var spriteWidth = s.x;
        var spriteHeight = s.z;
        Debug.Log("Width " + spriteWidth);
        Debug.Log("Height " + spriteHeight);

    }

    //// Update is called once per frame
    //void Update() {

    //}
}
