using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAppear : MonoBehaviour
{
    [SerializeField] private Texture2D _cursorTx; 
    
    private void Start()
    {
        Cursor.SetCursor(_cursorTx, Vector2.zero, CursorMode.Auto);
    }
}
