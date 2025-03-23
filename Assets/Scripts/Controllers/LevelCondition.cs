using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCondition : MonoBehaviour
{
    public event Action ConditionCompleteEvent = delegate { };
    public event Action ConditionFailEvent = delegate { };

    protected BoardController m_boardController;

    protected Text m_txt;

    protected bool m_conditionCompleted = false;

    

    public virtual void Setup(Text txt, BoardController board)
    {
        m_boardController = board;
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, GameManager mngr)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, BoardController board)
    {
        m_txt = txt;
        m_boardController = board;
        
    }
   

    protected virtual void UpdateText() { }

    protected virtual void OnDestroy()
    {

    }

    protected void TriggerWinEvent()
    {
        m_conditionCompleted = true;
        ConditionCompleteEvent();
    }

    protected void TriggerLoseEvent()
    {
        m_conditionCompleted = true;
        ConditionFailEvent();
    }
}
