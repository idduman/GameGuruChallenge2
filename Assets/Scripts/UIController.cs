using System.Collections;
using System.Collections.Generic;
using GameGuruChallenge;
using UnityEngine;

public class UIController : SingletonBehaviour<UIController>
{
    [SerializeField] private RectTransform _startPanel;
    [SerializeField] private RectTransform _successPanel;
    [SerializeField] private RectTransform _failPanel;

    public void ActivateEndgamePanel(bool success)
    {
        var panel = success ? _successPanel : _failPanel;

        panel.gameObject.SetActive(true);
    }

    public void Initialize()
    {
        _successPanel.gameObject.SetActive(false);
        _failPanel.gameObject.SetActive(false);
        _startPanel.gameObject.SetActive(true);
    }

    public void RestartLevel()
    {
        GameManager.Instance.EndLevel(false);
    }

    public void NextLevel()
    {
        GameManager.Instance.EndLevel(true);
    }

    public void ToggleStartPanel(bool active)
    {
        _startPanel.gameObject.SetActive(active);
    }
}
