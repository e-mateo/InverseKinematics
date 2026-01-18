using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKZone : MonoBehaviour
{
    [SerializeField] List<Transform> IKTargets = new List<Transform>();
    [SerializeField] Transform TargetPoint;

    List<Vector3> baseLocalPositionIKTargets = new List<Vector3>();

    bool bIsActive = false;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < IKTargets.Count; i++)
        {
            baseLocalPositionIKTargets.Add(IKTargets[i].localPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(bIsActive)
        {
            for (int i = 0; i < IKTargets.Count; i++)
            {
                IKTargets[i].transform.position = TargetPoint.transform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            bIsActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            bIsActive = false;
            for (int i = 0; i < IKTargets.Count; i++)
            {
                IKTargets[i].transform.localPosition = baseLocalPositionIKTargets[i];
            }
        }
    }
}
