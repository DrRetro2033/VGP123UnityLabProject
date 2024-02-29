using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarProjectile : MonoBehaviour
{
    public float speed = 3.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * Time.deltaTime * speed;
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision != null)
		{
			if (collision.gameObject.CompareTag("Enemy"))
			{
				Destroy(collision.gameObject);
				Destroy(gameObject);
			}
		}
	}
}
