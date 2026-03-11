using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

public class PLayerSettings : MonoBehaviour
{
    [SerializeField] int health = 100;
    int maxHealth;
    [SerializeField] Slider healthBar;
    PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        maxHealth = health;
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }

    [PunRPC]
    public void UpdateHealth(int value)
    {
        health -= value;
        if(health <= 0)
        {
            health = maxHealth;
            GetComponentInChildren<PlayerController>().Respawn();
        }
        healthBar.value = health;
    }

    public void TakeDamage(int damage)
    {
        pv.RPC(nameof(UpdateHealth), RpcTarget.All, damage);
    }
    
}
