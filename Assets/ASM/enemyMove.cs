using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyMove : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent agent;
    private EnemyManager emanager;
    public bool isDeadBool;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("PLAYER");
        emanager = GetComponent<EnemyManager>();
        isDeadBool = false;
        agent.speed = emanager.speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDeadBool) return;
        if (Vector3.Distance(transform.position, player.transform.position) > emanager.attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    public void isDead()
    {
        isDeadBool = true;
        agent.isStopped = true;
    }
}
