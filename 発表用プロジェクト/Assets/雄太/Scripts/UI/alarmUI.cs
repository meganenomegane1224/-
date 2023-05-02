using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class alarmUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;
private void Start() {
    rectTransform.localPosition = new Vector3(0,0,0);
    Destroy(rectTransform.gameObject,3);
}}
