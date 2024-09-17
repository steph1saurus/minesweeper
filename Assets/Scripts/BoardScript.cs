
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardScript : MonoBehaviour
{

    public Tilemap tilemap { get; private set; } //other script will need to access and read tilemap but doesn't need to change values

    public Tile tileUnknown;
    public Tile tileEmpty;
    public Tile tileMine;
    public Tile tileExplode;
    public Tile tileFlag;
    public Tile tileNum1;
    public Tile tileNum2;
    public Tile tileNum3;
    public Tile tileNum4;
    public Tile tileNum5;
    public Tile tileNum6;
    public Tile tileNum7;
    public Tile tileNum8;


    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();

    }

    public void Draw(Cell[,] state) //calls array of Cell
    {
        int width = state.GetLength(0);
        int height = state.GetLength(1);


        //loops
        for (int x = 0; x< width; x++)
        {
            for (int y = 0; y< height; y++)
            {
                Cell cell = state[x, y]; //extracting data at the x,y coordinates
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }

    private Tile GetTile(Cell cell)
    {
        if (cell.revealed)
        {
            return GetRevealedTile(cell);
        }
        else if (cell.flagged)
        {
            return tileFlag;
        }
        else
        {
            return tileUnknown;
        }
    }

    private Tile GetRevealedTile(Cell cell)
    {
        switch(cell.type)
        {
            case Cell.Type.Empty: return tileEmpty;
            case Cell.Type.Mine: return cell.explode ? tileExplode : tileMine; //returns the exploded tile, or else returns regular mine tile
            case Cell.Type.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    private Tile GetNumberTile (Cell cell)
    {
        switch(cell.number)
        {
            case 1: return tileNum1;
            case 2: return tileNum2;
            case 3: return tileNum3;
            case 4: return tileNum4;
            case 5: return tileNum5;
            case 6: return tileNum6;
            case 7: return tileNum7;
            case 8: return tileNum8;
            default: return null;
        }
    }

}
