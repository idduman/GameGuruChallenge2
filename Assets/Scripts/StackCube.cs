using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameGuruChallenge
{
    public enum CubeState
    {
        Waiting,
        Moving,
        Stopped,
        Completed,
        Failed,
    }
    public class StackCube : MonoBehaviour
    {
        public CubeState State = CubeState.Waiting;

        public float Width => transform.localScale.x;
        public float LengthZ => transform.localScale.z;
        public float CenterPosZ => transform.position.z;
        public float CenterPosX => transform.position.x;
        public float StartPosZ => CenterPosZ - LengthZ/2f;
        public float EndPosZ => CenterPosZ + LengthZ/2f;
        
    }

}