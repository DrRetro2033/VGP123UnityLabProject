using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] Transform player;
	[SerializeField] GameObject level;

	Vector3 velocity = Vector3.zero;
	private void LateUpdate()
	{
		Vector3 cameraPos = transform.position;
		cameraPos.x = Mathf.Clamp(player.transform.position.x, level.GetComponent<SpriteRenderer>().bounds.min.x, level.GetComponent<SpriteRenderer>().bounds.max.x);
		cameraPos.y = Mathf.Clamp(player.transform.position.y, level.GetComponent<SpriteRenderer>().bounds.min.y, level.GetComponent<SpriteRenderer>().bounds.max.y);
		transform.position = Vector3.SmoothDamp(transform.position,cameraPos,ref velocity,1.0f);
	}
}
