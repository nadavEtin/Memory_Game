using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager Instance;

    public delegate void SimpleActionEvent();
    public event SimpleActionEvent gameStart;
    public event SimpleActionEvent gameSave;
    public event SimpleActionEvent gameLoad;

    public delegate void CardClickedEvent(GameObject obj);
    public event CardClickedEvent cardClicked;

    public delegate void BoolReturnEvent(bool result);
    public event BoolReturnEvent preventClick;
    public event BoolReturnEvent gameOver;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    //OPTIONAL: MAKE GENRIC FUNC FOR THESE
    public void SubscribeToEventCardRevealed(CardClickedEvent obj)
    {
        cardClicked += obj;
    }

    public void UnSubscribeFromsEventCardRevealed(CardClickedEvent obj)
    {
        cardClicked -= obj;
    }

    public void InvokeCardRevealed(GameObject obj)
    {
        cardClicked?.Invoke(obj);
    }

    public void SubscribeToEventPreventCardClick(BoolReturnEvent obj)
    {
        preventClick += obj;
    }

    public void UnSubscribeFromsEventPreventCardClick(BoolReturnEvent obj)
    {
        preventClick -= obj;
    }

    public void InvokePreventCardClick(bool preventing)
    {
        preventClick?.Invoke(preventing);
    }

    public void SubscribeToEventGameStarted(SimpleActionEvent obj)
    {
        gameStart += obj;
    }

    public void UnSubscribeFromsEventGameStarted(SimpleActionEvent obj)
    {
        gameStart -= obj;
    }

    public void InvokeGameStarted()
    {
        gameStart?.Invoke();
    }

    public void SubscribeToEventGameSave(SimpleActionEvent obj)
    {
        gameSave += obj;
    }

    public void UnSubscribeFromsEventGameSave(SimpleActionEvent obj)
    {
        gameSave -= obj;
    }

    public void InvokeGameSave()
    {
        gameSave?.Invoke();
    }

    public void SubscribeToEventGameLoad(SimpleActionEvent obj)
    {
        gameLoad += obj;
    }

    public void UnSubscribeFromsEventGameLoad(SimpleActionEvent obj)
    {
        gameLoad -= obj;
    }

    public void InvokeGameLoad()
    {
        gameLoad?.Invoke();
    }

    public void SubscribeToEventGameOver(BoolReturnEvent obj)
    {
        gameOver += obj;
    }

    public void UnSubscribeFromsEventGameOver(BoolReturnEvent obj)
    {
        gameOver -= obj;
    }

    public void InvokeGameOver(bool playerWon)
    {
        gameOver?.Invoke(playerWon);
    }
}
