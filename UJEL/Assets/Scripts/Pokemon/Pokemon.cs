using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public Pokemon(PokemonBase pBase, int pLevel){
        _base = pBase;
        level = pLevel;

        Init();
    }

    public PokemonBase Base {
        get {
            return _base;
        }
    }
    public int Level {
        get {
            return level;
        }
    }
    public int HP {get; set;}

    public List<Move> Moves {get; set;}
    public int Exp { get; set; }

    public event System.Action OnHPChanged;

    public void Init()
    {
        HP = MaxHp;

        Exp = Base.GetExpForLevel(Level);

        // Generate Moves
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
    }

    public PokemonSaveData GetSaveData(){
        var saveData = new PokemonSaveData(){
            name = Base.name,
            hp = HP,
            level = Level,
            exp = Exp,
            // Add Status?
            moves = Moves.Select(m => m.GetSaveData()).ToList()
        };

        return saveData;
    }

    public Pokemon(PokemonSaveData saveData){
        _base = PokemonDB.GetPokemonByName(saveData.name);
        HP = saveData.hp;
        level = saveData.level;
        Exp = saveData.exp;

        // Status?
        // Calculate Stats?

        Moves = saveData.moves.Select(s => new Move(s)).ToList();
    }

    public bool CheckForLevelUp(){
        if (Exp > Base.GetExpForLevel(level + 1)){
            ++level;
            return true;
        }
        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrLevel(){
        return Base.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }

    public void LearnMove(LearnableMove moveToLearn){
        if (Moves.Count > PokemonBase.MaxNumOfMoves){
            return;
        }

        Moves.Add(new Move(moveToLearn.Base));
    }

    public int Attack {
        get { return Mathf.FloorToInt((2 * Base.Attack * Level) / 100f) + Level + 5;}
    }
    public int Defense {
        get { return Mathf.FloorToInt((2 * Base.Defense * Level) / 100f) + Level + 5;}
    }
    public int SpAttack {
        get { return Mathf.FloorToInt((2 * Base.SpAttack * Level) / 100f) + Level + 5;}
    }
    public int SpDefense {
        get { return Mathf.FloorToInt((2 * Base.SpDefense * Level) / 100f) + Level + 5;}
    }
    public int Speed {
        get { return Mathf.FloorToInt((2 * Base.Speed * Level) / 100f) + Level + 5;}
    }
    public int MaxHp {
        get { return Mathf.FloorToInt((2 * Base.MaxHp * Level) / 100f) + Level + 10;}
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker){

        float critical = 1f;
        if (UnityEngine.Random.value * 100f <= 6.25f){
            critical = 2f;
        }

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.type2);

        var damageDetails = new DamageDetails(){
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (move.Base.IsSpecial)? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.IsSpecial)? SpDefense : Defense;

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

    public Move GetRandomMove()
    {
        int r = UnityEngine.Random.Range(0, Moves.Count);
        return Moves[r];
    }
}

public class DamageDetails{
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

    // Status Effects?

    public List<MoveSaveData> moves;
}