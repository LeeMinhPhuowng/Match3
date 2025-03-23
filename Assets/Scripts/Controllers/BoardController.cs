using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnMoveEvent = delegate { };

    public bool IsBusy { get; private set; }

    private Board m_board;

    private BottomCells m_bottomCells;

    private GameManager m_gameManager;

//  private bool m_isDragging;

    private Camera m_cam;

    private Collider2D m_hitCollider;

    private GameSettings m_gameSettings;

    private List<Cell> m_potentialMatch;

    private float m_timeAfterFill;

    private bool m_hintIsShown;

    private bool m_gameOver;

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;

        m_gameSettings = gameSettings;

        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;

        m_board = new Board(this.transform, gameSettings);

        m_bottomCells = new BottomCells(this.transform, gameSettings);

        Fill();
    }

    private void Fill()
    {
        m_board.Fill();
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_WIN:
                m_gameOver = true;
                StopHints();
                break;
            case GameManager.eStateGame.GAME_OVER:
                m_gameOver = true;
                StopHints();
                break;
        }
    }


    public void Update()
    {
        if (m_gameOver) return;
        if (IsBusy) return;

        if (!m_hintIsShown)
        {
            m_timeAfterFill += Time.deltaTime;
            if (m_timeAfterFill > m_gameSettings.TimeForHint)
            {
                m_timeAfterFill = 0f;
                ShowHint();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
//              m_isDragging = true;
                Cell chosenCell = hit.collider.GetComponent<Cell>();
                if(chosenCell != null)
                {
                    MoveItemToBottomCells(chosenCell);
                    m_bottomCells.RemoveMatchingItems();
                }    
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetRayCast();
        }
    }

    private void MoveItemToBottomCells(Cell cell)
    {
        if (cell.IsEmpty) return;

        Item itemToMove = cell.Item;
        List<Cell> bottomCellsList = m_bottomCells.GetCells(); // Lấy danh sách các ô trong BottomCells
        int itemCount = bottomCellsList.Count(cell => !cell.IsEmpty);
       
        if (itemCount == m_gameSettings.BottomCellsSizeX) return;

        Cell targetCell = null;
        int lastSameTypeIndex = -1;

        for (int i = 0; i < bottomCellsList.Count; i++)
        {
            if (bottomCellsList[i].IsEmpty && targetCell == null)
            {
                targetCell = bottomCellsList[i];
            }

            if (!bottomCellsList[i].IsEmpty && bottomCellsList[i].Item.IsSameType(itemToMove))
            {
                lastSameTypeIndex = i; 
            }
        }

        if (lastSameTypeIndex != -1)
        {
            targetCell = lastSameTypeIndex + 1 < bottomCellsList.Count ? bottomCellsList[lastSameTypeIndex + 1] : null;
        }

        if (targetCell != null)
        {
            for (int i = bottomCellsList.Count - 1; i > targetCell.BoardX; i--)
            {
                if (!bottomCellsList[i - 1].IsEmpty && !bottomCellsList[i - 1].Item.IsSameType(itemToMove))
                {
                    bottomCellsList[i].Assign(bottomCellsList[i - 1].Item);
                    bottomCellsList[i].ApplyItemMoveToPosition();
                    bottomCellsList[i - 1].Free();
                }
            }

            targetCell.Assign(itemToMove);
            targetCell.ApplyItemMoveToPosition();
            cell.Free();
        }
    }

    private void ResetRayCast()
    {
        m_hitCollider = null;
    }
/*
    private void FindMatchesAndCollapse(Cell cell1, Cell cell2)
    {
        if (cell1.Item is BonusItem)
        {
            cell1.ExplodeItem();
            StartCoroutine(ShiftDownItemsCoroutine());
        }
        else if (cell2.Item is BonusItem)
        {
            cell2.ExplodeItem();
            StartCoroutine(ShiftDownItemsCoroutine());
        }
        else
        {
            List<Cell> cells1 = GetMatches(cell1);
            List<Cell> cells2 = GetMatches(cell2);

            List<Cell> matches = new List<Cell>();
            matches.AddRange(cells1);
            matches.AddRange(cells2);
            matches = matches.Distinct().ToList();

            if (matches.Count < m_gameSettings.MatchesMin)
            {
                m_board.Swap(cell1, cell2, () =>
                {
                    IsBusy = false;
                });
            }
            else
            {
                OnMoveEvent();

                CollapseMatches(matches, cell2);
            }
        }
    }

    private void FindMatchesAndCollapse()
    {
        List<Cell> matches = m_board.FindFirstMatch();

        if (matches.Count > 0)
        {
            CollapseMatches(matches, null);
        }
        else
        {
            m_potentialMatch = m_board.GetPotentialMatches();
            if (m_potentialMatch.Count > 0)
            {
                IsBusy = false;

                m_timeAfterFill = 0f;
            }
            else
            {
                StartCoroutine(ShuffleBoardCoroutine());
            }
        }
    }

    private List<Cell> GetMatches(Cell cell)
    {
        List<Cell> listHor = m_board.GetHorizontalMatches(cell);
        if (listHor.Count < m_gameSettings.MatchesMin)
        {
            listHor.Clear();
        }

        List<Cell> listVert = m_board.GetVerticalMatches(cell);
        if (listVert.Count < m_gameSettings.MatchesMin)
        {
            listVert.Clear();
        }

        return listHor.Concat(listVert).Distinct().ToList();
    }
*/
/*
    private void CollapseMatches(List<Cell> matches, Cell cellEnd)
    {
        for (int i = 0; i < matches.Count; i++)
        {
            matches[i].ExplodeItem();
        }

        if(matches.Count > m_gameSettings.MatchesMin)
        {
            m_board.ConvertNormalToBonus(matches, cellEnd);
        }

        StartCoroutine(ShiftDownItemsCoroutine());
    }

    private IEnumerator ShiftDownItemsCoroutine()
    {
        m_board.ShiftDownItems();

        yield return new WaitForSeconds(0.2f);

        m_board.FillGapsWithNewItems();

        yield return new WaitForSeconds(0.2f);

        FindMatchesAndCollapse();
    }

    private IEnumerator RefillBoardCoroutine()
    {
        m_board.ExplodeAllItems();

        yield return new WaitForSeconds(0.2f);

        m_board.Fill();

        yield return new WaitForSeconds(0.2f);

        FindMatchesAndCollapse();
    }

    private IEnumerator ShuffleBoardCoroutine()
    {
        m_board.Shuffle();

        yield return new WaitForSeconds(0.3f);

        FindMatchesAndCollapse();
    }
*/

    private void SetSortingLayer(Cell cell1, Cell cell2)
    {
        if (cell1.Item != null) cell1.Item.SetSortingLayerHigher();
        if (cell2.Item != null) cell2.Item.SetSortingLayerLower();
    }

    private bool AreItemsNeighbor(Cell cell1, Cell cell2)
    {
        return cell1.IsNeighbour(cell2);
    }

    internal void Clear()
    {
        m_board.Clear();
    }

    private void ShowHint()
    {
        m_hintIsShown = true;
        foreach (var cell in m_potentialMatch)
        {
            cell.AnimateItemForHint();
        }
    }

    private void StopHints()
    {
        m_hintIsShown = false;
        foreach (var cell in m_potentialMatch)
        {
            cell.StopHintAnimation();
        }

        m_potentialMatch.Clear();
    }

    public bool BottomCellsEmpty()
    {
        List<Cell> bottomCellsList = m_bottomCells.GetCells();
        int countItems = bottomCellsList.Count(cell => !cell.IsEmpty);
        return countItems == 0;
    }

    public bool BottomCellsFull()
    {
        List<Cell> bottomCellsList = m_bottomCells.GetCells();
        int countItems = bottomCellsList.Count(cell => !cell.IsEmpty);
        return countItems == m_gameSettings.BottomCellsSizeX;
    }

    public bool MainBoardEmpty()
    {
        List<Cell> boardCellsList = m_board.GetCells();
        int countItems = boardCellsList.Count(cell => !cell.IsEmpty);
        return countItems == 0;
    }

}
