
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    public class Move
    {
        public MoveData Base { get; set; }
        public int PP { get; set; }

        public Move(MoveData pBase){
            Base = pBase;
            PP = pBase.PP;
        }
    }

}
