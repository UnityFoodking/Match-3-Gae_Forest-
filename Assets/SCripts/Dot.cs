using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{


    [Header("Board Variables")]
    public int column;
    public int row;
    public int TargetX;
    public int TargetY;
    public bool isMatched = false;
    public int previousColumn;
    public int previousRow;
    [Space]
    [Header("swipe Stuff")]
    public float swipeAngle = 0;
    public float swipeResist = 1.0f;
    [Space]
    [Header("бонусы")]
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public GameObject  rowArrow;
    public GameObject columnArrow;
    public GameObject ColorBomb;



    [Space]
    private FindMatched findMatches;
    private Bord8x8 board;
    public GameObject otherDots;
    private Vector2 first_touchPosition;
    private Vector2 final_touchPosition;
    private Vector2 tempPosition;


  

    private void Start()
    {

        isColumnBomb = false;
        isRowBomb = false;
        board = FindObjectOfType<Bord8x8>();
        findMatches = FindObjectOfType<FindMatched>();
    }

    /// <summary>
    /// //
    /// </summary>
    //для тестов и дебага
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isColorBomb = true;
            GameObject Gm = Instantiate(ColorBomb, transform.position, Quaternion.identity);
            Gm.transform.parent = this.transform;
        }
    }


    private void Update()
    {
        
        TargetX = column;
        TargetY = row;

        if(Mathf.Abs(TargetX-transform.position.x) > 0.1f)
        {
            //MOve Towards the target
            tempPosition = new Vector2(TargetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.1f);
            if(board.allDots[column,row]!= this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }

            findMatches.FindallMatches();
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(TargetX, transform.position.y);
            transform.position = tempPosition;
            
        }
        if (Mathf.Abs(TargetY - transform.position.y) > 0.1f)
        {
            //MOve Towards the target
            tempPosition = new Vector2(transform.position.x, TargetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.1f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindallMatches();
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(transform.position.x, TargetY);
            transform.position = tempPosition;
           
        }
    }

    public IEnumerator CheckMove()
    {
        /////передаём тэг объекта которого мы подвинули,что бы потом уничтожать объекты у которых тэги равны
        if (isColorBomb)
        {
            findMatches.MatchPieces0fColor(otherDots.tag);
            isMatched = true;
        }else if (otherDots.GetComponent<Dot>().isColorBomb)
        {
            findMatches.MatchPieces0fColor(this.gameObject.tag);
            otherDots.GetComponent<Dot>().isMatched = true;
        }
        ////

        yield return new WaitForSeconds(0.2f);
        if(otherDots != null)
        {
            if (!isMatched && !otherDots.GetComponent<Dot>().isMatched)
            {
                otherDots.GetComponent<Dot>().row = row;
                otherDots.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                board.currentDot = null;
                board.currentState = GameState.Move;
            }
            else
            {
                board.DestroyMathes();
        
            }
       
        }
       
    }


    private void OnMouseDown()
    {
        if(board.currentState == GameState.Move)
        first_touchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.Move){

            final_touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }


    }
    //функция для того что бы высчитывать куда направлена мышка
    void CalculateAngle()
     {
        if (Mathf.Abs(final_touchPosition.y - first_touchPosition.y) > swipeResist || Mathf.Abs(final_touchPosition.x - first_touchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(final_touchPosition.y - first_touchPosition.y, final_touchPosition.x - first_touchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentState = GameState.Wait;
            board.currentDot = this;
        } else
        {
            board.currentState = GameState.Move;
      
        }
    }

    //перемещение
    void MovePieces()
    {
        if(swipeAngle > - 45 && swipeAngle <= 45 && column < board.Wight )
        {
            //свап права
            otherDots = board.allDots[column + 1, row];
            previousColumn = column;
            previousRow = row;
            otherDots.GetComponent<Dot>().column -=1;
            column += 1;
        } else if(swipeAngle > 45 && swipeAngle <= 135 && row < board.Height-1)
            {
            //свайп вверх
            otherDots = board.allDots[column, row+1];
            previousColumn = column;
            previousRow = row;
            otherDots.GetComponent<Dot>().row -= 1;
            row += 1;
        }else if((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
            {
            //свайп в лево
            otherDots = board.allDots[column - 1, row];
            previousColumn = column;
            previousRow = row;
            otherDots.GetComponent<Dot>().column += 1;
            column -= 1;
        } else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //свайп вниз
            otherDots = board.allDots[column, row-1];
            previousColumn = column;
            previousRow = row;
            otherDots.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMove());
    }

    //поиск элементов для удаления
    void FindMatcheds()
    {
        if(column > 0 && column < board.Wight - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject RightDot1 = board.allDots[column + 1, row];
            if (leftDot1 != null && RightDot1 != null)
            {
                if (leftDot1.gameObject.tag == this.gameObject.tag && RightDot1.gameObject.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    RightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;

                }
            }
       
        }
        if (row > 0 && row < board.Height - 1)
        {
            GameObject upDot1 = board.allDots[column, row+1];
            GameObject downDot1 = board.allDots[column, row-1];
            if (upDot1 != null && downDot1 != null)
            {

                if (upDot1.gameObject.tag == this.gameObject.tag && downDot1.gameObject.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;

                }
            }
        }
    }

    //появление срелки
    public void makeRowBomb()
    {
        
            isRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
               
    }
    //появление срелки
    public void MakeColumnBomb()
    {
          isColumnBomb = true;
            GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        
    }


}
