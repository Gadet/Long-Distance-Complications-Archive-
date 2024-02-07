using System;
using UnityEngine;

// Token: 0x02000004 RID: 4
public class PlayerBehaviour : MonoBehaviour
{
	// Token: 0x0600000E RID: 14 RVA: 0x00002518 File Offset: 0x00000718
	private void Start()
	{
		Application.targetFrameRate = 60;
		this.movementStatus = "Normal";
		this.selfRigid = base.GetComponent<Rigidbody2D>();
		this.selfCollider = base.GetComponent<BoxCollider2D>();
		this.selfAnimator = base.GetComponent<Animator>();
		this.selfAnimator.SetBool("facingRight", true);
		this.grappleReturned = true;
		this.terrainMask = LayerMask.GetMask(new string[]
		{
			"Terrain"
		});
	}

	// Token: 0x0600000F RID: 15 RVA: 0x00002594 File Offset: 0x00000794
	private void Update()
	{
		this.jumpKeyDown |= Input.GetButtonDown("Jump");
		this.jumpKeyUp |= Input.GetButtonUp("Jump");
		this.resetKeyDown |= Input.GetKeyDown(KeyCode.R);
		this.shootKeyDown |= Input.GetButtonDown("Fire1");
	}

	// Token: 0x06000010 RID: 16 RVA: 0x000025FC File Offset: 0x000007FC
	private void FixedUpdate()
	{
		Vector2 vector = new Vector2(base.transform.position.x - this.selfCollider.size.x * base.transform.localScale.x / 2f + 0.01f, base.transform.position.y - this.selfCollider.size.y * base.transform.localScale.y / 2f);
		Vector2 vector2 = new Vector2(base.transform.position.x + this.selfCollider.size.x * base.transform.localScale.x / 2f - 0.01f, base.transform.position.y - this.selfCollider.size.y * base.transform.localScale.y / 2f - 0.01f);
		Debug.DrawLine(vector, vector2, Color.yellow);
		if (Physics2D.OverlapArea(vector, vector2, this.terrainMask) != null)
		{
			this.selfAnimator.SetBool("isGrounded", true);
			this.isGrounded = true;
		}
		else
		{
			this.selfAnimator.SetBool("isGrounded", false);
			this.isGrounded = false;
		}
		string a = this.movementStatus;
		if (!(a == "Normal"))
		{
			if (!(a == "Hanging"))
			{
				if (!(a == "Dragging"))
				{
					this.moveNormally();
				}
				else if (!this.isGrounded)
				{
					this.retractGrapple();
					this.movementStatus = "Normal";
				}
				else
				{
					Vector2 to = base.transform.position - this.createdGrapple.transform.position;
					Debug.Log("MAG: " + to.magnitude.ToString() + "| ANG: " + Vector2.SignedAngle(new Vector2(1f, 0f), to).ToString());
					if (to.magnitude >= this.createdGrapple.GetComponent<DistanceJoint2D>().distance)
					{
						float num = Vector2.SignedAngle(new Vector2(1f, 0f), to);
						if ((num > 90f || num < -90f) && Input.GetAxisRaw("Horizontal") < 0f)
						{
							this.moveDragging();
						}
						else if ((num < 90f || num > -90f) && Input.GetAxisRaw("Horizontal") > 0f)
						{
							this.moveDragging();
						}
						else
						{
							this.moveNormally();
						}
					}
					else
					{
						this.moveNormally();
					}
					if (this.shootKeyDown)
					{
						this.retractGrapple();
					}
				}
			}
			else
			{
				if (Input.GetButton("Fire2"))
				{
					this.createdGrapple.GetComponent<DistanceJoint2D>().distance -= this.pullSpeed;
				}
				if (!this.isGrounded)
				{
					this.moveHanging();
				}
				else
				{
					this.moveNormally();
				}
				if (this.shootKeyDown)
				{
					this.retractGrapple();
				}
			}
		}
		else
		{
			this.moveNormally();
			if (this.grappleReturned && this.shootKeyDown)
			{
				this.shootGrapple();
			}
		}
		if (this.resetKeyDown)
		{
			Debug.Log("Retrying Area");
			this.resetKeyDown = false;
			this.retry();
		}
		this.jumpKeyUp = false;
		this.jumpKeyDown = false;
		this.resetKeyDown = false;
		this.shootKeyDown = false;
		this.updateLineRenderer();
		if (this.selfRigid.velocity.x == 0f)
		{
			this.selfAnimator.SetBool("isIdle", true);
		}
		else if (this.selfRigid.velocity.x > 0f)
		{
			this.selfAnimator.SetBool("isIdle", false);
			this.selfAnimator.SetBool("facingRight", true);
		}
		else
		{
			this.selfAnimator.SetBool("isIdle", false);
			this.selfAnimator.SetBool("facingRight", false);
		}
		this.selfAnimator.SetFloat("velocityY", this.selfRigid.velocity.y);
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00002A30 File Offset: 0x00000C30
	private void moveNormally()
	{
		Vector2 velocity = this.selfRigid.velocity;
		velocity.x = 0f;
		float axisRaw = Input.GetAxisRaw("Horizontal");
		if ((axisRaw > 0f && this.selfRigid.velocity.x <= this.speedWalking) || (axisRaw < 0f && this.selfRigid.velocity.x >= this.speedWalking * -1f))
		{
			velocity.x = axisRaw * this.speedWalking;
		}
		if (this.jumpKeyDown && this.isGrounded)
		{
			Debug.Log("Player jumped off ground");
			velocity.y = this.jumpForce;
			if (this.movementStatus == "Dragging")
			{
				this.retractGrapple();
			}
		}
		else if (this.jumpKeyUp && !this.isGrounded)
		{
			Debug.Log("Ended jump early. Velocity: " + this.selfRigid.velocity.y.ToString());
			if (0f < this.selfRigid.velocity.y)
			{
				velocity.y = -0.1f;
			}
		}
		this.selfRigid.velocity = velocity;
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00002B5C File Offset: 0x00000D5C
	private void moveHanging()
	{
		Vector2 velocity = this.selfRigid.velocity;
		float axisRaw = Input.GetAxisRaw("Horizontal");
		if ((axisRaw > 0f && this.selfRigid.velocity.x < this.speedHanging) || (axisRaw < 0f && this.selfRigid.velocity.x > this.speedHanging * -1f))
		{
			this.selfRigid.AddForce(new Vector2(this.speedHanging * axisRaw, 0f));
		}
		if (this.jumpKeyDown)
		{
			this.retractGrapple();
			if (this.selfRigid.velocity.y < this.jumpForce)
			{
				velocity.y = this.jumpForce;
			}
		}
		this.selfRigid.velocity = velocity;
	}

	// Token: 0x06000013 RID: 19 RVA: 0x00002C24 File Offset: 0x00000E24
	private void moveDragging()
	{
		this.moveNormally();
		float axisRaw = Input.GetAxisRaw("Horizontal");
		Rigidbody2D connectedBody = this.createdGrapple.GetComponent<FixedJoint2D>().connectedBody;
		if ((axisRaw > 0f && connectedBody.velocity.x <= this.speedWalking) || (axisRaw < 0f && connectedBody.velocity.x >= this.speedWalking * -1f))
		{
			connectedBody.velocity = new Vector2(axisRaw * this.speedWalking * this.speedDragging, connectedBody.velocity.y);
		}
	}

	// Token: 0x06000014 RID: 20 RVA: 0x00002CB4 File Offset: 0x00000EB4
	private void shootGrapple()
	{
		this.grappleReturned = false;
		this.createdGrapple = Object.Instantiate<GameObject>(this.grapplePrefab, base.transform.position, new Quaternion(0f, 0f, 0f, 0f), base.transform.parent.transform);
		this.createdGrapple.GetComponent<GrappleHeadBehaviour>().grappleOwner = base.gameObject;
		Physics2D.IgnoreCollision(base.GetComponent<BoxCollider2D>(), this.createdGrapple.GetComponent<BoxCollider2D>());
		this.createdGrapple.GetComponent<GrappleHeadBehaviour>().shootGrapple();
	}

	// Token: 0x06000015 RID: 21 RVA: 0x00002D49 File Offset: 0x00000F49
	private void retractGrapple()
	{
		this.movementStatus = "Normal";
		if (!this.grappleReturned)
		{
			this.createdGrapple.GetComponent<GrappleHeadBehaviour>().retractGrapple();
		}
	}

	// Token: 0x06000016 RID: 22 RVA: 0x00002D70 File Offset: 0x00000F70
	private void retry()
	{
		this.selfRigid.velocity = Vector2.zero;
		for (int i = 0; i < this.currentArea.GetComponentsInChildren<Transform>().Length - 1; i++)
		{
			if (this.currentArea.GetChild(i).CompareTag("Respawn"))
			{
				base.transform.position = this.currentArea.GetChild(i).transform.position;
				break;
			}
		}
		this.retractGrapple();
		this.currentArea.gameObject.GetComponent<AreaBehaviour>().resetArea();
	}

	// Token: 0x06000017 RID: 23 RVA: 0x00002E00 File Offset: 0x00001000
	private void OnTriggerEnter2D(Collider2D collision)
	{
		string tag = collision.gameObject.tag;
		if (!(tag == "AreaBoundary"))
		{
			if (!(tag == "DeathZone"))
			{
				return;
			}
			this.retry();
			return;
		}
		else
		{
			if (this.forcedControl)
			{
				this.forcedControl = false;
				return;
			}
			if (!this.grappleReturned)
			{
				this.retractGrapple();
			}
			this.currentArea = collision.transform;
			Vector3 position = collision.transform.position;
			position.z = -2f;
			Camera.main.transform.position = position;
			Camera.main.orthographicSize = 4.5f;
			Vector2 to = base.transform.position - collision.transform.position;
			float num = Vector2.SignedAngle(new Vector2(1f, 0f), to);
			BoxCollider2D component = collision.gameObject.GetComponent<BoxCollider2D>();
			float num2 = 57.29578f * Mathf.Atan(component.size.y / 2f / (component.size.x / 2f));
			Vector2 v = base.transform.position;
			if (-1f * (180f - num2) > num || num > 180f - num2)
			{
				v.x += base.transform.localScale.x;
			}
			else if (-1f * (180f - num2) < num && num < -1f * num2)
			{
				v.y += base.transform.localScale.y;
			}
			else if (180f - num2 > num && num > num2)
			{
				v.y -= base.transform.localScale.y;
			}
			else
			{
				v.x -= base.transform.localScale.x;
			}
			Debug.Log("SW ANG: " + (-1f * (180f - num2)).ToString() + " | NW ANG: " + (180f - num2).ToString());
			Debug.Log(string.Concat(new string[]
			{
				"PLR: ",
				base.transform.position.ToString(),
				" | AREA: ",
				collision.gameObject.transform.position.ToString(),
				" | ANG: ",
				num.ToString()
			}));
			base.transform.position = v;
			return;
		}
	}

	// Token: 0x06000018 RID: 24 RVA: 0x000030A4 File Offset: 0x000012A4
	public void updateLineRenderer()
	{
		LineRenderer component = base.GetComponent<LineRenderer>();
		if (!this.grappleReturned)
		{
			component.enabled = true;
			component.SetPosition(0, base.transform.position);
			component.SetPosition(1, this.createdGrapple.transform.position);
			return;
		}
		component.enabled = false;
	}

	// Token: 0x0400000D RID: 13
	private Rigidbody2D selfRigid;

	// Token: 0x0400000E RID: 14
	private Animator selfAnimator;

	// Token: 0x0400000F RID: 15
	public GameObject grapplePrefab;

	// Token: 0x04000010 RID: 16
	private GameObject createdGrapple;

	// Token: 0x04000011 RID: 17
	public float grappleSpeed;

	// Token: 0x04000012 RID: 18
	public bool grappleReturned;

	// Token: 0x04000013 RID: 19
	public Transform respawnLocation;

	// Token: 0x04000014 RID: 20
	public string movementStatus;

	// Token: 0x04000015 RID: 21
	public bool isGrounded;

	// Token: 0x04000016 RID: 22
	public float speedWalking;

	// Token: 0x04000017 RID: 23
	public float speedDragging;

	// Token: 0x04000018 RID: 24
	public float speedHanging;

	// Token: 0x04000019 RID: 25
	public float jumpForce;

	// Token: 0x0400001A RID: 26
	public float pullSpeed;

	// Token: 0x0400001B RID: 27
	public bool forcedControl;

	// Token: 0x0400001C RID: 28
	public Transform currentArea;

	// Token: 0x0400001D RID: 29
	private bool jumpKeyDown;

	// Token: 0x0400001E RID: 30
	private bool jumpKeyUp;

	// Token: 0x0400001F RID: 31
	private bool resetKeyDown;

	// Token: 0x04000020 RID: 32
	private bool shootKeyDown;

	// Token: 0x04000021 RID: 33
	private LayerMask terrainMask;

	// Token: 0x04000022 RID: 34
	private BoxCollider2D selfCollider;
}
