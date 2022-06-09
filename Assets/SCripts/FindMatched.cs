using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;///

public class FindMatched : MonoBehaviour
{
    [SerializeField]
    private Bord8x8 Board;
    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        Board = FindObjectOfType<Bord8x8>();
       
    }

    public void FindallMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < Board.Wight; i++){
            for (int j = 0; j < Board.Wight; j++)
            {
                GameObject currentDot = Board.allDots[i, j];
                if(currentDot != null)
                {
                    if(i  > 0 && i<Board.Wight -1)
                    {
                        GameObject leftDot = Board.allDots[i - 1, j];
                        GameObject rigthDot = Board.allDots[i + 1, j];
                        if(leftDot != null && rigthDot != null)
                        {

                            if (leftDot.tag == currentDot.tag && rigthDot.tag == currentDot.tag)
                            {
                                if(currentDot.GetComponent<Dot>().isRowBomb || leftDot.GetComponent<Dot>().isRowBomb || rigthDot.GetComponent<Dot>().isRowBomb)

                                {
                                    currentMatches.Union(GetRowPieces(j));
                                }

                                if (currentDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetcolumnPieces(i));
                                }


                                if (leftDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetcolumnPieces(i-1));
                                }


                                if (rigthDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetcolumnPieces(i + 1));
                                }

                                if (!currentMatches.Contains(leftDot))
                                {
                                    currentMatches.Add(leftDot);
                                }
                                leftDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(rigthDot))
                                {
                                    currentMatches.Add(rigthDot);
                                }
                                rigthDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;

                            }
                        }
                    }

                    if (j > 0 && j < Board.Height - 1)
                    {
                        GameObject UpDot = Board.allDots[i, j+1];
                        GameObject DownDot = Board.allDots[i, j-1];
                        if (UpDot != null && DownDot != null)
                        {
                            if (UpDot.tag == currentDot.tag && DownDot.tag == currentDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().isColumnBomb || UpDot.GetComponent<Dot>().isColumnBomb || DownDot.GetComponent<Dot>().isColumnBomb)

                                {
                                    currentMatches.Union(GetcolumnPieces(i));
                                }

                                if (currentDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j));
                                }

                                if (UpDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j+1));
                                }

                                if (DownDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j-1));
                                }

                                if (!currentMatches.Contains(UpDot))
                                {
                                    currentMatches.Add(UpDot);
                                }
                                UpDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(DownDot))
                                {
                                    currentMatches.Add(DownDot);
                                }
                                DownDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;

                            }
                        }
                    }


                }
            }
        }
    }
    
    /// //////////////
    public void MatchPieces0fColor(string color)
    {
        for (int i = 0; i < Board.Wight; i++)
        {
            for (int j = 0; j < Board.Height; j++)
            {
                if(Board.allDots[i,j]!= null)
                {
                    if(Board.allDots[i,j].tag == color)
                    {
                        Board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    //запись объектов в список которые нужно уничтожить по колонкам
    List<GameObject> GetcolumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();

        for(int i = 0; i< Board.Height; i++)
        {
            if(Board.allDots[column,i] != null)
            {
                dots.Add(Board.allDots[column, i]);
                Board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }


        return dots;
    }
    //тоже самое только для столбцов
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = 0; i < Board.Wight; i++)
        {
            if (Board.allDots[i, row] != null)
            {
                dots.Add(Board.allDots[i, row]);
                Board.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }


        return dots;
    }

    //появление объектов стрелки для уничтожения
    public void CheckBombs()
    {
        if(Board.currentDot != null)
        {
            if (Board.currentDot.isMatched)
            {
                Board.currentDot.isMatched = false;
                /*
                int typeofBomb = Random.Range(0, 100);
                if(typeofBomb < 50)
                {
                    Board.currentDot.makeRowBomb();
                }else if(typeofBomb >= 50)
                {
                    Board.currentDot.MakeColumnBomb();
                }
                */


                if((Board.currentDot.swipeAngle > -45 && Board.currentDot.swipeAngle <= 45)
                    || (Board.currentDot.swipeAngle < -135 || Board.currentDot.swipeAngle >= 135))
                {
                    Board.currentDot.makeRowBomb();
                } else
                {
                    Board.currentDot.MakeColumnBomb();
                }
            } else if (Board.currentDot.otherDots != null)
            {
                Dot otherDot = Board.currentDot.GetComponent<Dot>().otherDots.GetComponent<Dot>();
                if (otherDot.isMatched)
                {
                    otherDot.isMatched = false;

                    if ((Board.currentDot.swipeAngle > -45 && Board.currentDot.swipeAngle <= 45)
                   || (Board.currentDot.swipeAngle < -135 || Board.currentDot.swipeAngle >= 135))
                    {
                        otherDot.makeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }

                    /*
                    if ((otherDot.swipeAngle > -45 && otherDot.swipeAngle <= 45)
                     || (otherDot.swipeAngle < -135 || otherDot.swipeAngle >= 135))
                    {
                        otherDot.makeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }


                    
                    int typeofBomb = Random.Range(0, 100);
                    if (typeofBomb < 50)
                    {
                        otherDot.makeRowBomb();
                    }
                    else if (typeofBomb >= 50)
                    {
                        otherDot.MakeColumnBomb();
                    }
                    */
                }
            }


        }
    }

}
