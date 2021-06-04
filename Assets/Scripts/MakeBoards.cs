using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeBoards : MonoBehaviour
{
    [SerializeField]
    GameObject board;


    // Start is called before the first frame update
    void Start()
    {
        var b1 = Instantiate(board, new Vector3(-7, 3, 0), Quaternion.identity);
        b1.GetComponent<Board>().New(16, 16, 40);
        
        var b2 = Instantiate(board, new Vector3(1, 3, 0), Quaternion.identity);
        b2.GetComponent<Board>().New(16, 16, 40);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
