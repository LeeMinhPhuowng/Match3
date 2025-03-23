using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelMain : MonoBehaviour, IMenu
{
    [SerializeField] private Button btnTimeAttack;

    [SerializeField] private Button btnNormal;

    [SerializeField] private Button btnAutoPlay;

    [SerializeField] private Button btnAutoLose;

    private UIMainManager m_mngr;

    private void Awake()
    {
        btnNormal.onClick.AddListener(OnClickNormal);
        btnTimeAttack.onClick.AddListener(OnClickTimeAttack);
        btnAutoPlay.onClick.AddListener(OnClickAutoPlay);
        btnAutoLose.onClick.AddListener(OnClickAutoLose);
    }

    private void OnDestroy()
    {
        if (btnNormal) btnNormal.onClick.RemoveAllListeners();
        if (btnTimeAttack) btnTimeAttack.onClick.RemoveAllListeners();
        if (btnAutoPlay) btnAutoPlay.onClick.RemoveAllListeners();
        if (btnAutoLose) btnAutoLose.onClick.RemoveAllListeners();
    }

    public void Setup(UIMainManager mngr)
    {
        m_mngr = mngr;
    }

    private void OnClickTimeAttack()
    {
        m_mngr.LoadLevelTimeAttack();
    }

    private void OnClickNormal()
    {
        m_mngr.LoadLevelNormal();
    }

    private void OnClickAutoPlay()
    {
        m_mngr.LoadLevelAutoPlay();
    }

    private void OnClickAutoLose()
    {
        m_mngr.LoadLevelAutoLose();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
