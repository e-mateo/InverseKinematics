using UnityEditor;
using UnityEngine;

public enum AxisConstraints
{ 
    FREE,
    CLAMP,
    LOCK
}

public class Jacobian_Constraints : MonoBehaviour
{
    public AxisConstraints XAxisConstraint;
    public AxisConstraints YAxisConstraint;
    public AxisConstraints ZAxisConstraint;

    [Range(-180f, 0)] public float minXAngle = -180f;
    [Range(0, 180f)] public float maxXAngle = 180f;

    [Range(-180f, 0)] public float minYAngle = -180f;
    [Range(0, 180f)] public float maxYAngle = 180f;

    [Range(-180f, 0)] public float minZAngle = -180f;
    [Range(0, 180f)] public float maxZAngle = 180f;

    public bool IsLock(EAxis axis)
    {
        switch(axis) 
        { 
            case EAxis.X: return XAxisConstraint == AxisConstraints.LOCK;
            case EAxis.Y: return YAxisConstraint == AxisConstraints.LOCK;
            case EAxis.Z: return ZAxisConstraint == AxisConstraints.LOCK;
            default: return false;
        }
    }

    public bool IsClamp(EAxis axis)
    {
        switch (axis)
        {
            case EAxis.X: return XAxisConstraint == AxisConstraints.CLAMP;
            case EAxis.Y: return YAxisConstraint == AxisConstraints.CLAMP;
            case EAxis.Z: return ZAxisConstraint == AxisConstraints.CLAMP;
            default: return false;
        }
    }

    public void GetMinMaxAngle(EAxis axis, out float minAngle, out float maxAngle)
    {
        minAngle = -180f;
        maxAngle = 180f;

        if (!IsClamp(axis))
        {
            return;
        }

        switch (axis)
        {
            case EAxis.X:
                minAngle = minXAngle;
                maxAngle = maxXAngle;
                break;
            case EAxis.Y:
                minAngle = minYAngle;
                maxAngle = maxYAngle;
                break;
            case EAxis.Z:
                minAngle = minZAngle;
                maxAngle = maxZAngle;
                break;
            default: break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (Selection.activeGameObject != gameObject)
            return;

        Vector3 ROTx = transform.right;
        Vector3 ROTy = transform.up;
        Vector3 ROTz = transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (ROTx * 0.5f));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (ROTy * 0.5f));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (ROTz * 0.5f));

        Jacobian_Constraints constraints = transform.GetComponent<Jacobian_Constraints>();
        if (constraints)
        {
            constraints.GetMinMaxAngle(EAxis.X, out float minX, out float maxX);
            constraints.GetMinMaxAngle(EAxis.Y, out float minY, out float maxY);
            constraints.GetMinMaxAngle(EAxis.Z, out float minZ, out float maxZ);
            if (!constraints.IsLock(EAxis.X))
            {
                UnityEditor.Handles.color = Color.red;
                UnityEditor.Handles.DrawSolidArc(transform.position, ROTx, ROTz, maxX, 0.5f);
                UnityEditor.Handles.DrawSolidArc(transform.position, ROTx, ROTz, minX, 0.5f);
            }
            if (!constraints.IsLock(EAxis.Y))
            {
                UnityEditor.Handles.color = Color.green;
                UnityEditor.Handles.DrawSolidArc(transform.position, ROTy, -ROTx, minY, 0.5f);
                UnityEditor.Handles.DrawSolidArc(transform.position, ROTy, -ROTx, maxY, 0.5f);
            }
            if (!constraints.IsLock(EAxis.Z))
            {
                UnityEditor.Handles.color = Color.blue;
                UnityEditor.Handles.DrawSolidArc(transform.position, ROTz, -ROTx, minZ, 0.5f);
                UnityEditor.Handles.DrawSolidArc(transform.position, ROTz, -ROTx, maxZ, 0.5f);
            }
        }
    }
}
