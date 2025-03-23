using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelNormal : LevelCondition
{
    private BoardController m_board;
    private Text m_text;

    public override void Setup(Text txt, BoardController board)
    {
        base.Setup(txt, board);

        m_board = board;
        m_text = txt;   
    }

    private void Update()
    {
        if (m_conditionCompleted || m_boardController == null) return;

        if (CheckWinCondition())
        {
            TriggerWinEvent();
        }
        else if (CheckLoseCondition())
        {
            TriggerLoseEvent();
        }

    }
    protected virtual bool CheckWinCondition()
    {
        return m_board.BottomCellsEmpty() && m_board.MainBoardEmpty();
    }

    protected virtual bool CheckLoseCondition()
    {
        return m_board.BottomCellsFull();
    }
}
