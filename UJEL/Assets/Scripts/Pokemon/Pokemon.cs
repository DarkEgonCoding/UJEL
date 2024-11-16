using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

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

    public void Init(){
        HP = MaxHp;

        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves){
            if (move.Level <= Level){
                Moves.Add(new Move(move.Base));
            }
            if (Moves.Count >= 4){
                break;
            }
        }
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

        HP -= damage;
        if (HP <= 0){
            HP = 0;
            damageDetails.Fainted = true;
            return damageDetails;
        }
        
        damageDetails.Fainted = false;
        return damageDetails;
    }

    public Move GetRandomMove(){
        int r = UnityEngine.Random.Range(0, Moves.Count);
        return Moves[r];
    }
}

public class DamageDetails{
        public bool Fainted { get; set; }
        public float Critical { get; set; }
        public float TypeEffectiveness { get; set; }
    }
