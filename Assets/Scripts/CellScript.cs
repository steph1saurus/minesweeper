
using UnityEngine;

public struct Cell

{
//define classification of cells
public enum Type
    {
        Invalid,
        Empty,
        Mine,
        Number,
    }
    public Vector3Int position; //where the cell is on the "board" with XY coordinates
    public Type type;
    public int number; //what number is displayed
    public bool revealed; //if cell has been revealed or not clicked yet
    public bool flagged; //if flag is placed on cell
    public bool explode; //if cell is a mine

}
