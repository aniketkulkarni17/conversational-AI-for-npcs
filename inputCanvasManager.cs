using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class inputCanvasManager : MonoBehaviour
{
    public inputBackground bg;
    public TMP_InputField inp;
    
    // Start is called before the first frame update
    void Start()
    {

    }
    
    
    // Update is called once per frame

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            bg.gameObject.SetActive(!bg.gameObject.activeSelf);
            inp.ActivateInputField();
            inp.interactable = true;

        }

       
    }
}
