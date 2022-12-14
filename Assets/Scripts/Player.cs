using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PrimaryWeapon[] weapons;

    float moveSpeed = 3;
    float boostSpeed = 3;
    float leftBoundary = 1.5f;
    float rightBoundary = 16f;
    float topBoundary = 9;
    float bottomBoundary = 1;

    bool moveUp;
    bool moveDown;
    bool moveLeft;
    bool moveRight;
    bool speedBoost;
    bool shoot;

    GameObject shield;
    int powerUpPrimaryWeaponLevel = 0;

    // Start is called before the first frame update
    void Start()
    {
        shield = transform.Find("Shield").gameObject;
        DeactivateShield();
       weapons = transform.GetComponentsInChildren<PrimaryWeapon>(); 
       foreach (PrimaryWeapon weapon in weapons)
       {
          weapon.isActive = true;
          if (weapon.powerUpLevelRequirement != powerUpPrimaryWeaponLevel)
          {
            weapon.gameObject.SetActive(false);
          }
       }
    }

    // Update is called once per frame
    void Update()
    {
        moveUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        moveDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        moveLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        moveRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        speedBoost = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        shoot = Input.GetKeyDown(KeyCode.Space);

        if (shoot)
        {
            shoot = false;
            foreach (PrimaryWeapon weapon in weapons)
            {
                if (weapon.gameObject.activeSelf)
                {
                    weapon.Shoot();
                }
            }
        }       
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        float moveAmount = moveSpeed * Time.fixedDeltaTime;
        if (speedBoost)
        {
            moveAmount *= boostSpeed;
        }
        
        Vector2 move = Vector2.zero;

        if (moveUp)
        {
            move.y += moveAmount;
        }

        if (moveDown)
        {
            move.y -= moveAmount;
        }

        if (moveLeft)
        {
            move.x -= moveAmount;
        }

        if (moveRight)
        {
            move.x += moveAmount;
        }

        float moveMagnitude = Mathf.Sqrt(move.x * move.x + move.y * move.y);

        if (moveMagnitude > moveAmount)
        {
            float ratio = moveAmount / moveMagnitude;
            move *= ratio;
        }

        pos += move;

        if (pos.x <= leftBoundary)
        {
            pos.x = leftBoundary;
        }

        if (pos.x >= rightBoundary)
        {
            pos.x = rightBoundary;
        }

        if (pos.y <= bottomBoundary)
        {
            pos.y = bottomBoundary;
        }

        if (pos.y >= topBoundary)
        {
            pos.y = topBoundary;
        }

        transform.position = pos;
    }

    void ActivateShield()
    {
        shield.SetActive(true);
    }

    void DeactivateShield()
    {
        shield.SetActive(false);
    }

    bool HasShield()
    {
        return shield.activeSelf;
    }

    void AddPrimaryWeapons()
    {
        powerUpPrimaryWeaponLevel++;
        foreach (PrimaryWeapon weapon in weapons)
        {
            if (weapon.powerUpLevelRequirement == powerUpPrimaryWeaponLevel)
            {
                weapon.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Projectile projectile = collision.GetComponent<Projectile>();

        if (projectile != null && projectile.isEnemy)
        {
            if (HasShield())
            {
                DeactivateShield();
            }
            else 
            {
                Destroy(gameObject);
            }
            Destroy(projectile.gameObject);
        }

        Destructable destructable = collision.GetComponent<Destructable>();

        if (destructable != null)
        {
            if (HasShield())
            {
                DeactivateShield();
            }
            else 
            {
                Destroy(gameObject);
            }
            Destroy(destructable.gameObject);
        }

        PowerUp powerUp = collision.GetComponent<PowerUp>();
        if (powerUp)
        {
            if (powerUp.activateShield)
            {
                ActivateShield();
            }
            if (powerUp.addPrimaryWeapons)
            {
                AddPrimaryWeapons();
            }
            Destroy(powerUp.gameObject);
        }
    }
}
