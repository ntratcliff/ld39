using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILightAffected
{
    /// <summary>
    /// Called when a light source in the scene hits this object during raycast
    /// </summary>
    void OnLightHit();
}
