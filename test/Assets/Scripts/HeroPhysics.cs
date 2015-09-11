using UnityEngine;
using System.Collections;

public class HeroPhysics : MonoBehaviour {

	public bool jump = false;
	public bool facingRight = true;

	public float moveforce = 365f;
	public float maxSpeed = 5f;
	public float jumpforce = 1000f;
	public Transform groundCheck;

	private bool grounded = false;
	private Animator anim;
	private Rigidbody2D rb2d;

	// Use this for initialization
	void Awake () {
		anim = GetComponent<Animator> ();
		rb2d = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));
		if (grounded && Input.GetButtonDown("Jump")) {
			jump = true;
		}
	}

	void FixedUpdate () {
		float h = Input.GetAxis("Horizontal");
		anim.SetFloat("Speed", Mathf.Abs(h));

		if (h * rb2d.velocity.x < maxSpeed) {
			rb2d.AddForce(h * moveforce * Vector2.right);
		}

		if (Mathf.Abs(rb2d.velocity.x) > maxSpeed) {
			rb2d.velocity = new Vector2 (Mathf.Sign(rb2d.velocity.x), rb2d.velocity.y);
		}

		if (h > 0 && !facingRight){
			Flip();
		} else if (h < 0 && facingRight){
			Flip();
		}

		if (jump){
			anim.SetTrigger("Jump");
			rb2d.AddForce(new Vector2(0, jumpforce));
			jump = false;
		}

	}

	void Flip (){
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
