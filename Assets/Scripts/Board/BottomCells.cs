using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BottomCells
{
    public enum eMatchDirection
    {
        HORIZONTAL
    }

    private int bottomCellsSizeX;

    private int bottomCellsSizeY;

    private Cell[,] m_cells;

    private Transform m_root;

    private int m_matchMin;

    public BottomCells(Transform transform, GameSettings gameSettings)
    {
        m_root = transform;

        m_matchMin = gameSettings.MatchesMin;

        this.bottomCellsSizeX = gameSettings.BottomCellsSizeX;
        this.bottomCellsSizeY = gameSettings.BottomCellsSizeY;

        m_cells = new Cell[bottomCellsSizeX, bottomCellsSizeY];
        CreateCells(gameSettings);
    }

    private void CreateCells(GameSettings gameSettings)
    {
        Vector3 origin = new Vector3(-bottomCellsSizeX * 0.5f + 0.5f, -bottomCellsSizeY * 0.5f - gameSettings.BoardAndBottomCellsOffset, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int x = 0; x < bottomCellsSizeX; x++)
        {
            for (int y = 0; y < bottomCellsSizeY; y++)
            {
                GameObject go = GameObject.Instantiate(prefabBG);
                go.transform.position = origin + new Vector3(x, y, 0f);
                go.transform.SetParent(m_root);

                Cell cell = go.GetComponent<Cell>();
                cell.Setup(x, y);

                m_cells[x, y] = cell;
            }
        }

        //set neighbours
        for (int x = 0; x < bottomCellsSizeX; x++)
        {
            for (int y = 0; y < bottomCellsSizeY; y++)
            {
                if (y + 1 < bottomCellsSizeY) m_cells[x, y].NeighbourUp = m_cells[x, y + 1];
                if (x + 1 < bottomCellsSizeX) m_cells[x, y].NeighbourRight = m_cells[x + 1, y];
                if (y > 0) m_cells[x, y].NeighbourBottom = m_cells[x, y - 1];
                if (x > 0) m_cells[x, y].NeighbourLeft = m_cells[x - 1, y];
            }
        }

    }

    public void RemoveMatchingItems()
    {
        for (int y = 0; y < bottomCellsSizeY; y++)
        {
            List<Cell> matchedCells = new List<Cell>();
            for (int x = 0; x < bottomCellsSizeX; x++)
            {
                Cell cell = m_cells[x, y];

                matchedCells = GetHorizontalMatches(cell);

                if (matchedCells.Count == 3)
                {
                    foreach (Cell matchedCell in matchedCells)
                    {
                        matchedCell.ExplodeItem();
                    }
                }
            }
        }
        ShiftBottomCells();
    }

    public void ShiftBottomCells()
    {
        List<Cell> allCells = GetCells();
        List<Item> items = new List<Item>();

        foreach (Cell cell in allCells)
        {
            if (!cell.IsEmpty)
            {
                items.Add(cell.Item);
                cell.Free(); 
            }
        }

        for (int i = 0; i < items.Count; i++)
        {
            allCells[i].Assign(items[i]);
            allCells[i].ApplyItemMoveToPosition();
        }
    }

    public List<Cell> GetHorizontalMatches(Cell cell)
    {
        List<Cell> list = new List<Cell>();
        list.Add(cell);

        //check horizontal match
        Cell newcell = cell;
        while (true)
        {
            Cell neib = newcell.NeighbourRight;
            if (neib == null) break;

            if (neib.IsSameType(cell))
            {
                list.Add(neib);
                newcell = neib;
            }
            else break;
        }

        newcell = cell;
        while (true)
        {
            Cell neib = newcell.NeighbourLeft;
            if (neib == null) break;

            if (neib.IsSameType(cell))
            {
                list.Add(neib);
                newcell = neib;
            }
            else break;
        }

        return list;
    }
    public List<Cell> GetCells()
    {
        List<Cell> allCells = new List<Cell>();

        for (int x = 0; x < bottomCellsSizeX; x++)
        {
            for (int y = 0; y < bottomCellsSizeY; y++)
            {
                allCells.Add(m_cells[x, y]);
            }
        }

        return allCells;
    }

}
