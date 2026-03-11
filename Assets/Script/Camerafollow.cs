using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camerafollow : MonoBehaviour
{
    GameObject mainCharacter;
    [SerializeField] float returnSpeed;
    [SerializeField] float height;
    [SerializeField] float rearDistance;

    Vector3 cameraOffset;
    Vector3 currentVector;
    void Start()
    {
        mainCharacter = transform.parent.gameObject.GetComponentInChildren<PlayerController>().gameObject;
        transform.position = new Vector3(
            mainCharacter.transform.position.x,
            mainCharacter.transform.position.y + height,
            mainCharacter.transform.position.z - rearDistance);
        transform.rotation = Quaternion.LookRotation(mainCharacter.transform.position - transform.position);
    }

    public void SetOffset(Vector3 offset)
    {
        if (offset.z < 0) cameraOffset = offset * 10.001f;
        else if (offset.z > 0) cameraOffset = offset * 3.1415f;
        else cameraOffset = offset * 8.567f;
    }

    void CameraMove()
    {
        currentVector = new Vector3(
            mainCharacter.transform.position.x + cameraOffset.x,
            mainCharacter.transform.position.y + height,
            mainCharacter.transform.position.z - rearDistance + cameraOffset.z);
        transform.position = Vector3.Lerp(transform.position, currentVector, returnSpeed * Time.deltaTime);
    }

    void Update()
    {
        CameraMove();
    }
}
