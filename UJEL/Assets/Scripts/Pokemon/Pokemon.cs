using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    List<string> pMoves;
    public List<string> PMoves => pMoves;
    Natures nature;
    string ability;

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;

        nature = getRandomNature();
        ability = getRandomAbility(pBase.Abilities);

        Init();
    }

    public Pokemon(PokemonBase pBase, int pLevel, List<string> pMoves)
    {
        _base = pBase;
        level = pLevel;
        this.pMoves = pMoves;

        nature = getRandomNature();
        ability = getRandomAbility(pBase.Abilities);

        Init();
    }

    public PokemonBase Base
    {
        get
        {
            return _base;
        }
    }
    public int Level
    {
        get
        {
            return level;
        }
    }
    public int HP { get; set; }

    // Old list of moves
    public List<Move> Moves { get; set; }
    public int Exp { get; set; }

    public Condition Status { get; private set; }
    public event System.Action OnStatusChanged;
    public event System.Action OnHPChanged;

    public void Init()
    {
        HP = MaxHp;

        Exp = Base.GetExpForLevel(Level);

        // Auto generate possible moves if they don't exist
        if (pMoves == null)
        {
            pMoves = new List<string>();
            foreach (var moveLearn in Base.LevelUpMoves)
            {
                if (moveLearn.Level <= Level)
                {
                    pMoves.Add(moveLearn.Move);
                }
                if (pMoves.Count >= PokemonBase.MaxNumOfMoves) {
                    break;
                }
            }
        }

        // OLD MOVE GENERATION
        /*
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }
            if (Moves.Count >= PokemonBase.MaxNumOfMoves)
            {
                break;
            }
        }
        */
    }

    public string getRandomAbility(List<string> abilities)
    {
        if (abilities == null || abilities.Count == 0)
        {
            Debug.LogWarning("No ability list found.");
            return null;
        }

        int index = UnityEngine.Random.Range(0, abilities.Count);
        return abilities[index];
    }

    public Natures getRandomNature()
    {
        var values = Enum.GetValues(typeof(Natures));
        var random = new System.Random();
        int index = random.Next(values.Length);
        return (Natures)values.GetValue(index);
    }

    public PokemonSaveData GetSaveData()
    {
        var saveData = new PokemonSaveData()
        {
            name = Base.PokemonName,
            hp = HP,
            level = Level,
            exp = Exp,
            statusId = Status?.Id,
            moves = pMoves,
            nature = this.nature,
            ability = this.ability,
        };

        return saveData;
    }

    public Pokemon(PokemonSaveData saveData)
    {
        _base = PokemonDB.GetPokemonByName(saveData.name);
        HP = saveData.hp;
        level = saveData.level;
        Exp = saveData.exp;
        nature = saveData.nature;
        ability = saveData.ability;

        if (saveData.statusId != null)
        {
            Status = ConditionsDB.Conditions[saveData.statusId.Value];
        }
        else Status = null;

        // Calculate Stats?

        pMoves = saveData.moves;
    }

    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }
        return false;
    }

    /*
    public LearnableMove GetLearnableMoveAtCurrLevel()
    {
        return Base.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }
    */

    public string GetLearnableMoveAtCurrLevel()
    {
        var moveLearn = Base.LevelUpMoves.FirstOrDefault(x => x.Level == level);
        return moveLearn?.Move; // returns null if none found
    }

    /*
    public void LearnMove(MoveBase moveToLearn)
    {
        if (Moves.Count > PokemonBase.MaxNumOfMoves)
        {
            return;
        }

        Moves.Add(new Move(moveToLearn));
    }
    */

    public void LearnMove(string moveToLearn)
    {
        if (pMoves.Count > PokemonBase.MaxNumOfMoves)
        {
            return;
        }

        pMoves.Add(moveToLearn);
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((2 * Base.Attack * Level) / 100f) + Level + 5; }
    }
    public int Defense
    {
        get { return Mathf.FloorToInt((2 * Base.Defense * Level) / 100f) + Level + 5; }
    }
    public int SpAttack
    {
        get { return Mathf.FloorToInt((2 * Base.SpAttack * Level) / 100f) + Level + 5; }
    }
    public int SpDefense
    {
        get { return Mathf.FloorToInt((2 * Base.SpDefense * Level) / 100f) + Level + 5; }
    }
    public int Speed
    {
        get { return Mathf.FloorToInt((2 * Base.Speed * Level) / 100f) + Level + 5; }
    }
    public int MaxHp
    {
        get { return Mathf.FloorToInt((2 * Base.MaxHp * Level) / 100f) + Level + 10; }
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {

        float critical = 1f;
        if (UnityEngine.Random.value * 100f <= 6.25f)
        {
            critical = 2f;
        }

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (move.Base.IsSpecial) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.IsSpecial) ? SpDefense : Defense;

        float modifiers = UnityEngine.Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level / 5) + 2;
        float d = (a * move.Base.Power * (float)attack / defense / 50) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        if (ReduceHP(damage))
        {
            damageDetails.Fainted = true;
            return damageDetails;
        }

        damageDetails.Fainted = false;
        return damageDetails;
    }

    public bool ReduceHP(int damage)
    {
        HP -= damage;
        OnHPChanged?.Invoke();

        if (HP <= 0)
        {
            HP = 0;
            return true; // The pokemon fainted
        }
        else return false; // The pokemon did not faint
    }

    public void IncreaseHP(int heal)
    {
        HP = Mathf.Clamp(HP + heal, 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    public void Heal()
    {
        HP = MaxHp;
        CureStatus();
    }

    /*
    public bool HasMove(MoveBase moveToCheck)
    {
        return Moves.Count(m => m.Base == moveToCheck) > 0;
    }
    */

    public bool HasMove(string moveToCheck)
    {
        return pMoves.Count(m => m == moveToCheck) > 0;
    }

    public Evolution CheckForEvolution()
    {
        return Base.Evolutions.FirstOrDefault(e => e.RequiredLevel <= level);
    }

    public void Evolve(Evolution evolution)
    {
        var nameOfEvolution = PokemonDB.FixWeirdPokemonNames(evolution.EvolvesInto);
        _base = PokemonDB.GetPokemonByName(nameOfEvolution);

        float hpPercent = (float)HP / MaxHp;

        HP = Mathf.RoundToInt(hpPercent * MaxHp);

        foreach (var moveLearn in Base.LevelUpMoves)
        {
            if (moveLearn.Level <= Level && !pMoves.Contains(moveLearn.Move))
            {
                if (pMoves.Count < PokemonBase.MaxNumOfMoves)
                {
                    pMoves.Add(moveLearn.Move);
                }
            }
        }
    }

    /*
    public Move GetRandomMove()
    {
        int r = UnityEngine.Random.Range(0, Moves.Count);
        return Moves[r];
    }
    */

    public string GetRandomMove()
    {
        int r = UnityEngine.Random.Range(0, pMoves.Count);
        return pMoves[r];
    }

    public void SetStatus(ConditionID conditionId)
    {
        if (Status != null) return;

        Status = ConditionsDB.Conditions[conditionId];
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}

[System.Serializable]
public class PokemonSaveData
{
    public string name;
    public int hp;
    public int level;
    public int exp;
    public ConditionID? statusId;
    // public List<MoveSaveData> moves;
    public List<string> moves;
    public Natures nature;
    public string ability;
}

public enum Natures
{
    adamant,
    bashful,
    bold,
    brave,
    calm,
    careful,
    docile,
    gentle,
    hardy,
    hasty,
    impish,
    jolly,
    lax,
    lonely,
    mild,
    modest,
    naive,
    naughty,
    quiet,
    quirky,
    rash,
    relaxed,
    sassy,
    serious,
    timid,
}