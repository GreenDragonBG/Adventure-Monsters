using System;
using UnityEngine;
public class EnemyWallCheck : MonoBehaviour
{
    private GameObject enemy;
    [SerializeField] private bool isBehind;

    private void Start()
    {
        enemy = this.transform.parent.gameObject;
    }

    public  void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag != "Trigger")
        {
            // if hit a player out of cooldown time Attack
            if (other.tag == "Player" && (Time.time - enemy.GetComponent<EnemyScript>().lastTimeHitAPlayer) >= 0.5f)
            {
                enemy.GetComponent<EnemyScript>().lastTimeHitAPlayer = Time.time;
                enemy.GetComponent<EnemyScript>().attacking = true;
                if (isBehind)
                {
                    // if hit a player with back Notifiy enemy
                    enemy.GetComponent<EnemyScript>().hitFromBihind = true;
                }
                else
                {
                    // if hit a player with front Notifiy enemy
                    enemy.GetComponent<EnemyScript>().hitFromBihind = false;
                }
            }
            // if hit a wall out of cooldown rotate
            if ((Time.time - enemy.GetComponent<EnemyScript>().lastTimeHitAWall) >= 0.2)
            {
                enemy.GetComponent<EnemyScript>().lastTimeHitAWall = Time.time;
                enemy.GetComponent<EnemyScript>().ReverseDirection();
            }
        }
    }
}
