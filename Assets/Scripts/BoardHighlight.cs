using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHighlight : MonoBehaviour
{

    public static BoardHighlight Instance { set; get; } //Referrence of this class so that we can use it in BoardManager

    public GameObject highlightPrefab;
    private List<GameObject> highlights;
    private void Start() //Instantiate List and object here
    {
        Instance = this;
        highlights = new List<GameObject>();
    }

    private GameObject GetHighlightObject() //Used to find the highlights
    {
        GameObject go = highlights.Find(g => !g.activeSelf);
        if (go == null)
        {
            go = Instantiate(highlightPrefab);
            highlights.Add(go);
             
        }
        return go;
    }

    public void HighLightAllowedMoves(bool[,] moves) //this method find the available allowed highlightprefab moves
    {
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if (moves[i, j])
                {
                    GameObject go = GetHighlightObject();
                    go.SetActive(true);
                    go.transform.position = new Vector3(i+0.5f, 0, j+0.5f);

                }
            }
        }
    }

    public void Hidehighlights() //When any of the chess man is not pressed we hide the highlight
    {
        foreach (GameObject go in highlights)
            go.SetActive(false);
    }
}
