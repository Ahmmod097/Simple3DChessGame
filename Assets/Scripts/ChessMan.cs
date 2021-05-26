using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessMan : MonoBehaviour
{
   public int CurrentX { set; get; }
   public int CurrentY { set; get; }
   public bool isWhite ;

    //Update the chessman or chess piece position after every move
   public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }

    //Create this function to override it 
    public virtual bool [,]PossibleMove()
    {
        return new bool[8, 8];
    }

    //Move around the board
    public bool Move(int x, int y, ref bool[,] r)
    {
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            ChessMan c = BoardManager.Instance.Chessmans[x, y];
            if (c == null)
                r[x, y] = true;
            else
            {
                if (isWhite != c.isWhite)
                    r[x, y] = true;
                return true;
            }
        }
        return false;
    }

}
