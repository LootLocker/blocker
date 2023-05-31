using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Transform colorChildsParent;

    public Transform colorSelector;

    public int currentChild;

    public Color currentColor;

    // Start is called before the first frame update
    void Start()
    {
        UpdateColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            MoveDown();
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            MoveUp();
        }
    }

    void MoveDown()
    {
        currentChild --;
        if (currentChild < 0)
        {
            currentChild = colorChildsParent.childCount-1;
        }
        colorSelector.SetParent(colorChildsParent.GetChild(currentChild));
        colorSelector.localPosition = Vector3.zero;
        UpdateColor();
    }

    void MoveUp()
    {
        currentChild++;
        if(currentChild > colorChildsParent.childCount-1)
        {
            currentChild = 0;
        }
        colorSelector.SetParent(colorChildsParent.GetChild(currentChild));
        colorSelector.localPosition = Vector3.zero;
        UpdateColor();
    }

    void UpdateColor()
    {
        currentColor = colorChildsParent.GetChild(currentChild).GetComponent<Image>().color;
    }
}
