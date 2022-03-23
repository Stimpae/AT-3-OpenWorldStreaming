using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Hover();
    void UnHover();
    void Select();
    void Deselect();
    void Interact();
}
