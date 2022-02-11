using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalUtility.InheritBehaviour;

public class PasswordPannel : Signal
{
    [SerializeField]
    char[] entry;

    [SerializeField]
    string password = "1111";

    [SerializeField]
    List<PannelButton> _buttons = new List<PannelButton>();

    PannelButton[] _clicked;

    bool settable = true;

    int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            if (!settable)
                return;

            if (value >= password.Length)
            {
                _currentIndex = 0;
                if (string.Join("", entry) == password)
                {
                    IsActive = true;
                    settable = false;
                    foreach (PannelButton b in _buttons)
                        b.settable = false;
                }
                else 
                {
                    IsActive = false;
                    foreach(PannelButton b in _clicked)
                    {
                        b.GetComponent<Animator>()?.SetBool("isPressed", false);
                        b.IsActive = false;
                    }
                }
            }
            else _currentIndex = value;
        }
    }
    int _currentIndex = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        entry = new char[password.Length];
        _clicked = new PannelButton[password.Length];

        foreach (PannelButton b in _buttons)
            b.ButtonPressed += AddSymbol;
    }

    void AddSymbol(PannelButton button, char symbol)
    {
        entry[CurrentIndex] = symbol;
        _clicked[CurrentIndex] = button;
        CurrentIndex++;
    }
}
