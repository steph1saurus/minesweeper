
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [Header("Public variables")]
    //default size of gameboard
    public int width = 16;
    public int height = 16;
    public int mineCount = 32;

    [Header("Private variables")]
    private BoardScript board;
    private Cell[,] state;
    private bool gameOver;


    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }

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
        gameOver = false;
        //create 2D array of cells
        state = new Cell[width, height];

        GenerateCells();
        GenerateMines();
        GenerateNumbers();

        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        board.Draw(state);

    }

    private void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0); //for tile maps, use Vector3INt
                cell.type = Cell.Type.Empty; //initally, all cells are empty
                state[x, y] = cell;
            }
        }
    }

    private void GenerateMines()
    {
        for (int i = 0; i < mineCount; i++) //loop over mine and generate a new one up to the limit
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            //check prevent multiple mines on same coordinate

            while (state[x, y].type == Cell.Type.Mine)
            {
                x++;

                if (x >= width)
                {
                    x = 0;
                    y++;

                    if (y >= height)
                    {
                        y = 0;
                    }

                }
            }


            state[x, y].type = Cell.Type.Mine;

        }
    }

    private void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                if (cell.type == Cell.Type.Mine)
                {
                    continue;
                }


                cell.number = CountMines(x, y);

                if (cell.number > 0)
                {
                    cell.type = Cell.Type.Number;
                }


                state[x, y] = cell;

            }

        }

    }


    private int CountMines(int cellX, int cellY)
    {

        int count = 0;
        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0)
                {
                    continue;
                }

                int x = cellX + adjacentX;
                int y = cellY + adjacentY;

                //skip over tiles that are out of bounds
               
                if (GetCell(x,y).type == Cell.Type.Mine)
                {
                    count++;
                }

            }
        }

        return count;

    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }

        else if(!gameOver)
        {
            if (Input.GetMouseButtonDown(1))
            {
                FlagMouseLocation();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                RevealCell();
            }
        }
        
    }

    private void FlagMouseLocation()
    {   
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //convert screen position of the mouse to World position
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition); //convert mouse world position to cell
        Cell cell = GetCell(cellPosition.x, cellPosition.y);//Get cell position

        if (cell.type == Cell.Type.Invalid || cell.revealed)
        {
            return;
        }

        cell.flagged = !cell.flagged;
        state[cellPosition.x, cellPosition.y] = cell;
        board.Draw(state);

    }


    private void RevealCell()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);//convert screen position of the mouse to World position
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);//convert mouse world position to cell
        Cell cell = GetCell(cellPosition.x, cellPosition.y); //Get cell position

       
        if (cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged)//if cell is flagged, it will also not be revealed
        {
            return;
        }

        switch(cell.type)
        {
            case Cell.Type.Mine:
                Explode(cell);
                break;
            case Cell.Type.Empty:
                FloodEmpty(cell);
                CheckWinCondition();
                break;

            default:
                cell.revealed = true;
                state[cellPosition.x, cellPosition.y] = cell;
                CheckWinCondition();
                break;
        }

        
        board.Draw(state);
    }

    //use recursion to reveal all adjacent empty tiles if the tile selected is Empty
    private void FloodEmpty(Cell cell)
    {

        //Exit recursion conditions
        if (cell.revealed) return; //condition 1: if cell is already revealed, stop recursion
        if (cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) return; //condition 2: if the cell (about to be revealed) is a mine or invalid, stop recursion

        //if recursion conditions are not met
        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;

        //if the cell is empty, FloodEmpty() is called for next cell on X and Y axis in all directions
        if (cell.type == Cell.Type.Empty)
        {
            FloodEmpty(GetCell(cell.position.x -1, cell.position.y)); //cell to left
            FloodEmpty(GetCell(cell.position.x + 1, cell.position.y)); //cell to the right
            FloodEmpty(GetCell(cell.position.x, cell.position.y - 1)); //cell above
            FloodEmpty(GetCell(cell.position.x, cell.position.y + 1)); //cell below
        }

    }

    //when mine is clicked
    private void Explode(Cell cell)
    {
        Debug.Log("Game over");
        gameOver = true;

        cell.revealed = true;
        cell.explode = true; //renders exploded tile when mine is clicked
        state[cell.position.x, cell.position.y] = cell;

        for (int x = 0; x < width; x++)
        {
           for (int y = 0; y < height; y ++)
            {
                cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.revealed = true;
                    state[x, y] = cell;
                }

            }
        }

    }

    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                if (cell.type !=Cell.Type.Mine && !cell.revealed)
                {
                    return;
                }
            }
        }

        Debug.Log("You won!");
        gameOver = true;

        //reveal all remaining mines
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.flagged = true;
                    state[x, y] = cell;
                }

            }
        }

    }


    private Cell GetCell(int x, int y)
    {
        if (isValid(x, y))
        {
            return state[x, y];
        }
        else
        {
            return new Cell();
        }
    }

    private bool isValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

}
