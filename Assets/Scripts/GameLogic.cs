using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;

    private int amountForMatch, matchesMade;
    private float tickingSoundStartTime;
    private bool stopTimer;
    private List<BaseCard> revealedUnresolvedCards;

    [SerializeField]
    private TextMeshProUGUI gameTimer;
    [SerializeField]
    private int gameTime;

    void Start()
    {
        if (Instance == null)
            Instance = this;

        revealedUnresolvedCards = new List<BaseCard>();
        amountForMatch = 2;
        tickingSoundStartTime = 10;
        SubscribeToEvents();
    }
    
    public void StartGame()
    {
        EventsManager.Instance.InvokePreventCardClick(false);
        gameTimer.text = gameTime.ToString();
        StartCoroutine("RunGameTime");
    }

    private void GameEnded(bool playerWon)
    {
        if (playerWon == true)
            InteractWithTimer(true);
        GetComponent<AudioSource>().Stop();
        EventsManager.Instance.InvokePreventCardClick(true);
    }

    private void SaveGameTime()
    {
        SaveData.Instance.SaveGameData("gameTime", gameTime);
    }

    private void LoadGameData()
    {
        gameTime = LoadData.Instance.LoadIntData("gameTime");
        gameTimer.text = gameTime.ToString();
        StopCoroutine("RunGameTime");
        StartCoroutine("RunGameTime");
    }

    private void InteractWithTimer(bool shouldStop)
    {
        stopTimer = shouldStop;
    }

    private IEnumerator RunGameTime()
    {
        while (gameTime > 0 && stopTimer == false)
        {
            yield return new WaitForSeconds(1);
            gameTime--;
            gameTimer.text = gameTime.ToString();
            if (GetComponent<AudioSource>().isPlaying == false && gameTime <= 10)
                GetComponent<AudioSource>().Play();
        }

        if(gameTime <= 0)
        {
            gameTimer.text = "Time's up!";
            EventsManager.Instance.InvokeGameOver(false);
        }
    }

    private void CardRevealed(GameObject obj)
    {
        BaseCard baseCard = obj.GetComponent<BaseCard>();
        RevealedUnresolvedCards(baseCard);
        if (revealedUnresolvedCards.Count >= amountForMatch)
            CheckMatches();
    }

    public void RevealedUnresolvedCards(BaseCard card)
    {
        //card.ChangeImage(true);
        revealedUnresolvedCards.Add(card);
    }

    public void ClearUnresolvedCards()
    {
        revealedUnresolvedCards.Clear();
    }

    private void CheckMatches()
    {
        //OPTIONAL: EXTEND THE MATCHING TO MORE THAN 2, MAKE IT GENERIC
        //not a match
        if (revealedUnresolvedCards[0].pairNum.Equals(revealedUnresolvedCards[1].pairNum) == false)
            StartCoroutine(FailedMatch());
        else
        {
            for (int i = 0; i < revealedUnresolvedCards.Count; i++)
            {
                revealedUnresolvedCards[i].matchComplete = true;
            }
            revealedUnresolvedCards.Clear();
            matchesMade++;
            if (matchesMade == LevelManager.Instance.columnAmount * LevelManager.Instance.lineAmount / amountForMatch)
                EventsManager.Instance.InvokeGameOver(true);
        }
    }

    private IEnumerator FailedMatch()
    {
        //OTIONAL: SOLVE LOADING WHILE THIS TIMER IS RUNNING
        EventsManager.Instance.InvokePreventCardClick(true);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < revealedUnresolvedCards.Count; i++)
        {
            revealedUnresolvedCards[i].ChangeImage(false);
        }

        EventsManager.Instance.InvokePreventCardClick(false);
        revealedUnresolvedCards.Clear();
    }

    private void SubscribeToEvents()
    {
        EventsManager.Instance.SubscribeToEventCardRevealed(CardRevealed);
        EventsManager.Instance.SubscribeToEventGameSave(SaveGameTime);
        EventsManager.Instance.SubscribeToEventGameLoad(LoadGameData);
        EventsManager.Instance.SubscribeToEventGameStarted(StartGame);
        EventsManager.Instance.SubscribeToEventGameOver(GameEnded);
    }

    private void UnsubscribeFromEvents()
    {
        EventsManager.Instance.UnSubscribeFromsEventCardRevealed(CardRevealed);
        EventsManager.Instance.UnSubscribeFromsEventGameSave(SaveGameTime);
        EventsManager.Instance.UnSubscribeFromsEventGameLoad(LoadGameData);
        EventsManager.Instance.UnSubscribeFromsEventGameStarted(StartGame);
        EventsManager.Instance.UnSubscribeFromsEventGameOver(GameEnded);
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}
