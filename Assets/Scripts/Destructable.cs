using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    bool onScreen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < 17.0f)
        {
            onScreen = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!onScreen)
        {
            return;
        }

        Projectile projectile = collision.GetComponent<Projectile>();

        if (projectile != null)
        {
            Destroy(gameObject);
            Destroy(projectile.gameObject);
        }
    }
}
