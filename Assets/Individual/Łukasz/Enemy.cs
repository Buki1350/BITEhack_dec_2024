using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject Player;
    public NavMeshAgent agent;
    
    private void Update()
    {
        agent.baseOffset = GetComponent<Collider>().bounds.size.y / 2;
        agent.SetDestination(Player.transform.position);
    }
}