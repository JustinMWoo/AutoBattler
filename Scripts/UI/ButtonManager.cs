using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    #region Singleton
    private static ButtonManager _instance;
    public static ButtonManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    [SerializeField]
    private EnumButton[] buttons;

    public void EnableButton(Buttons buttonType)
    {
        foreach (EnumButton button in buttons)
        {
            if(button.type == buttonType)
            {
                button.gameObject.SetActive(true);
            }
        }
    }

    public void DisableButton(Buttons buttonType)
    {
        foreach (EnumButton button in buttons)
        {
            if (button.type == buttonType)
            {
                button.gameObject.SetActive(false);
            }
        }
    }
}
