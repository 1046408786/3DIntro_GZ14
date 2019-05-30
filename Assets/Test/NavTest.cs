using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavTest : MonoBehaviour
{
    GameObject player;
    NavMeshAgent nav;
    Animator anim;

    void Start()
    {
        player = GameObject.Find("Player");
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!nav.enabled) return;

        nav.SetDestination(player.transform.position);

        if (nav.isStopped == true || nav.remainingDistance < nav.stoppingDistance)

        {
            anim.SetBool("Move", false);
        }
        else
        {
            anim.SetBool("Move", true);
        }
    }

    public void Die()
    {
        anim.SetTrigger("Dead");
        nav.isStopped = true;
    }

    void StartSinking()
    {
        nav.enabled = false;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 2f);
    }
}
