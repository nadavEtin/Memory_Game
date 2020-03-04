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
    //private bool cardMatrixInitialized;

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
        //cardMatrixInitialized = true;
    }

    private void InitCardMatches()
    {
        if(columnAmount * lineAmount % 2 != 0)
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

        save.SaveGameData("lineAmount", lineAmount);
        save.SaveGameData("columnAmount", columnAmount);

        if (allCards != null)
        {
            for (int i = 0; i < allCards.Count; i++)
            {
                BaseCard currentCard = allCards[i];
                save.SaveGameData(i.ToString(), currentCard.name);
                save.SaveGameData(string.Format("{0} line", currentCard.name), currentCard.line);
                save.SaveGameData(string.Format("{0} column", currentCard.name), currentCard.column);
                save.SaveGameData(string.Format("{0} pairNum", currentCard.name), currentCard.pairNum);
                save.SaveGameData(string.Format("{0} imageShowing", currentCard.name), currentCard.imageShowing);
                save.SaveGameData(string.Format("{0} matchComplete", currentCard.name), currentCard.matchComplete);
            }
        }
    }

    private void LoadLevel()
    {
        lineAmount = LoadData.Instance.LoadIntData("lineAmount");
        columnAmount = LoadData.Instance.LoadIntData("columnAmount");
        //no data to load
        if(lineAmount == 0 || columnAmount == 0)
        {
            Debug.Log("There's no data to load");
            return;
        }

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
        //List<BaseCard> loadedCards = new List<BaseCard>();
        int cardAmount = lineAmount * columnAmount;
        for (int i = 0; i < cardAmount; i++)
        {
            string cardName = LoadData.Instance.LoadStringData(i.ToString());
            int cardLine = LoadData.Instance.LoadIntData(string.Format("{0} line", cardName));
            int cardColumn = LoadData.Instance.LoadIntData(string.Format("{0} column", cardName));
            int pairNum = LoadData.Instance.LoadIntData(string.Format("{0} pairNum", cardName));
            bool imageShowing = LoadData.Instance.LoadBoolData(string.Format("{0} imageShowing", cardName));
            bool matchComplete = LoadData.Instance.LoadBoolData(string.Format("{0} matchComplete", cardName));
            allCards[i].GetComponent<BaseCard>().InitCardParams(cardName, pairNum, cardLine, cardColumn, imageShowing, matchComplete);
            if (imageShowing)
                //allCards[i].GetComponent<BaseCard>().ChangeImage(1);
                allCards[i].GetComponent<Animator>().SetTrigger("Start");
            if (imageShowing == true && matchComplete == false)
                GameLogic.Instance.RevealedUnresolvedCards(allCards[i]);

            //loadedCards.Add(currentCard);
        }
        //allCards = loadedCards;
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