using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseCard : MonoBehaviour
{
    public int pairNum { private set; get; }
    public int line { private set; get; }
    public int column { private set; get; }
    public bool imageShowing { private set; get; }
    public bool matchComplete;

    private bool preventReveal;

    [SerializeField]
    private GameObject cardBack;
    [SerializeField]
    private GameObject cardFrame;
    [SerializeField]
    private GameObject cardFront;

    //movement
    //private bool isMoving;
    //private Vector3 destinationPosition;
    //public float movementSpeed;

    public void InitCardParams(string name, int pairNum, int line, int column, bool imageShowing, bool matchComplete)
    {
        gameObject.name = name;
        SetPairNumAndSprite(pairNum);
        SetPositionInMatrix(line, column);
        this.imageShowing = imageShowing;
        this.matchComplete = matchComplete;
    }

    private void Start()
    {
        EventsManager.Instance.SubscribeToEventPreventCardClick(PreventCardReveal);
    }

    private void ResetCard()
    {
        cardBack.SetActive(true);
        cardFrame.SetActive(false);
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

    //public void FlipAnimationOver(int backToFront)
    //{
    //    gameObject.GetComponent<Animator>().enabled = false;
    //    if (backToFront == 1)
    //        ChangeImage(true);
    //    else
    //        ChangeImage(false);
    //}

    public void ChangeImage(bool reveal)
    {
        //OPTIONAL: ADD ANIMATIONS FOR CARD TURN AROUND
        if(reveal)
        {
            cardBack.SetActive(false);
            cardFrame.SetActive(true);
            imageShowing = true;
            transform.GetComponentInParent<AudioSource>().Play();
            //gameObject.GetComponent<Animator>().SetTrigger("Start");
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
            //gameObject.GetComponent<Animator>().enabled = true;
            //gameObject.GetComponent<Animator>().SetTrigger("Start");
            EventsManager.Instance.InvokeCardRevealed(gameObject);
        }
    }
}
