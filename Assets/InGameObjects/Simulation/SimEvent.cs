using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimEvent : MonoBehaviour
{
    public checkState selfCheckState;
    public int simStepCounter;
    public int testedMaxTrafficLight = 0;
    public int pairTrafficLight= 0;
    public int testedScore;
    public int pairTLsScore;

    public Dictionary<int, int> TLIDScorePair;

    public enum checkState
    {
        init,
        firstCheck,
        pairCheck
    }

    public void SetAllParameters (checkState CheckState, int SimStepCounter, int TestedMaxTL, int PairTL, int TestedScore, int PairScore, Dictionary <int,int> TLScores)
    {
        selfCheckState = CheckState;
        simStepCounter = SimStepCounter;
        testedMaxTrafficLight = TestedMaxTL;
        pairTrafficLight = PairTL;
        testedScore = TestedScore;
        pairTLsScore = PairScore;
        TLIDScorePair = TLScores;
    }
}
