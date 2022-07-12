using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BrowserEngineManager : MonoBehaviour
{
    public GameObject AndroidBrowserView;
    public GameObject WindowsBrowserView;
    public Image PlaceholderImg;
    public EventTrigger trigger;
    public string url;


    // Start is called before the first frame update
    void Awake()
    {
        //make image transparent 
        Color c = Color.white;
        c.a = 0f;
        PlaceholderImg.color = c;


#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

        SimpleWebBrowser.WebBrowser wb = WindowsBrowserView.GetComponent<SimpleWebBrowser.WebBrowser>();
        wb.InitialURL = url;
        WindowsBrowserView.SetActive(true);

        
        EventTrigger.Entry PointerDownEvent = new EventTrigger.Entry();
        PointerDownEvent.eventID = EventTriggerType.PointerDown;
        PointerDownEvent.callback.AddListener((data) => { wb.OnMouseDownOverride(data); });
        trigger.triggers.Add(PointerDownEvent);


        EventTrigger.Entry PointerUpEvent = new EventTrigger.Entry();
        PointerUpEvent.eventID = EventTriggerType.PointerUp;
        PointerUpEvent.callback.AddListener((data) => { wb.OnMouseUpOverride(data); });
        trigger.triggers.Add(PointerUpEvent);

#elif UNITY_ANDROID

        BrowserView bv = AndroidBrowserView.GetComponent<BrowserView>();
        bv.startUrl = url;
        AndroidBrowserView.SetActive(true);

        EventTrigger.Entry PointerClickEvent = new EventTrigger.Entry();
        PointerClickEvent.eventID = EventTriggerType.PointerDown;
        PointerClickEvent.callback.AddListener((data) => { bv.OnClick(data); });
        trigger.triggers.Add(PointerClickEvent);

#endif

    }


}
