using System.Collections;
using System.Collections.Generic;
using QFramework;
using QFramework.AmorHero;
using QFramework.Example;
using UnityEngine;
using UnityEngine.EventSystems;

public class PropImage : MonoBehaviour,IPointerClickHandler,ICanSendCommand
{
    // Start is called before the first frame update
    public PropItem thisPropItem;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        this.SendCommand(new ShowPropContent(thisPropItem.prop_name,thisPropItem.prop_des));
    }

    public IArchitecture GetArchitecture()
    {
        return AmorHeroArchitecture.Interface;
    }
}
