using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Interactable
{
    public virtual void Interact(Player player)
    {

    }

    public abstract string Name();
}