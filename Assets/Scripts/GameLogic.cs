
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    //default size of gameboard
    public int width = 16;
    public int height = 16;

    //script references
    private BoardScript board;
    private Cell[,] state;


    private void Awake()
    {
        board = GetComponentInChildren<BoardScript>();
    }

    private void Start()
    {
        NewGame();
        
    }

    private void NewGame()
    {
        //create 2D array of cells
        state = new Cell[width, height];
        GenerateCells();

        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        board.Draw(state);
   
    }

    private void GenerateCells()
    {
        for(int x=0; x <width; x++)
        {
            for (int y = 0; y<height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0); //for tile maps, use Vector3INt
                cell.type = Cell.Type.Empty; //initally, all cells are empty
                state[x, y] = cell;
            }
        }
    }

}
