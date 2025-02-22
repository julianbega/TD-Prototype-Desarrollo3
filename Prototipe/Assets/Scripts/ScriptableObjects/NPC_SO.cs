﻿using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "ScriptableObjects/NPCScriptableObject", order = 1)]
public class NPC_SO : ScriptableObject
{
    public string npc_name;
    public Sprite image;
    public Sprite background;
}
