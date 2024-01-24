using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is created for identifying the context in opened 'Book UI'
// for 'illustrated guide' or for 'information check in store.'
// Other states can be added along with that.

public enum BookUIState {
    Guide,
    Inform
}
public class GameStateManager : MonoBehaviour 
{   
    public BookUIState BookUIState {get; private set;}
    void Start()
    {
        BookUIState = BookUIState.Guide;
    }
    public void ChangeBookUIState() {
        if (BookUIState == BookUIState.Guide) {
            BookUIState = BookUIState.Inform;
        }
        else {
            BookUIState = BookUIState.Guide;
        }
    }
}
