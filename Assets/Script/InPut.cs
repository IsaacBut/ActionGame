using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InPut
{
    public Keyboard keyboard {  get; private set; }
    public Mouse mouse {  get; private set; }

    public void GetInputDevice()
    {
        keyboard = InputSystem.GetDevice<Keyboard>();
        mouse = InputSystem.GetDevice<Mouse>();
    }

}
