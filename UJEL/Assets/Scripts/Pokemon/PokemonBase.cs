using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    // Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves;

    public string Name {
        get { return name; }
    }
    public string Description {
        get { return description; }
    }
    public Sprite FrontSprite{
        get { return frontSprite; }
    }
    public Sprite BackSprite {
        get { return backSprite; }
    }
    public int MaxHp {
        get { return maxHp; }
    }
    public int Attack {
        get { return attack; }
    }
    public int SpAttack {
        get { return spAttack; }
    }
    public int Defense {
        get { return defense; }
    }
    public int SpDefense {
        get { return spDefense; }
    }
    public int Speed {
        get { return speed; }
    }
    public List<LearnableMove> LearnableMoves {
        get { return learnableMoves;}
    }
}

public enum PokemonType{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Fairy
}

[System.Serializable]
public class LearnableMove{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base {
        get { return moveBase; }
    }

    public int Level {
        get { return level;}
    }
}
