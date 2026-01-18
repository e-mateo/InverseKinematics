using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAxis
{
    X,
    Y,
    Z,
}

public abstract class IK_Solver : MonoBehaviour
{
    [SerializeField] protected Transform endEffector;
    [SerializeField] protected Transform baseBone;
    [SerializeField] protected Transform target;
    [SerializeField] protected int maxIterations;
    [SerializeField][Range(0f, 1f)] protected float weight;
    [SerializeField] protected float EPS;
    [SerializeField] protected float simulationStep;

    protected List<Transform> joints = new List<Transform>();
}
