﻿using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<eStateGame> StateChangedAction = delegate { };

    public enum eLevelMode
    {
        NORMAL,
        AUTO_PLAY,
        AUTO_LOSE,
        TIME_ATTACK
    }

    public enum eStateGame
    {
        SETUP,
        MAIN_MENU,
        GAME_STARTED,
        PAUSE,
        GAME_WIN,
        GAME_OVER
    }

    private eStateGame m_state;
    public eStateGame State
    {
        get { return m_state; }
        private set
        {
            m_state = value;

            StateChangedAction(m_state);
        }
    }


    private GameSettings m_gameSettings;


    private BoardController m_boardController;

    private UIMainManager m_uiMenu;

    private LevelCondition m_levelCondition;

    private void Awake()
    {
        State = eStateGame.SETUP;

        m_gameSettings = Resources.Load<GameSettings>(Constants.GAME_SETTINGS_PATH);

        m_uiMenu = FindObjectOfType<UIMainManager>();
        m_uiMenu.Setup(this);
    }

    void Start()
    {
        State = eStateGame.MAIN_MENU;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_boardController != null) m_boardController.Update();
    }


    internal void SetState(eStateGame state)
    {
        State = state;

        if(State == eStateGame.PAUSE)
        {
            DOTween.PauseAll();
        }
        else
        {
            DOTween.PlayAll();
        }
    }

    public void LoadLevel(eLevelMode mode)
    {
        m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
        m_boardController.StartGame(this, m_gameSettings);

        if (mode == eLevelMode.NORMAL)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelNormal>();
            m_levelCondition.Setup(m_uiMenu.GetLevelConditionView(), m_boardController);
        }
        else if (mode == eLevelMode.TIME_ATTACK)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelTimeAttack>();
            m_levelCondition.Setup(m_gameSettings.LevelTime, m_uiMenu.GetLevelConditionView(), this);
        }

        m_levelCondition.ConditionCompleteEvent += GameWin;
        m_levelCondition.ConditionFailEvent += GameLose;
        State = eStateGame.GAME_STARTED;
    }

    public void GameWin()
    {
        StartCoroutine(WaitBoardController(eStateGame.GAME_WIN));
    }

    public void GameLose()
    {
        StartCoroutine(WaitBoardController(eStateGame.GAME_OVER));
    }    

    internal void ClearLevel()
    {
        if (m_boardController)
        {
            m_boardController.Clear();
            Destroy(m_boardController.gameObject);
            m_boardController = null;
        }
    }

    private IEnumerator WaitBoardController(eStateGame endState)
    {
        while (m_boardController.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);

        State = endState;

        if (m_levelCondition != null)
        {
            m_levelCondition.ConditionCompleteEvent -= GameWin;
            m_levelCondition.ConditionFailEvent -= GameLose;

            Destroy(m_levelCondition);
            m_levelCondition = null;
        }
    }
}
