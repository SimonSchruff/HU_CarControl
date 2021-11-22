using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InputFieldModifier : MonoBehaviour
{
    private InputField m_TargetInputField;

    public bool m_PermitDigits;
    public bool m_PermitLetters;
    public bool m_PermitSpaces;
    public bool m_PermitPeriod;
    public bool m_PermitPunctuation;

    void Start()
    {
        m_TargetInputField = GetComponent<InputField>();
    }

    /// <summary>
    /// Corrects the input of the inputfield. call on edit and on edit end.
    /// Source :
    /// https://forum.unity.com/threads/alphanumeric-with-spaces-script-solution.840526/
    /// </summary>
    public void OnValueChangedFunction()
    {
        if (m_TargetInputField.text.Length > 0)
        {
            char c = m_TargetInputField.text[m_TargetInputField.text.Length - 1]; // get last character
            if (m_PermitDigits && char.IsDigit(c))
                return;
            if (m_PermitLetters && char.IsLetter(c))
                return;
            if (m_PermitSpaces && char.IsWhiteSpace(c))
                return;
            if (m_PermitPeriod && c == '.')
                return;
            if (m_PermitPunctuation && char.IsPunctuation(c))
                return;
            // if we get here, the character is not allowed and should be removed
            CullLastChar();
        }
    }
    private void CullLastChar()
    {
        m_TargetInputField.text = m_TargetInputField.text.Substring(0, m_TargetInputField.text.Length - 1);
    }

    public void CopyToClipboard()
    {
         GUIUtility.systemCopyBuffer =  m_TargetInputField.text; 
    }
}
