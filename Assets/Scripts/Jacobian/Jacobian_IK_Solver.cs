using System.Collections.Generic;
using UnityEngine;


public class Jacobian_IK_Solver : IK_Solver
{
    float jointMaxLenght = 0f;
    Vector3 goalPosition;
    List<Quaternion> baseLocalRotations = new List<Quaternion>();
    List<Vector3> currentAngles = new List<Vector3>();

    void Start()
    {
        CreateJoints();
        jointMaxLenght = ComputeJointMaxLenght();
    }

    private void CreateJoints()
    {
        Transform currentBone = endEffector;
        AddJoint(currentBone);
        while (currentBone != baseBone && currentBone != null)
        {
            currentBone = currentBone.parent;
            AddJoint(currentBone);
        }

        joints.Reverse();
        baseLocalRotations.Reverse();
    }

    // Update is called once per frame
    void Update()
    {
        int iterations = 0;
        goalPosition = ComputeGoalPosition();
        while ((goalPosition - endEffector.position).magnitude > EPS && iterations < maxIterations)
        {
            SolveIK();
            iterations++;
        }
    }

    void AddJoint(Transform joint)
    {
        joints.Add(joint);
        baseLocalRotations.Add(joint.localRotation);
        currentAngles.Add(Vector3.zero);
    }

    float ComputeJointMaxLenght()
    {
        float lenght = 0f;
        for (int i = 0; i < joints.Count - 1; i++)
        {
            lenght += Vector3.Distance(joints[i].position, joints[i + 1].position);
        }
        return lenght;
    }

    Vector3 ComputeGoalPosition()
    {
        float StartToGoalLenght = Vector3.Distance(baseBone.position, target.position);
        if (StartToGoalLenght < jointMaxLenght)
        {
            return target.position;
        }
        else
        {
            return baseBone.position + ((target.position - baseBone.position).normalized * jointMaxLenght);
        }
    }

    void SolveIK()
    {
        for (int axis = (int)EAxis.X; axis <= (int)EAxis.Z; axis++) //Loop for each three axis
        {
            //Compute Jacobian Matrix
            Matrix jacobianMatrix = GetJacobianMatrix((EAxis)axis);
            Matrix transposeJacobian = Matrix.GetTranspose(jacobianMatrix);

            //Calculate DeltaAngle and add it to CurrentAngle
            float[] deltaAngle = GetDeltaOrientation(transposeJacobian);
            AddDeltaAngle(deltaAngle, (EAxis)axis);
        }
    }

    Matrix GetJacobianMatrix(EAxis axis)
    {
        Matrix jacobian = new Matrix(3, joints.Count);

        for (int i = 0; i < joints.Count; i++)
        {
            Vector3 rotationAxis = GetJointRotationAxis(axis, i);
            Vector3 crossProduct = Vector3.Cross(rotationAxis, endEffector.position - joints[i].position);
            jacobian[0, i] = crossProduct.x;
            jacobian[1, i] = crossProduct.y;
            jacobian[2, i] = crossProduct.z;
        }

        return jacobian;
    }

    Vector3 GetJointRotationAxis(EAxis axis, int jointIndex)
    {
        Quaternion rotation = joints[jointIndex].parent.rotation * baseLocalRotations[jointIndex];
        switch (axis)
        {
            case EAxis.X: 
                return rotation * Vector3.right;
            case EAxis.Y: 
                return rotation * Vector3.up;
            case EAxis.Z: 
                return rotation * Vector3.forward;
            default: 
                break;
        }

        return Vector3.zero;
    }

    float[] GetDeltaOrientation(Matrix jacobianMatrix)
    {
        Vector3 targetToEnd = goalPosition - endEffector.position;
        float[] deltaAngle = Matrix.MultiplyVector(jacobianMatrix, targetToEnd);
        return deltaAngle;
    }

    void AddDeltaAngle(float[] DeltaAngleToAdd, EAxis axis)
    {
        for (int i = 0; i < joints.Count; i++)
        {
            Vector3 angles = currentAngles[i];
            float NewAngle = angles[(int)axis] + (DeltaAngleToAdd[i] * simulationStep);
            angles[(int)axis] = Mathf.Lerp(angles[(int)axis], NewAngle, weight);
            angles[(int)axis] = GetAngleBetween_Minus180_180(angles[(int)axis]);

            ResolveConstraints(joints[i].GetComponent<Jacobian_Constraints>(), axis, ref angles, i);

            Quaternion BaseRotation = joints[i].parent.rotation * baseLocalRotations[i];
            Quaternion NewRotation = BaseRotation * Quaternion.Euler(angles[0], angles[1], angles[2]);
            joints[i].rotation = NewRotation;
            currentAngles[i] = angles;
        }
    }

    void ResolveConstraints(Jacobian_Constraints constraints, EAxis axis, ref Vector3 angles, int jointIndex)
    {
        if (constraints == null)
        {
            return;
        }

        if (constraints.IsLock(axis))
        {
            angles[(int)axis] = currentAngles[jointIndex][(int)axis];
        }
        else if(constraints.IsClamp(axis))
        {
            constraints.GetMinMaxAngle(axis, out float minAngle, out float maxAngle);
            angles[(int)axis] = Mathf.Clamp(angles[(int)axis], minAngle, maxAngle);
        }
    }

    float GetAngleBetween_Minus180_180(float angle)
    {
        if(angle < -180)
        {
            float difference = angle - (-180);
            angle = 180f - (angle - ((angle / 360) * 360f));
        }
        else if(angle > 180)
        {
            float difference = angle - 180;
            angle = -180f + (angle - ((angle / 360) * 360f));
        }

        return angle;
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < joints.Count; i++)
        {
            Jacobian_Constraints constraints = joints[i].GetComponent<Jacobian_Constraints>();
            if (constraints)
            {
                Vector3 ROTx = GetJointRotationAxis(EAxis.X, i);
                Vector3 ROTy = GetJointRotationAxis(EAxis.Y, i);
                Vector3 ROTz = GetJointRotationAxis(EAxis.Z, i);

                if (!constraints.IsLock(EAxis.X))
                {
                    UnityEditor.Handles.color = Color.blue;
                    constraints.GetMinMaxAngle(EAxis.X, out float minX, out float maxX);
                    UnityEditor.Handles.DrawSolidArc(joints[i].position, ROTx, ROTz, maxX, 0.25f);
                    UnityEditor.Handles.DrawSolidArc(joints[i].position, ROTx, ROTz, minX, 0.25f);
                }
                if (!constraints.IsLock(EAxis.Y))
                {
                    UnityEditor.Handles.color = Color.red;
                    constraints.GetMinMaxAngle(EAxis.Y, out float minY, out float maxY);
                    UnityEditor.Handles.DrawSolidArc(joints[i].position, ROTy, ROTx, minY, 0.25f);
                    UnityEditor.Handles.DrawSolidArc(joints[i].position, ROTy, ROTx, maxY, 0.25f);
                }
                if (!constraints.IsLock(EAxis.Z))
                {
                    UnityEditor.Handles.color = Color.green;
                    constraints.GetMinMaxAngle(EAxis.Z, out float minZ, out float maxZ);
                    UnityEditor.Handles.DrawSolidArc(joints[i].position, ROTz, ROTy, minZ, 0.25f);
                    UnityEditor.Handles.DrawSolidArc(joints[i].position, ROTz, ROTy, maxZ, 0.25f);
                }
            }
        }
    }
}
