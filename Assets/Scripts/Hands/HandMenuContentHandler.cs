using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class HandMenuContentHandler : MonoBehaviour
{
    public List<GameObject> contentCards = new List<GameObject>();
    public int cardsInList = 0;
    [SerializeField] private GameObject placeholderCard;
    [SerializeField] private GameObject contentCardPrefab;

    [SerializeField] private RectTransform ScrollBase;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addContentCard(OrgActivity content)
    {
        if (cardsInList == 0)
        {
            Destroy(placeholderCard);
        }
        cardsInList++;
        ScrollBase.sizeDelta = new Vector2(176, 90 * (cardsInList+1));
        var newCard = Instantiate(contentCardPrefab, transform);
        newCard.transform.SetAsFirstSibling();
        newCard.transform.DOScale(Vector3.zero, 0.5f).From().SetEase(Ease.InBounce);
        newCard.GetComponentInChildren<ActivityCard>().setContent(FormatTime(System.DateTime.Now),content.heading, content.text, content.iconName, content.iconColor);
    }
    public string FormatTime( DateTime time )
    {
        return string. Format("{0:00}:{1:00}:{2:00}", time.Hour, time.Minute, time.Second );
    }
    
}
