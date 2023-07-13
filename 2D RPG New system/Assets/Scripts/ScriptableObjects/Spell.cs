using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "ScriptableObjects", order = 1)]
public class Spell : ScriptableObject
{
    public string spellName;
    public SpellType spellType;
    public UnitType unitType;
    public SpellShape spellShape;
    public int range;
    public int damageRange;
    public int baseDamage;
    public int attackPoints;

    public enum SpellType
    {
        attack,
        heal,
    }

    public enum UnitType
    {
        player,
        enemy,
        both,
    }

    public enum SpellShape
    {
        line,
        diamond,
        cross,
        wall,
    }
}
