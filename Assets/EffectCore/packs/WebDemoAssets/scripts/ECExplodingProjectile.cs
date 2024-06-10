﻿using UnityEngine;

public class ECExplodingProjectile : MonoBehaviour
{
    public GameObject impactPrefab; // Prefab hiệu ứng khi va chạm
    public GameObject explosionPrefab; // Prefab hiệu ứng nổ
    public float thrust; // Lực đẩy của viên đạn
    public int damage; // Số lượng sát thương của viên đạn
    public float critDamageMultiplier = 1.3f; // Hệ số sát thương crit

    private PlayerManager playerManager; // Tham chiếu đến PlayerManager

    public Rigidbody thisRigidbody;

    public GameObject particleKillGroup;
    private Collider thisCollider;

    public bool LookRotation = true;
    public bool Missile = false;
    public Transform missileTarget;
    public float projectileSpeed;
    public float projectileSpeedMultiplier;

    public bool ignorePrevRotation = false;

    public bool explodeOnTimer = false;
    public float explosionTimer;
    float timer;

    private Vector3 previousPosition;

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        thisRigidbody = GetComponent<Rigidbody>();
        if (Missile)
        {
            missileTarget = GameObject.FindWithTag("Target").transform;
        }
        thisCollider = GetComponent<Collider>();
        previousPosition = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= explosionTimer && explodeOnTimer == true)
        {
            Explode();
        }
    }

    void FixedUpdate()
    {
        if (Missile)
        {
            projectileSpeed += projectileSpeed * projectileSpeedMultiplier;
            transform.LookAt(missileTarget);
            thisRigidbody.AddForce(transform.forward * projectileSpeed);
        }

        if (LookRotation && timer >= 0.05f)
        {
            transform.rotation = Quaternion.LookRotation(thisRigidbody.velocity);
        }

        CheckCollision(previousPosition);

        previousPosition = transform.position;
    }

    void CheckCollision(Vector3 prevPos)
    {
        RaycastHit hit;
        Vector3 direction = transform.position - prevPos;
        Ray ray = new Ray(prevPos, direction);
        float dist = Vector3.Distance(transform.position, prevPos);
        if (Physics.Raycast(ray, out hit, dist))
        {
            transform.position = hit.point;
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            Vector3 pos = hit.point;

            GameObject bullet = Instantiate(impactPrefab, pos, rot); // Tạo hiệu ứng va chạm
            bullet.tag = "bullet";

            if (hit.collider.CompareTag("Enemy"))
        {
            // Lấy sát thương và tỷ lệ crit từ PlayerManager
            float baseDamage = playerManager.damage;
            float critRate = playerManager.critRate;
            float critDamage = playerManager.critDamage;

            // Tính toán sát thương cơ bản và crit
            float finalDamage = CalculateDamage(baseDamage, critRate, critDamage);
            hit.collider.GetComponent<EnemyManager>().TakeDamage(finalDamage);
        }
        

            if (!explodeOnTimer && Missile == false)
            {
                Destroy(gameObject);
            }
            else if (Missile == true)
            {
                thisCollider.enabled = false;
                particleKillGroup.SetActive(false);
                thisRigidbody.velocity = Vector3.zero;
                Destroy(gameObject, 5);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "FX")
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
            if (ignorePrevRotation)
            {
                rot = Quaternion.Euler(0, 0, 0);
            }
            Vector3 pos = contact.point;
            Instantiate(impactPrefab, pos, rot);
            if (!explodeOnTimer && Missile == false)
            {
                Destroy(gameObject);
            }
            else if (Missile == true)
            {
                thisCollider.enabled = false;
                particleKillGroup.SetActive(false);
                thisRigidbody.velocity = Vector3.zero;
                Destroy(gameObject, 5);
            }
        }
    }

    void Explode()
    {
        Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject);
    }
    float CalculateDamage(float baseDamage, float critRate, float critDamage)
    {
        // Kiểm tra xem crit có xảy ra không
        bool isCrit = Random.value <= critRate;
        
        // Tính toán sát thương dựa trên việc có crit hay không
        float finalDamage = isCrit ? Mathf.RoundToInt(baseDamage * critDamageMultiplier) : baseDamage;

        return finalDamage;
    }
}
