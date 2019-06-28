using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Footstep Group", order = 1)]
public class FootstepGroup : ScriptableObject{
    public AudioClip[] footSounds;
    public Texture2D[] textures;
}
