using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { set; get; } //so that the chess man knows everything what is happend on the board before any move
    private bool[,] allowedMoves { set; get; }
    private bool[,] availableMoves { set; get; }

    private const float Title_size = 1.0f;
    private const float Title_offset = 0.5f; // half size of title_size
    private int selectionX = -1;
    private int selectionY = -1;
    public List<GameObject> ChessPieces;
    private List<GameObject> activechessman;
    private List<ChessMan> Pieces;
    public ChessMan[,] Chessmans { set; get; } //This is Chess Board
    private ChessMan selectedChessman; //Which piece or chess man is currently selected

    public bool isWhiteturn = true;
    public bool isBlackturn = true;
    public int[] enPassantMove; //Make This Accesible for our pawns
    public GameObject whiteWin;
    public GameObject blackWin;
    public GameObject whiteCheck;
    public GameObject blackCheck;

    public int cnt = 0;
    public bool isWhiteCastling = true;
    public bool countWhiteKingMove = false;
    public bool countWhiteLeftRookMove = false;
    public bool countWhiteRightRookMove = false;
    
    public bool checkCheckAtAnyTime = false;
    public bool castleCheck = false;
    public bool castleCheckAt6_0 = false;
    public bool castleCheckAt2_0 = false;
    public bool pawnCheck = false;

    public bool isBlackCastling = true;
    public bool countBlackKingMove = false;
    public bool countBlackLeftRookMove = false;
    public bool countBlackRightRookMove = false;



    private void Start() //This method is used to create black and white chessman on the chess board
    {
        Instance = this;

        SpawnAllChessMan();



    }
    private void Update()
    {
        UpdateSelection();
        DrawChessBoard(); //Create The Chess Board


        if (Input.GetMouseButtonDown(0))
        {
            
            if (selectionX >= 0 && selectionY >= 0)
            {

                if (selectedChessman == null)
                {


                    SelectChessMan(selectionX, selectionY); //Used to select the square in the board where we want our chess piece to go


                }
                else
                {



                    MoveChessMan(selectionX, selectionY); //Move the Chessman to our desired position

                }
            }

        }

    }

    //Used to select the square in the board where we want our chess piece to go
    private void SelectChessMan(int x, int y)
    {


        if (Chessmans[x, y] == null)
        {
            return;
        }
        if (Chessmans[x, y].isWhite != isWhiteturn)
        {
            return;
        }
        bool hasatLeastOnemove = false;
        
        allowedMoves = Chessmans[x, y].PossibleMove();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (allowedMoves[i, j])
                {
                    hasatLeastOnemove = true;
                }
            }
        }
        if (!hasatLeastOnemove)
        {
            return;
        }

        selectedChessman = Chessmans[x, y];
        


        BoardHighlight.Instance.HighLightAllowedMoves(allowedMoves);



    }

    //Move the Chessman to our desired position
    private void MoveChessMan(int x, int y)
    {

        if (allowedMoves[x, y])
        {
           
            int ans = 0;
            ChessMan c = Chessmans[x, y];
            
            
            if (c != null && c.isWhite != isWhiteturn)
            {
                
                //kill the enemy

                //if it is the king
                if (c.GetType() == typeof(King)) //Get Type returns the lowest children, like King is the lowest children(0)
                {
                    //end the game
                    Endgame();
                    return;
                }
               
                activechessman.Remove(c.gameObject);
                Pieces.Remove(c);
                Destroy(c.gameObject);
                //Debug.Log(isCheck());
                //Debug.Log(ans);
                
                
            }


            if (x == enPassantMove[0] && y == enPassantMove[1] && checkCheckAtAnyTime == false)
            {


                if (isWhiteturn)
                {
                    c = Chessmans[x, y - 1];

                }
                else
                {
                    c = Chessmans[x, y + 1];
                }
                activechessman.Remove(c.gameObject);
                Pieces.Remove(c);
                Destroy(c.gameObject);
            }
            enPassantMove[0] = -1;
            enPassantMove[1] = -1;

            if (selectedChessman.GetType() == typeof(Pawn)) //Check our current selected chess man is pawn or not
            {
                checkCheckAtAnyTime = isCheck();

                if (y == 7)
                {
                    activechessman.Remove(selectedChessman.gameObject);
                    Pieces.Remove(selectedChessman);
                    Destroy(selectedChessman.gameObject);
                    SpawnChessMan(1, x, y); // Spawn Promoted To Queen
                    selectedChessman = Chessmans[x, y];
                }
                else if (y == 0)
                {
                    activechessman.Remove(selectedChessman.gameObject);
                    Pieces.Remove(selectedChessman);
                    Destroy(selectedChessman.gameObject);
                    SpawnChessMan(7, x, y);
                    selectedChessman = Chessmans[x, y];
                }
                enPassantMove[0] = x;
                if (selectedChessman.CurrentY == 1 && y == 3)
                {
                    enPassantMove[1] = y - 1;
                }
                else if (selectedChessman.CurrentY == 6 && y == 4)
                {
                    enPassantMove[1] = y + 1;
                }
            }


            if (selectedChessman.GetType() == typeof(Rook)) //Check if this is the first move for Rook which helps to find if castling is possible or not
            {
                if (selectedChessman.CurrentY == 0 && selectedChessman.CurrentX == 0) //White Rook
                {
                    countWhiteLeftRookMove = true;

                }
                else if (selectedChessman.CurrentY == 0 && selectedChessman.CurrentX == 7)
                {
                    countWhiteRightRookMove = true;

                }
                else if (selectedChessman.CurrentY == 7 && selectedChessman.CurrentX == 0)
                {
                    countBlackLeftRookMove = true;

                }
                else if (selectedChessman.CurrentY == 7 && selectedChessman.CurrentX == 7)
                {
                    countBlackRightRookMove = true;

                }

            }

            //Castling Trying For White King
            if (selectedChessman.GetType() == typeof(King)) // To check if king piece is clicked or not
            {

                if (selectedChessman.CurrentY == 0 && selectedChessman.CurrentX == 4)
                {

                    countWhiteKingMove = true;
                    

                    if (x == 2 && y == 0)
                    {

                        c = Chessmans[0, 0];
                        if (c.GetType() == typeof(Rook))
                        {
                            //Debug.Log(selectedChessman.GetComponent<Rook>().CurrentX);
                            SpawnChessMan(2, x + 1, y);


                            //Delete the Rook object


                            activechessman.Remove(c.gameObject);
                            Pieces.Remove(c);
                            Destroy(c.gameObject);

                            // DeletePiece();
                            //countWhiteKingMove = true; // So that once you moved your king, you cant do castling
                            //countWhiteLeftRookMove = true; // So that once you moved your rook, you cant do castling
                            //countWhiteRightRookMove = true;

                        }
                        else
                        {

                            Debug.Log("You can't do castling");
                            isWhiteCastling = false;
                        }
                    }
                    /*if (x!=2 || y!=0)
                    {
                        countKingMove = true;
                    }*/


                    else if (x == 6 && y == 0)
                    {
                        c = Chessmans[7, 0];
                        if (c.GetType() == typeof(Rook))
                        {
                            SpawnChessMan(2, x - 1, y);


                            //Delete the Rook object


                            activechessman.Remove(c.gameObject);
                            Pieces.Remove(c);
                            Destroy(c.gameObject);

                            // DeletePiece();
                            ///countWhiteKingMove = true; // So that once you moved your king, you cant do castling
                            //countWhiteRightRookMove = true; // So that once you moved your rook, you cant do castling
                            //countWhiteLeftRookMove = true;
                        }
                        else
                        {

                            Debug.Log("You can't do castling");
                            isWhiteCastling = false;
                        }
                    }
                    else
                    {
                        countWhiteKingMove = true;
                    }

                }


            }

            //Castling Trying For Black King
            if (selectedChessman.GetType() == typeof(King)) // To check if king piece is clicked or not
            {
                if (selectedChessman.CurrentY == 7 && selectedChessman.CurrentX == 4)
                {
                    countBlackKingMove = true;

                    if (x == 2 && y == 7)
                    {
                        c = Chessmans[0, 7];
                        if (c.GetType() == typeof(Rook))
                        {
                            //Debug.Log(selectedChessman.GetComponent<Rook>().CurrentX);
                            SpawnChessMan(8, x + 1, y);


                            //Delete the Rook object


                            activechessman.Remove(c.gameObject);
                            Pieces.Remove(c);
                            Destroy(c.gameObject);

                            // DeletePiece();
                            //countBlackKingMove = true; // So that once you moved your king, you cant do castling
                            //countBlackLeftRookMove = true; // So that once you moved your rook, you cant do castling
                            //countBlackRightRookMove = true;

                        }
                        else
                        {

                            Debug.Log("You can't do castling");
                            isBlackCastling = false;
                        }
                    }
                    /*if (x!=2 || y!=0)
                    {
                        countKingMove = true;
                    }*/


                    else if (x == 6 && y == 7)
                    {
                        c = Chessmans[7, 7];
                        if (c.GetType() == typeof(Rook))
                        {
                            SpawnChessMan(8, x - 1, y);


                            //Delete the Rook object


                            activechessman.Remove(c.gameObject);
                            Pieces.Remove(c);
                            Destroy(c.gameObject);

                            // DeletePiece();
                            ///countBlackKingMove = true; // So that once you moved your king, you cant do castling
                            ///countBlackRightRookMove = true; // So that once you moved your rook, you cant do castling
                            //countBlackLeftRookMove = true;
                        }
                        else
                        {

                            Debug.Log("You can't do castling");
                            isBlackCastling = false;
                        }
                    }
                    else
                    {
                        countBlackKingMove = true;
                    }

                }
            }






            //Checking Check 
            bool b = isWhiteturn;
            if (!isCheck())
            {

                if (b)
                {

                    whiteCheck.SetActive(false);
                    int tx = selectedChessman.CurrentX;
                    int ty = selectedChessman.CurrentY;
                   
                    Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;

                    selectedChessman.transform.position = getTitlecenter(x, y);
                    selectedChessman.SetPosition(x, y); //We get the selected position x and position y
                                                        //Debug.Log(Chessmans[x, y]);
                    
                    ChessMan blackPiece;
                    if (Chessmans[x, y] != null )
                    {
                        blackPiece = Chessmans[x, y] ;
                    }
                    else
                    {
                        blackPiece = null;
                        //Destroy(selectedChessman.gameObject);
                    }
                    Chessmans[x, y] = selectedChessman;
                    if (isCheck())
                    {

                        
                        
                        Destroy(selectedChessman.gameObject);
                        createWhitePiece(selectedChessman, tx, ty);
                        if (blackPiece != null)
                        {
                            createBlackPiece(blackPiece, x, y);
                        }
                        isWhiteturn = b;



                    }
                    else
                    {
                        ans = 0;
                        
                       
                        
                        isWhiteturn = !b;
                    }



                }
                else
                {
                    blackCheck.SetActive(false);
                    int tx = selectedChessman.CurrentX;
                    int ty = selectedChessman.CurrentY;
                    Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
                    selectedChessman.transform.position = getTitlecenter(x, y);
                    selectedChessman.SetPosition(x, y); //We get the selected position x and position y
                    
                    ChessMan whitePiece;
                    if (Chessmans[x, y] != null)
                    {
                        whitePiece = Chessmans[x, y];
                    }
                    else
                    {
                        whitePiece = null;
                        //Destroy(selectedChessman.gameObject);
                    }
                    Chessmans[x, y] = selectedChessman;
                    if (isCheck())
                    {
                        Destroy(selectedChessman.gameObject);
                        createBlackPiece(selectedChessman, tx, ty);
                        if (whitePiece != null)
                        {
                            createWhitePiece(whitePiece, x, y);
                        }
                        isWhiteturn = b;



                    }
                    else
                    {
                        ans = 0;
                        isWhiteturn = !b;
                    }
                }
            }
            else
            {

                if (b)
                {

                    whiteCheck.SetActive(true);
                    int tx = selectedChessman.CurrentX;
                    int ty = selectedChessman.CurrentY;
                    Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
                    selectedChessman.transform.position = getTitlecenter(x, y);
                    selectedChessman.SetPosition(x, y); //We get the selected position x and position y
                    ChessMan blackPiece1;
                    if (Chessmans[x, y] != null)
                    {
                        blackPiece1 = Chessmans[x, y];
                    }
                    else
                    {
                        blackPiece1 = null;
                        //Destroy(selectedChessman.gameObject);
                    }
                    Chessmans[x, y] = selectedChessman;

                    isCheck();
                    if (isCheck() == false)
                    {
                        whiteCheck.SetActive(false);
                        isWhiteturn = !b;
                    }
                    else
                    {

                        Debug.Log("Check");
                        Destroy(selectedChessman.gameObject);

                        createWhitePiece(selectedChessman, tx, ty);
                        if (blackPiece1 != null)
                        {
                            createBlackPiece(blackPiece1, x, y);
                        }

                        isWhiteturn = b;
                    }



                }
                else
                {

                    blackCheck.SetActive(true);
                    int tx = selectedChessman.CurrentX;
                    int ty = selectedChessman.CurrentY;
                    Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
                    selectedChessman.transform.position = getTitlecenter(x, y);
                    selectedChessman.SetPosition(x, y); //We get the selected position x and position y
                    ChessMan whitePiece1;
                    if (Chessmans[x, y] != null)
                    {
                        whitePiece1 = Chessmans[x, y];
                    }
                    else
                    {
                        whitePiece1 = null;
                        //Destroy(selectedChessman.gameObject);
                    }
                    Chessmans[x, y] = selectedChessman;

                    isCheck();
                    if (isCheck() == false)
                    {
                        blackCheck.SetActive(false);
                        isWhiteturn = !b;
                    }
                    else
                    {


                        Destroy(selectedChessman.gameObject);

                        createBlackPiece(selectedChessman, tx, ty);
                        if (whitePiece1 != null)
                        {
                            createWhitePiece(whitePiece1, x, y);
                        }

                        isWhiteturn = b;
                    }
                }


            }

            //Test




            //isWhiteturn = !isWhiteturn;
            if (isCheck())
            {

                if (isWhiteturn)
                {
                    
                    whiteCheck.SetActive(true);
                    castleCheck = true;
                    
                }
                else
                {
                    
                    blackCheck.SetActive(true);
                    castleCheck = true;
                    
                }

            }
            else
            {

                if (isWhiteturn)
                {
                    whiteCheck.SetActive(false);
                    castleCheck = false;
                    castleCheckAt2_0 = isCheckAt_2_0_Position();
                    
                    castleCheckAt6_0 = isCheckAt_6_0_Position();
                    
                }
                else
                {
                    blackCheck.SetActive(false);
                    castleCheck = false;
                    castleCheckAt2_0 = isCheckAt_2_0_Position();
                    
                    castleCheckAt6_0 = isCheckAt_6_0_Position();
                    

                }
            }
            
            
            

        }
        BoardHighlight.Instance.Hidehighlights();
       
        
        selectedChessman = null;
    }

    private void UpdateSelection()
    {
        if (!Camera.main)
        {
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Plane")))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    //Create the White Chessman or Chess Piece
    private void SpawnChessMan(int index, int x, int y)
    {
        //Test
        ChessMan cm;
        //Finish
        GameObject go = Instantiate(ChessPieces[index], getTitlecenter(x, y), Quaternion.Euler(-90, 0, -90)) as GameObject;
        go.name = ChessPieces[index].name;
        go.transform.SetParent(transform);

        //Test
        cm = go.GetComponent<ChessMan>();
        //Finish

        Chessmans[x, y] = go.GetComponent<ChessMan>();
        Chessmans[x, y].SetPosition(x, y); //Here we get the chessman position
        //Debug.Log(Chessmans[x, y].CurrentX);
        //Debug.Log(Chessmans[x, y].CurrentY);
        activechessman.Add(go);

        //Test
        Pieces.Add(cm);
        //Finish
    }

    //Create the Black Chessman or Chess Piece
    private void SpawnChessManBlack(int index, int x, int y)
    {
        // Test
        ChessMan cm;
        //Finish

        GameObject go = Instantiate(ChessPieces[index], getTitlecenter(x, y), Quaternion.Euler(-90, 0, 90)) as GameObject;
        go.name = ChessPieces[index].name;
        go.transform.SetParent(transform);
        //Debug.Log(go.transform);

        //Test
        cm = go.GetComponent<ChessMan>();
        //Finish

        Chessmans[x, y] = go.GetComponent<ChessMan>();
        Chessmans[x, y].SetPosition(x, y); //Here we get the chessman position
        //Debug.Log(Chessmans[x, y].CurrentX);
        //Debug.Log(Chessmans[x, y].CurrentY);
        activechessman.Add(go);
        Pieces.Add(cm);
    }


    //Place the created chessman on the board at the start
    private void SpawnAllChessMan()
    {
        activechessman = new List<GameObject>();

        //Test
        Pieces = new List<ChessMan>();


        Chessmans = new ChessMan[8, 8];
        enPassantMove = new int[2] { -1, -1 };

        //AllWhite

        //King
        SpawnChessMan(0, 4, 0);

        //Queen
        SpawnChessMan(1, 3, 0);

        //Rook
        SpawnChessMan(2, 0, 0);
        SpawnChessMan(2, 7, 0);

        //Bishop
        SpawnChessMan(3, 2, 0);
        SpawnChessMan(3, 5, 0);

        //Knight
        SpawnChessMan(4, 1, 0);
        SpawnChessMan(4, 6, 0);

        //Pawn
        for (int i = 0; i < 8; i++)
        {
            SpawnChessMan(5, i, 1);
        }

        //AllBlack

        //King
        SpawnChessManBlack(6, 4, 7);

        //Queen
        SpawnChessManBlack(7, 3, 7);

        //Rook
        SpawnChessManBlack(8, 0, 7);
        SpawnChessManBlack(8, 7, 7);

        //Bishop
        SpawnChessManBlack(9, 2, 7);
        SpawnChessManBlack(9, 5, 7);

        //Knight
        SpawnChessManBlack(10, 1, 7);
        SpawnChessManBlack(10, 6, 7);

        //Pawn
        for (int i = 0; i < 8; i++)
        {
            SpawnChessManBlack(11, i, 6);
        }
    }

    //Used to place the chess piece or chess man at the center of a square
    private Vector3 getTitlecenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (Title_size * x) + Title_offset;
        origin.z += (Title_size * y) + Title_offset;
        return origin;
    }

    //Draw the Chess Board
    private void DrawChessBoard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 forwardLine = Vector3.forward * 8;
        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);

            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + forwardLine);
            }
        }
        if (selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * selectionX, Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
            Debug.DrawLine(Vector3.forward * (selectionY + 1) + Vector3.right * selectionX, Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
        }
    }

    //End the game
    private void Endgame()
    {
        if (isWhiteturn)
        {

            whiteWin.SetActive(true);
            Debug.Log("White Team Wins!!");
        }
        else
        {

            blackWin.SetActive(true);
            Debug.Log("Black Team Wins!!");
        }
        BoardHighlight.Instance.Hidehighlights();

    }
    public void restart()
    {
        whiteWin.SetActive(false);
        blackWin.SetActive(false);
        whiteCheck.SetActive(false);
        blackCheck.SetActive(false);
        foreach (GameObject go in activechessman)
        {

            Destroy(go);

        }
        isWhiteturn = true;
        BoardHighlight.Instance.Hidehighlights();
        SpawnAllChessMan();
    }



    // Get king's Position 
    public bool isCheck()
    {


        if (isWhiteturn)
        {
            GameObject ob = GameObject.FindGameObjectWithTag("KingWhite");

            int PosX = (int)((int)ob.transform.position.x);
            int PosY = (int)((int)ob.transform.position.z);

            //allowedMoves = Chessmans[PosX, PosY].PossibleMove();


            for (int i = (Pieces.Count) / 2; i < Pieces.Count; i++)
            {
                ChessMan piece = Pieces[i];

                availableMoves = piece.PossibleMove();

                if (availableMoves[PosX, PosY])
                {

                    Debug.Log("White King is in Check");

                    return true;

                }
            }
        }
        else
        {
            GameObject ob = GameObject.FindGameObjectWithTag("KingBlack");

            int PosX = (int)((int)ob.transform.position.x);
            int PosY = (int)((int)ob.transform.position.z);

            //allowedMoves = Chessmans[PosX, PosY].PossibleMove();
            for (int i = 0; i < (Pieces.Count) / 2; i++)
            {
                ChessMan piece = Pieces[i];
                availableMoves = piece.PossibleMove();

                if (availableMoves[PosX, PosY])
                {

                    Debug.Log("Black King is in Check");

                    return true;

                }
            }
        }

        return false;
    }

    public void isCheckMate()
    {
        
        if (isWhiteturn)
        {
            whiteCheck.SetActive(false);
            blackCheck.SetActive(false);
            blackWin.SetActive(true);
            Debug.Log("Black Team Wins!!");
        }
        else
        {
            whiteCheck.SetActive(false);
            blackCheck.SetActive(false);
            whiteWin.SetActive(true);

            Debug.Log("White Team Wins!!");
        }
    }

    private void createWhitePiece(ChessMan ch, int posX, int posY)
    {
        int index12 = -1;
        string s1;
        string s2;
        for (int i = 0; i < ChessPieces.Count; i++)
        {
            s1 = ch.gameObject.name;

            s2 = ChessPieces[i].name;

            if (s1 == s2)
            {

                index12 = i;
            }
        }


        SpawnChessMan(index12, posX, posY);


    }
    private void createBlackPiece(ChessMan ch, int posX, int posY)
    {
        int index12 = -1;
        for (int i = 0; i < ChessPieces.Count; i++)
        {
            string s1 = ch.gameObject.name;

            string s2 = ChessPieces[i].name;
            if (s1 == s2)
            {
                index12 = i;
            }
        }



        SpawnChessManBlack(index12, posX, posY);


    }

    public bool isCheckAt_2_0_Position()
    {


        if (isWhiteturn)
        {


            int PosX1 = 3;
            int PosY1 = 0;
            int PosX2 = 2;
            int PosY2 = 0;
            int cntTrue = 0;
            int cntTrue1 = 0;
            
            ChessMan checkPiece = Chessmans[PosX1, PosY1];
            ChessMan checkPiece1 = Chessmans[PosX2, PosY2];
            //allowedMoves = Chessmans[PosX, PosY].PossibleMove();
            if (checkPiece1 == null)
            {
                cntTrue = 0;
                for (int i = (Pieces.Count) / 2; i < Pieces.Count; i++)
                {
                    
                    ChessMan piece = Pieces[i];
                    
                    availableMoves = piece.PossibleMove();



                    
                    if (availableMoves[PosX2, PosY2])
                    {

                        cntTrue++;
                        
                    }

                }
                
            }
            
            if (checkPiece == null)
            {
                cntTrue1 = 0;
                for (int i = (Pieces.Count) / 2; i < Pieces.Count; i++)
                {

                    ChessMan piece = Pieces[i];
                   
                    availableMoves = piece.PossibleMove();




                    if (availableMoves[PosX1, PosY1])
                    {
                        
                        cntTrue1++;
                        
                    }

                }

            }


            if (cntTrue == 0 && cntTrue1 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {


            int PosX1 = 3;
            int PosY1 = 7;
            int PosX2 = 2;
            int PosY2 = 7;

            //allowedMoves = Chessmans[PosX, PosY].PossibleMove();
            int cntTrue = 0;
            int cntTrue1 = 0;

            ChessMan checkPiece = Chessmans[PosX1, PosY1];
            ChessMan checkPiece1 = Chessmans[PosX2, PosY2];

            if (checkPiece1 == null)
            {
                cntTrue = 0;
                for (int i = 0; i< (Pieces.Count) / 2; i++)
                {

                    ChessMan piece = Pieces[i];

                    availableMoves = piece.PossibleMove();




                    if (availableMoves[PosX2, PosY2])
                    {

                        cntTrue++;

                    }

                }

            }

            if (checkPiece == null)
            {
                cntTrue1 = 0;
                for (int i = 0; i < (Pieces.Count) / 2; i++)
                {

                    ChessMan piece = Pieces[i];

                    availableMoves = piece.PossibleMove();




                    if (availableMoves[PosX1, PosY1])
                    {

                        cntTrue1++;

                    }

                }

            }


            if (cntTrue == 0 && cntTrue1 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        } 

       
    }

    public bool isCheckAt_6_0_Position()
    {


        if (isWhiteturn)
        {


            int PosX1 = 5;
            int PosY1 = 0;
            int PosX2 = 6;
            int PosY2 = 0;
            int cntTrue = 0;
            int cntTrue1 = 0;

            ChessMan checkPiece = Chessmans[PosX1, PosY1];
            ChessMan checkPiece1 = Chessmans[PosX2, PosY2];
            //allowedMoves = Chessmans[PosX, PosY].PossibleMove();
            if (checkPiece1 == null)
            {
                cntTrue = 0;
                for (int i = (Pieces.Count) / 2; i < Pieces.Count; i++)
                {

                    ChessMan piece = Pieces[i];

                    availableMoves = piece.PossibleMove();




                    if (availableMoves[PosX2, PosY2])
                    {

                        cntTrue++;

                    }

                }

            }

            if (checkPiece == null)
            {
                cntTrue1 = 0;
                for (int i = (Pieces.Count) / 2; i < Pieces.Count; i++)
                {

                    ChessMan piece = Pieces[i];

                    availableMoves = piece.PossibleMove();




                    if (availableMoves[PosX1, PosY1])
                    {

                        cntTrue1++;

                    }

                }

            }


            if (cntTrue == 0 && cntTrue1 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {


            int PosX1 = 5;
            int PosY1 = 7;
            int PosX2 = 6;
            int PosY2 = 7;

            //allowedMoves = Chessmans[PosX, PosY].PossibleMove();
            int cntTrue = 0;
            int cntTrue1 = 0;

            ChessMan checkPiece = Chessmans[PosX1, PosY1];
            ChessMan checkPiece1 = Chessmans[PosX2, PosY2];

            if (checkPiece1 == null)
            {
                cntTrue = 0;
                for (int i = 0; i < (Pieces.Count) / 2; i++)
                {

                    ChessMan piece = Pieces[i];

                    availableMoves = piece.PossibleMove();




                    if (availableMoves[PosX2, PosY2])
                    {

                        cntTrue++;

                    }

                }

            }

            if (checkPiece == null)
            {
                cntTrue1 = 0;
                for (int i = 0; i < (Pieces.Count) / 2; i++)
                {

                    ChessMan piece = Pieces[i];

                    availableMoves = piece.PossibleMove();




                    if (availableMoves[PosX1, PosY1])
                    {

                        cntTrue1++;

                    }

                }

            }


            if (cntTrue == 0 && cntTrue1 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }

}