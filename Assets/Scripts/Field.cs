using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Field : MonoBehaviour
{    
    public static event Action<Field> OnLeftClick, OnRightClick;
    
    public SpriteRenderer Rend;
    public bool IsBomb, IsOpened;
    public Sprite ClosedSprite, BaseSprite;

    private void Awake() {
        Rend = this.gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown() {
        if(!IsOpened) Rend.sprite = BaseSprite;
    }

    private void OnMouseUp() {
        if(!IsOpened) Rend.sprite = ClosedSprite;
    }

    private void OnMouseOver() {
        if(Input.GetMouseButtonDown(1) && !IsOpened)
        OnRightClick?.Invoke(this);        
    }

    private void OnMouseUpAsButton() {
        if(!IsOpened)
        {
            OnLeftClick?.Invoke(this);
        }
    }

}

