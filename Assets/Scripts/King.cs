using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessMan
{
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];
        ChessMan c,c1,c2,c3,c4;

        bool checkWhiteCastling = BoardManager.Instance.isWhiteCastling;
        bool whiteKingMoveCount = BoardManager.Instance.countWhiteKingMove;
        bool whiteRightRookMoveCount = BoardManager.Instance.countWhiteRightRookMove;
        bool whiteLeftRookMoveCount = BoardManager.Instance.countWhiteLeftRookMove;

        bool checkBlackCastling = BoardManager.Instance.isBlackCastling;
        bool blackKingMoveCount = BoardManager.Instance.countBlackKingMove;
        bool blackRightRookMoveCount = BoardManager.Instance.countBlackRightRookMove;
        bool blackLeftRookMoveCount = BoardManager.Instance.countBlackLeftRookMove;
        bool kingCastlingCheck = BoardManager.Instance.castleCheck;
        bool kingCastlingCheckAt2_0 = BoardManager.Instance.castleCheckAt2_0;
        bool kingCastlingCheckAt6_0 = BoardManager.Instance.castleCheckAt6_0;
        int i, j;

         //TopSide
         i = CurrentX - 1;
         j = CurrentY + 1;
         if (CurrentY != 7)
         {
             for(int k = 0; k < 3; k++)
             {
                 if(i>0 || i < 8)
                 {
                     c = BoardManager.Instance.Chessmans[i, j];
                     if (c == null)
                     {
                         r[i, j] = true;
                     }
                     else if(isWhite != c.isWhite)
                     {
                         r[i, j] = true;
                     }
                 }
                 i++;

             }
         }

         //DownSide
          i = CurrentX - 1;
          j = CurrentY - 1;
         if (CurrentY != 0)
         {
             for(int k = 0; k < 3; k++)
             {
                 if(i>0 || i < 8)
                 {
                     c = BoardManager.Instance.Chessmans[i, j];
                     if (c == null)
                     {
                         r[i, j] = true;
                     }
                     else if(isWhite != c.isWhite)
                     {
                         r[i, j] = true;
                     }
                 }
                 i++;

             }
         }

         //Middle Left
         if (CurrentX != 0)
         {
             c = BoardManager.Instance.Chessmans[CurrentX-1, CurrentY];
             if (c == null)
             {
                 r[CurrentX - 1, CurrentY] = true;
             }
             else if (isWhite != c.isWhite)
             {
                 r[CurrentX - 1, CurrentY] = true;
             }
         }

         //Middle Right
         if (CurrentX != 7)
         {
             c = BoardManager.Instance.Chessmans[CurrentX + 1, CurrentY];
             if (c == null)
             {
                 r[CurrentX + 1, CurrentY] = true;
             }
             else if (isWhite != c.isWhite)
             {
                 r[CurrentX + 1, CurrentY] = true;
             }
         }

        //Debug.Log( whiteKingCastlingCheck);
        //Castling Move For White King
       
        if (CurrentX == 4 && CurrentY == 0 && checkWhiteCastling==true && kingCastlingCheck == false) //&& whiteKingCastlingCheck == false) //Only Applicable for White king left side castling
        {

            
            c = BoardManager.Instance.Chessmans[CurrentX - 1, CurrentY];
            c1 = BoardManager.Instance.Chessmans[CurrentX - 2, CurrentY];
            c2 = BoardManager.Instance.Chessmans[CurrentX - 3, CurrentY];
            c3 = BoardManager.Instance.Chessmans[CurrentX + 1, CurrentY];
            c4 = BoardManager.Instance.Chessmans[CurrentX + 2, CurrentY];
            
            if ((c == null && c1 == null && c2 == null) && (c3 == null && c4 == null) &&
                whiteKingMoveCount == false && whiteRightRookMoveCount == false && whiteLeftRookMoveCount == false &&
                kingCastlingCheckAt2_0 == false && kingCastlingCheckAt6_0 == false)
            {
                r[CurrentX - 2, CurrentY] = true;
                r[CurrentX + 2, CurrentY] = true;

            }
            else if(c3 == null && c4 == null && whiteRightRookMoveCount == false && whiteKingMoveCount == false 
               && kingCastlingCheckAt6_0 == false)
            {
                r[CurrentX + 2, CurrentY] = true;
            }
            else if(c == null && c1 == null && c2 == null && whiteLeftRookMoveCount == false && whiteKingMoveCount == false 
                && kingCastlingCheckAt2_0 == false)
            {
                r[CurrentX - 2, CurrentY] = true;
            }
        }

        //Castling Move For Black  King

        if (CurrentX == 4 && CurrentY == 7 && checkBlackCastling == true && kingCastlingCheck == false) //Only Applicable for White king left side castling
        {


            c = BoardManager.Instance.Chessmans[CurrentX - 1, CurrentY];
            c1 = BoardManager.Instance.Chessmans[CurrentX - 2, CurrentY];
            c2 = BoardManager.Instance.Chessmans[CurrentX - 3, CurrentY];
            c3 = BoardManager.Instance.Chessmans[CurrentX + 1, CurrentY];
            c4 = BoardManager.Instance.Chessmans[CurrentX + 2, CurrentY];

            if ((c == null && c1 == null && c2 == null) && (c3 == null && c4 == null) &&
                blackKingMoveCount == false && blackRightRookMoveCount == false && blackLeftRookMoveCount == false
                && kingCastlingCheckAt2_0 == false && kingCastlingCheckAt6_0 == false)
            {
                r[CurrentX - 2, CurrentY] = true;
                r[CurrentX + 2, CurrentY] = true;

            }
            else if (c3 == null && c4 == null && blackRightRookMoveCount == false && 
                blackKingMoveCount == false &&  kingCastlingCheckAt6_0 == false)
            {
                r[CurrentX + 2, CurrentY] = true;
            }
            else if (c == null && c1 == null && c2 == null && blackLeftRookMoveCount == false && blackKingMoveCount == false
                && kingCastlingCheckAt2_0 == false)
            {
                r[CurrentX - 2, CurrentY] = true;
            }
        }


        return r;

    }
}
