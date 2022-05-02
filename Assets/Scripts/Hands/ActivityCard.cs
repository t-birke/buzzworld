using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ActivityCard : MonoBehaviour
{
    [SerializeField] private TMP_Text _TextTime;
    [SerializeField] private TMP_Text _TextHeading;
    [SerializeField] private TMP_Text _TextBody;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _iconBackground;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setContent(string time, string heading, string body, string icon, string iconColor)
    {
        _TextTime.text = time;
        _TextHeading.text = heading;
        _TextBody.text = body;
        _icon.sprite = Resources.Load<Sprite>("img/icons/" + icon);
        if (ColorUtility.TryParseHtmlString(iconColor, out var color))
        {
            Debug.Log("color parsed successfully: " + color);
            _iconBackground.color = color;
        }
        else
        {
            _iconBackground.color = Color.magenta;
        }
    }
}
