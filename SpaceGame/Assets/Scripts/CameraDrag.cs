using UnityEngine;
using System.Collections;


public class CameraDrag : MonoBehaviour
{
	public float dragSpeed = 2;
	private Vector3 dragOrigin;
	public float maxX = 50f;
	public float maxY = 50f;
	public float minX = -50f;
	public float minY = -25f;
	
	private Camera myCamera;
	void Start (){
		myCamera = GetComponent<Camera>();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(1))
		{
			dragOrigin = Input.mousePosition;
			return;
		}
		
		if (!Input.GetMouseButton(1) ) return;
		
		Vector3 pos = myCamera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
		Vector2 move = new Vector2(pos.x * dragSpeed, pos.y * dragSpeed);

		bool outOfBoundsX = true;
		bool outOfBoundsY = true;
		if (myCamera.isActiveAndEnabled){
			if (move.x > 0f)
			{
				if(this.transform.position.x < maxX)
				{
					outOfBoundsX = false;
				}
			}
			else{
				if(this.transform.position.x > minY)
				{
					outOfBoundsX = false;	
				}
			}
			if (move.y > 0f)
			{
				if(this.transform.position.y < maxY)
				{
					outOfBoundsY = false;
				}
			}
			else{
				if(this.transform.position.y > minY)
				{
					outOfBoundsY = false;
				}
			}

			if (!outOfBoundsX && !outOfBoundsY){
				this.transform.Translate(move, Space.World);
			}
		}
	}
	
	
}
