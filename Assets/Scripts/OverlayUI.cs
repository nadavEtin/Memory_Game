using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverlayUI : MonoBehaviour
{
    private string winText, loseText;

    [SerializeField]
    private Button Save;
    //[SerializeField]
    //private Button Load;
    [SerializeField]
    private GameObject playButton;
    [SerializeField]
    private GameObject headlineText;
    [SerializeField]
    AudioClip winSF;
    [SerializeField]
    AudioClip loseSF;

    void Start()
    {
        winText = "You won!";
        loseText = "GameOver";
        SubscribeToEvents();
    }

    public void StartButtonClicked()
    {
        EventsManager.Instance.InvokeGameStarted();
    }

    public void ReplayButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SaveButtonClicked()
    {
        SaveData.Instance.SaveButtonClicked();
        EventsManager.Instance.InvokeGameSave();
    }

    public void LoadButtonClicked()
    {
        //test for existing data before invoking load data event
        if (LoadData.Instance.SavedDataExists())
        {
            GameLogic.Instance.ClearUnresolvedCards();
            EventsManager.Instance.InvokeGameLoad();
        }
        else
            Debug.Log("No saved data to load");
    }

    private void GameStarted()
    {
        headlineText.SetActive(false);
        playButton.SetActive(false);
        Save.interactable = true;
    }

    private void GameEnded(bool playerWon)
    {
        headlineText.GetComponent<TextMeshProUGUI>().text = playerWon ? winText : loseText;
        headlineText.SetActive(true);
        playButton.GetComponentInChildren<TextMeshProUGUI>().text = "Replay";
        playButton.GetComponent<Button>().onClick.AddListener(ReplayButtonClicked);
        playButton.SetActive(true);
        Save.interactable = false;
        GetComponent<AudioSource>().clip = playerWon ? winSF : loseSF;
        GetComponent<AudioSource>().Play();
    }

    private void SubscribeToEvents()
    {
        EventsManager.Instance.SubscribeToEventGameStarted(GameStarted);
        EventsManager.Instance.SubscribeToEventGameOver(GameEnded);
        EventsManager.Instance.SubscribeToEventGameLoad(GameStarted);
    }

    private void UnsubscribeFromEvents()
    {
        EventsManager.Instance.UnSubscribeFromsEventGameStarted(GameStarted);
        EventsManager.Instance.UnSubscribeFromsEventGameOver(GameEnded);
        EventsManager.Instance.UnSubscribeFromsEventGameLoad(GameStarted);
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}
