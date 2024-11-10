using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    public class Pokemon
    {
        // Public interface for pokemon base stats. Ideally constant.
        public PokemonData Species { get; }
        
        // Non-constant stats for the pokemon.
        public int Level { get; set; }
        public int Hp { get; set; }
        public List<Move> Moves { get; set; }
        public string Nickname { get; set; }

        // In-battle multipliers for each of the stats of a pokemon.
        // Should be reset on entrance by default.
        public int AtkMult { get; set; }
        public int DefMult { get; set; }
        public int SpAMult { get; set; }
        public int SpDMult { get; set; }
        public int SpeMult { get; set; }

        // Individual values for the pokemon. Calculated upon construction and then constant.
        public float AtkIv { get; }
        public float DefIv { get; }
        public float SpAIv { get; }
        public float SpDIv { get; }
        public float SpeIv { get; }
        public float HpIv { get; }

        // Effort values for the pokemon.
        public float AtkEv { get; }
        public float DefEv { get; }
        public float SpAEv { get; }
        public float SpDEv { get; }
        public float SpeEv { get; }
        public float HpEv { get; }

        /// <summary>
        /// Constructs a pokemon given a base and a starting level.
        /// </summary>
        public Pokemon(PokemonData species, int level){
            this.Species = species;
            this.Level = level;
            this.Hp = MaxHp;

            Moves = new List<Move>();
            foreach (var move in species.LearnableMoves){
                if (move.Level <= Level){
                    Moves.Add(new Move(move.Base));
                }
                if (Moves.Count >= 4){
                    break;
                }
            }
        }

        // Functions to calcualte the current base stats of the pokemon.
        public int Atk {
            get { 
                float baseStat = 2.0f * Species.Atk + AtkIv + Mathf.FloorToInt(0.25f * AtkEv);
                return Mathf.FloorToInt(0.01f * baseStat * Level + 5);
            }
        }

        public int Def {
            get { 
                float baseStat = 2.0f * Species.Def + DefIv + Mathf.FloorToInt(0.25f * DefEv);
                return Mathf.FloorToInt(0.01f * baseStat * Level + 5);
            }
        }

        public int SpA {
            get { 
                float baseStat = 2.0f * Species.SpA + SpAIv + Mathf.FloorToInt(0.25f * SpAEv);
                return Mathf.FloorToInt(0.01f * baseStat * Level + 5);
            }
        }

        public int SpD {
            get { 
                float baseStat = 2.0f * Species.SpD + SpDIv + Mathf.FloorToInt(0.25f * SpDEv);
                return Mathf.FloorToInt(0.01f * baseStat * Level + 5);
            }
        }

        public int Spe {
            get { 
                float baseStat = 2.0f * Species.Spe + SpeIv + Mathf.FloorToInt(0.25f * SpeEv);
                return Mathf.FloorToInt(0.01f * baseStat * Level + 5);
            }
        }

        public int MaxHp {
            get { 
                float baseStat = 2.0f * Species.MaxHp + HpIv + Mathf.FloorToInt(0.25f * HpEv);
                return Mathf.FloorToInt(0.01f * baseStat * Level + 10);
            }
        }

        // Returns post adjustment stats.
        public int AdjAtk {
            get {
                return GenData.AdjustByMultiplier(Atk, AtkMult);
            }
        }

        public int AdjDef {
            get {
                return GenData.AdjustByMultiplier(Def, AtkMult);
            }
        }

        public int AdjSpA {
            get {
                return GenData.AdjustByMultiplier(SpA, AtkMult);
            }
        }

        public int AdjSpD {
            get {
                return GenData.AdjustByMultiplier(SpD, AtkMult);
            }
        }

        public int AdjSpe {
            get {
                return GenData.AdjustByMultiplier(Spe, AtkMult);
            }
        }
    }
}
