using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void set_value(int value){
        int x = 0;
        while(x < 6) {
            GetComponent<RectTransform>().GetChild(x).GetComponent<UnityEngine.UI.Toggle>().isOn = false;
			x++;
        }
        x = 0;
        while(x < value){
            GetComponent<RectTransform>().GetChild(x).GetComponent<UnityEngine.UI.Toggle>().isOn = true;
            x++;
        }
    }
}
