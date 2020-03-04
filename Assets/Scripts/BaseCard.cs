using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BaseCard : MonoBehaviour
{
    public int pairNum { private set; get; }
    public int line { private set; get; }
    public int column { private set; get; }
    public bool imageShowing { private set; get; }
    [HideInInspector]
    public bool matchComplete;

    private bool preventReveal;

    [SerializeField]
    private GameObject cardBack;
    [SerializeField]
    private GameObject cardFrame;
    [SerializeField]
    private GameObject cardFront;

    public void InitCardParams(string name, int pairNum, bool imageShowing, bool matchComplete)
    {
        gameObject.name = name;
        string[] position = name.Split(',');
        SetPositionInMatrix(int.Parse(position[0]), int.Parse(position[1]));
        SetPairNumAndSprite(pairNum);
        this.imageShowing = imageShowing;
        if (imageShowing)
            ChangeImage(true);
        this.matchComplete = matchComplete;
    }

    private void Start()
    {
        EventsManager.Instance.SubscribeToEventPreventCardClick(PreventCardReveal);
    }

    public void SetPositionInMatrix(int line, int column)
    {
        this.line = line;
        this.column = column;
    }

    public void SetPairNumAndSprite(int num)
    {
        pairNum = num;
        cardFront.GetComponent<SpriteRenderer>().sprite = LevelManager.Instance.cardFronts[num];
    }

    public void ChangeImage(bool reveal)
    {
        //OPTIONAL: ADD ANIMATIONS FOR CARD TURN AROUND
        if(reveal)
        {
            cardBack.SetActive(false);
            cardFrame.SetActive(true);
            imageShowing = true;
            transform.GetComponentInParent<AudioSource>().Play();
        }
        else
        {
            cardBack.SetActive(true);
            cardFrame.SetActive(false);
            imageShowing = false;
        }
    }

    private void PreventCardReveal(bool preventing)
    {
        preventReveal = preventing;
    }

    private void OnMouseDown()
    {
        if(imageShowing == false && preventReveal == false)
        {
            ChangeImage(true);
            EventsManager.Instance.InvokeCardRevealed(gameObject);
        }
    }

    private void OnDestroy()
    {
        EventsManager.Instance.UnSubscribeFromsEventPreventCardClick(PreventCardReveal);
    }
}
