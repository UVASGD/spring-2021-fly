using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkey : MonoBehaviour
{
    [Range(0f, 1f)]
    public float spawnProbability = 0.2f;
    
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform throwPoint;
    public float throwRate = 1f;
    public float projectileSpeed = 10f;
    public float leading = 0f;
    public float heightBias = 1f;
    public float maxDistance = 100f;

    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ThrowAtPlayer", Random.Range(0f, throwRate), throwRate);   
    }

    public void ThrowAtPlayer()
    {
        if (GameManager.instance == null || GameManager.instance.playerManager.activePlayer == null) return;
        Rigidbody player = GameManager.instance.playerManager.activePlayer.GetComponent<Rigidbody>();
        if (player == null || GameManager.instance.runManager.runStarted == false) return;

        GameObject obj = Instantiate(projectilePrefab);
        obj.transform.position = throwPoint.position;
        Rigidbody projectile = obj.GetComponent<Rigidbody>();

        Vector3 playerPos = player.position;
        Vector3 playerVel = player.velocity;

        Vector3 targetPosition = playerPos + leading * playerVel;
        direction = (targetPosition - transform.position);
        direction += Vector3.up * heightBias * direction.magnitude;
        Debug.DrawRay(obj.transform.position, direction, Color.yellow);
        if (direction.sqrMagnitude > maxDistance * maxDistance)
        {
            projectile.velocity = direction.normalized * projectileSpeed;
            projectile.angularVelocity = Random.insideUnitSphere * 360f;
        }
        Destroy(obj, 5f);
    }
}
