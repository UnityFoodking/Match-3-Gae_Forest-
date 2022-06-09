using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Wait,
    Move
}


public class Bord8x8 : MonoBehaviour
{
    public GameState currentState = GameState.Move;
    [SerializeField]
    private FindMatched findMatches;
    [Tooltip("Переменная для смещения тайлов вверх(Визуализация)")]
    public int offset_temp_position;
    public int Height;
    public int Wight;
    public GameObject Tile_prefabs;
    public GameObject[,] AllTileset;
    public GameObject[] dots;
    public GameObject[,] allDots;
    public Dot currentDot;
    public GameObject Destroy_Effect;

    void Start()
    {
        AllTileset = new GameObject[Wight, Height];
        allDots = new GameObject[Wight, Height];
        findMatches = FindObjectOfType<FindMatched>();
        GridSetUp();
    }


    public void GridSetUp()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Wight; j++)
            {
                int RandomTile = Random.Range(10, 20);
                Vector2 offset = new Vector2(i, j+ RandomTile);
                GameObject BackgroundTile = Instantiate(Tile_prefabs, offset, Quaternion.identity) as GameObject;
                BackgroundTile.transform.parent = this.transform;
                BackgroundTile.name = "(" + i + "," + j + ")";
                int index = Random.Range(0, dots.Length);
                int maxIterations = 0;
                while (MatchesAt(i, j, dots[index]) && maxIterations < 100)
                {
                    index = Random.Range(0, dots.Length);
                    maxIterations++;
                    Debug.Log(maxIterations);
                }
                maxIterations = 0;
                GameObject Dots = Instantiate(dots[index], offset, Quaternion.identity);
                Dots.GetComponent<Dot>().row = j;
                Dots.GetComponent<Dot>().column = i;
                Dots.transform.parent = this.transform;
                Dots.name = "(" + i + "," + j + ")";
                allDots[i, j] = Dots;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            ////How many elementes
            /// в листе что бы создаваь вообще срелочки
            if(findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
            {
                findMatches.CheckBombs();
            }

            findMatches.currentMatches.Remove(allDots[column, row]);
           GameObject particle =  Instantiate(Destroy_Effect,allDots[column,row].transform.position,Quaternion.identity);
            Destroy(particle, 1.5f);
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    public void DestroyMathes()
    {
        for (int i = 0; i < Wight; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }

        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < Wight; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;

                }
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }

            nullCount = 0;
        }
            yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoardCo());
    }

    private void ReliffBoard()
    {
        for (int i = 0; i < Wight; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if(allDots[i,j] == null)
                {
                    Vector2 tempPositon = new Vector2(i, j + offset_temp_position);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse],tempPositon,Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }

            }
        }
    }

    private bool MatchesOnBoard()
    {
        for(int i = 0; i <  Wight; i++)
        {
            for(int j = 0; j < Height; j++)
            {
                if(allDots[i,j]!= null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        ReliffBoard();
        yield return new WaitForSeconds(0.5f);
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyMathes();
        }
        /////
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(0.5f);
        currentState = GameState.Move;
    }


}
