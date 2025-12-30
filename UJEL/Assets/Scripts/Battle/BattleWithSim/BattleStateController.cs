using System.Collections;
using System.Collections.Generic;
using PsLib.Sim.Messages.Parts;
using UnityEngine;

public class BattleStateController : MonoBehaviour
{
    public static BattleStateController instance;

    private Request CurrentRequest;
    private int CurrentRequestId;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetCurrentRequest(Request request)
    {
        this.CurrentRequest = request;
        this.CurrentRequestId = request.rqid;
    }
}
