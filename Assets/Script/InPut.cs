using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class InPut
{
    public Keyboard keyboard {  get; private set; }
    public Mouse mouse {  get; private set; }
    //public Touch touch { get; private set; }
 
    public void GetInputDevice()
    {
        keyboard = InputSystem.GetDevice<Keyboard>();
        mouse = InputSystem.GetDevice<Mouse>();
        //touch = InputSystem.GetDevice<Touch>();

    }

}
