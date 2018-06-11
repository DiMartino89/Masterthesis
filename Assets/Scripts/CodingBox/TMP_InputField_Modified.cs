using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMP_InputField_Modified : TMP_InputField
{
    public void SetCursorPosition(int i)
    {
        this.m_CaretPosition = i;
        this.caretPosition = i;

        Debug.Log("x " + this.caretPosition);
        Debug.Log("m " + this.m_CaretPosition);


        this.caretSelectPositionInternal = i;
        this.selectionFocusPosition = i;
        this.m_CaretSelectPosition = i;
        this.Select();
    }
}
