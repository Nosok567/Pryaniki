using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using System.IO;


public class Aim : MonoBehaviour
{
    [SerializeField] Transform spawnPosition;
    [SerializeField] List<GameObject> allTargets;
    [SerializeField] GameObject targetCylinder;
    [SerializeField] float range;

    PlayerInputs inputs;
    PhotonView pv;
    CharacterController controller;
    GameObject targetObj;
    bool canSearch;
    int targetCount;

    private void Awake()
    {
        inputs = new PlayerInputs();
        controller = GetComponent<CharacterController>();
        pv = GetComponentInParent<PhotonView>();
         
    }
    private void OnEnable()
    {
        inputs.CharacterControlls.Enable();
    }
    private void OnDisable()
    {
        inputs.CharacterControlls.Disable();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    
    public void SetTargetSelected(bool isTarget)
    {
        targetCylinder.SetActive(isTarget);
         
    }

    void Calculate()
    {
        canSearch = false;
        allTargets.Clear();

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, range, transform.position, range);
        foreach(RaycastHit hit in hits )
        {
            GameObject temp = hit.transform.gameObject;
            if (temp.GetComponent<CharacterController>() && !temp.GetComponentInParent<PhotonView>().IsMine)
            {
                allTargets.Add(temp);
            }
        }
        SelectNewTarget();
    }

    void SelectedTarget()
    {
        if(controller.velocity == Vector3.zero)
        {
            if (canSearch) InvokeRepeating(nameof(Calculate), 0, 0.5f);
            else
            {
                if(targetObj != null)
                {
                    targetObj.GetComponent<Aim>().SetTargetSelected(false);
                    targetObj = null;
                }
                canSearch = true;
                CancelInvoke();
            }
        }
    }

    void SelectNewTarget()
    {
        foreach(GameObject obj  in allTargets)
        {
            obj.GetComponent<Aim>().SetTargetSelected(false);
        }
        if (targetCount >= allTargets.Count)
        {
            targetCount = 0;
        }
        if (allTargets.Count == 0) return;
        targetObj = allTargets[targetCount];
        targetObj.GetComponent<Aim>().SetTargetSelected(true);
    }
    void SelectNewTarget(InputAction.CallbackContext context)
    {
        foreach (GameObject obj in allTargets)
        {
            obj.GetComponent<Aim>().SetTargetSelected(false);
        }
        if (targetCount >= allTargets.Count)
        {
            targetCount = 0;
        }
        if (allTargets.Count == 0) return;
        targetObj = allTargets[targetCount];
        targetObj.GetComponent<Aim>().SetTargetSelected(true);
    }

    private void Start()
    {
        if (!pv.IsMine) return;
        targetCylinder.SetActive(false);
        inputs.CharacterControlls.ChangeTarget.started += SelectNewTarget;
        inputs.CharacterControlls.Fire.started += OnFire;
    }
    private void FixedUpdate()
    {
        if (!pv.IsMine) return;
        SelectedTarget();
            
    }

    void OnFire(InputAction.CallbackContext context)
    {
        if(targetObj != null)
        {
            Vector3 dir = (targetObj.transform.position = transform.position).normalized;

            GameObject temp = PhotonNetwork.Instantiate(Path.Combine("Fireball"),spawnPosition.position,Quaternion.identity);
            Physics.IgnoreCollision(temp.GetComponent<Collider>(), GetComponent<Collider>());
            temp.GetComponent<Bullet>().StartMove(dir);
          
            
        }
    }
}
