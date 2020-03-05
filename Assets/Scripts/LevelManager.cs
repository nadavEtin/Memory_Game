using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int lineAmount;
    public int columnAmount;
    
    private List<List<Vector3>> cardLinePositions;
    private List<BaseCard> allCards;
    private float cardWidth, cardHeight, columnSpace, lineSpace;

    [SerializeField]
    public Vector3 cardStartingPosition;
    [SerializeField]
    private GameObject basicCard;
    [SerializeField]
    private GameObject cardContainer;

    public Sprite[] cardFronts;

    void Start()
    {
        if (Instance == null)
            Instance = this;

        InitParams();
        SubscribeToEvents();
    }

    private void CreateLevel()
    {
        InitCardMatrix(lineAmount, columnAmount);
        InitCardMatches();
    }

    #region Level creation

    private void InitParams()
    {
        cardWidth = basicCard.GetComponent<BoxCollider2D>().size.x;
        cardHeight = basicCard.GetComponent<BoxCollider2D>().size.y;
        columnSpace = basicCard.GetComponent<BoxCollider2D>().size.x / 4;
        lineSpace = basicCard.GetComponent<BoxCollider2D>().size.y / 4;
        cardLinePositions = new List<List<Vector3>>();
        allCards = new List<BaseCard>();
    }

    //card creation and positioning 
    private void InitCardMatrix(int lineAmount, int columnAmount)
    {
        for (int i = 0; i < lineAmount; i++)
        {
            GameObject LineContainer = new GameObject();
            LineContainer.transform.SetParent(cardContainer.transform);
            LineContainer.name = "Line" + i;
            List<Vector3> newLine = new List<Vector3>();

            for (int j = 0; j < columnAmount; j++)
            {
                GameObject currentcard = Instantiate(basicCard);
                currentcard.GetComponent<BaseCard>().SetPositionInMatrix(i, j);
                currentcard.name = i + "," + j;
                if (i == 0 && j == 0)
                    currentcard.transform.position = cardStartingPosition;
                else if (j == 0 && i > 0)
                {
                    List<Vector3> previousLine = cardLinePositions[i - 1];
                    currentcard.transform.position = new Vector3(previousLine[0].x, previousLine[0].y + cardHeight + lineSpace);
                }
                else
                    currentcard.transform.position = new Vector3(newLine[newLine.Count - 1].x + cardWidth + columnSpace, newLine[newLine.Count - 1].y);

                newLine.Add(currentcard.transform.position);
                allCards.Add(currentcard.GetComponent<BaseCard>());
                currentcard.transform.SetParent(LineContainer.transform);
            }
            cardLinePositions.Add(newLine);
        }
    }

    //set card pairs randomly and assign matching sprites
    private void InitCardMatches()
    {
        if(columnAmount * lineAmount == 0)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            throw new System.Exception("Line and Column amount must be greater than 0");
        }
        else if(columnAmount * lineAmount % GameLogic.Instance.amountForMatch != 0)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            throw new System.Exception("Card amount must be an even number");
        }

        List<BaseCard> unassignedCards = new List<BaseCard>(allCards);
        int currentPairType = 0;

        while (unassignedCards.Count > 0)
        {
            if(unassignedCards.Count > 2)
            {
                //OPTIONAL: CHANGE THE LOGIC HERE TO WORK WITH ANY NUMBER OF NUMBER NEEDED TO MATCH
                int rnd = Random.Range(1, unassignedCards.Count - 1);
                unassignedCards[rnd].SetPairNumAndSprite(currentPairType);
                unassignedCards[0].SetPairNumAndSprite(currentPairType);
                unassignedCards.RemoveAt(rnd);
                unassignedCards.RemoveAt(0);
                currentPairType++;
                if (currentPairType > cardFronts.Length - 1)
                    currentPairType = 0;
            }
            else
            {
                unassignedCards[0].SetPairNumAndSprite(currentPairType);
                unassignedCards[1].SetPairNumAndSprite(currentPairType);
                unassignedCards.RemoveAt(1);
                unassignedCards.RemoveAt(0);
            }
        }
    }

    #endregion

    #region Save\Load

    private void SaveLevel()
    {
        SaveData save = SaveData.Instance;

        save.SaveGameData("line.Amount", lineAmount);
        save.SaveGameData("column.Amount", columnAmount);

        List<BaseCardDataSrlzd> binaryCardData = new List<BaseCardDataSrlzd>();

        if (allCards != null)
        {
            for (int i = 0; i < allCards.Count; i++)
            {
                //local memory + playerPrefs saving
                BaseCard currentCard = allCards[i];
                save.SaveGameData(i.ToString(), currentCard.name);
                save.SaveGameData(string.Format("{0} pairNum", currentCard.name), currentCard.pairNum);
                save.SaveGameData(string.Format("{0} imageShowing", currentCard.name), currentCard.imageShowing);
                save.SaveGameData(string.Format("{0} matchComplete", currentCard.name), currentCard.matchComplete);

                //binary saving
                BaseCardDataSrlzd bCardData = new BaseCardDataSrlzd();
                bCardData.name = currentCard.name;
                bCardData.pairNum = currentCard.pairNum;
                bCardData.imageShowing = currentCard.imageShowing;
                bCardData.matchComplete = currentCard.matchComplete;
                binaryCardData.Add(bCardData);
            }
            save.SaveGameData("memory.game", binaryCardData);
        }
    }

    private void LoadLevel()
    {
        lineAmount = LoadData.Instance.LoadIntData("lineAmount");
        columnAmount = LoadData.Instance.LoadIntData("columnAmount");

        //clear current cards before loading the saved ones
        allCards.Clear();
        foreach (Transform child in cardContainer.transform)
        {
            Destroy(child.gameObject);
        }

        

        InitCardMatrix(lineAmount, columnAmount);
        LoadCardData();
    }

    private void LoadCardData()
    {
        int cardAmount = lineAmount * columnAmount;
        if(LoadData.Instance.localMemory || LoadData.Instance.playerPrefs)
        {
            for (int i = 0; i < cardAmount; i++)
            {
                string cardName = LoadData.Instance.LoadStringData(i.ToString());
                int pairNum = LoadData.Instance.LoadIntData(string.Format("{0} pairNum", cardName));
                bool imageShowing = LoadData.Instance.LoadBoolData(string.Format("{0} imageShowing", cardName));
                bool matchComplete = LoadData.Instance.LoadBoolData(string.Format("{0} matchComplete", cardName));
                BaseCard currentCard = allCards.Find(a => a.name == cardName);
                currentCard.InitCardParams(cardName, pairNum, imageShowing, matchComplete);
                if (imageShowing && matchComplete == false)
                    GameLogic.Instance.RevealedUnresolvedCards(currentCard);
            }
        }
        else if (LoadData.Instance.binary)
        {
            List<BaseCardDataSrlzd> cardsData = LoadData.Instance.LoadBaseCardListData("memory.game");
            for (int i = 0; i < cardsData.Count; i++)
            {
                string cardName = cardsData[i].name;
                int pairNum = cardsData[i].pairNum;
                bool imageShowing = cardsData[i].imageShowing; 
                bool matchComplete = cardsData[i].matchComplete;
                BaseCard currentCard = allCards.Find(a => a.name == cardName);
                currentCard.InitCardParams(cardName, pairNum, imageShowing, matchComplete);
                if (imageShowing && matchComplete == false)
                    GameLogic.Instance.RevealedUnresolvedCards(currentCard);
            }
        }
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        EventsManager.Instance.SubscribeToEventGameStarted(CreateLevel);
        EventsManager.Instance.SubscribeToEventGameSave(SaveLevel);
        EventsManager.Instance.SubscribeToEventGameLoad(LoadLevel);
    }

    private void UnsubscribeFromEvents()
    {
        EventsManager.Instance.UnSubscribeFromsEventGameStarted(CreateLevel);
        EventsManager.Instance.UnSubscribeFromsEventGameSave(SaveLevel);
        EventsManager.Instance.UnSubscribeFromsEventGameLoad(LoadLevel);
    }
}