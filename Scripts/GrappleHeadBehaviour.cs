using System;
using UnityEngine;

// Token: 0x02000003 RID: 3
public class GrappleHeadBehaviour : MonoBehaviour
{
	// Token: 0x06000005 RID: 5 RVA: 0x00002119 File Offset: 0x00000319
	private void Start()
	{
		this.flightTimer = 0.0;
		this.isFlying = true;
		this.selfRigid = base.GetComponent<Rigidbody2D>();
		this.selfFixedJoint = base.GetComponent<FixedJoint2D>();
		this.selfDistanceJoint = base.GetComponent<DistanceJoint2D>();
	}

	// Token: 0x06000006 RID: 6 RVA: 0x00002155 File Offset: 0x00000355
	private void OnDestroy()
	{
		this.grappleOwner.GetComponent<PlayerBehaviour>().grappleReturned = true;
		this.grappleOwner.GetComponent<PlayerBehaviour>().movementStatus = "Normal";
	}

	// Token: 0x06000007 RID: 7 RVA: 0x00002180 File Offset: 0x00000380
	private void Update()
	{
		if (this.isFlying)
		{
			this.flightTimer += 1.0;
			if (this.flightTimer >= 90.0)
			{
				this.isFlying = false;
				this.retractGrapple();
				return;
			}
			this.selfRigid.velocity = this.movementVelocity;
		}
	}

	// Token: 0x06000008 RID: 8 RVA: 0x000021DC File Offset: 0x000003DC
	public void shootGrapple()
	{
		this.selfRigid = base.GetComponent<Rigidbody2D>();
		Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
		vector.z = 0f;
		float num = Mathf.Atan2(vector.y - base.transform.position.y, vector.x - base.transform.position.x);
		float z = 57.295776f * num;
		this.isFlying = true;
		base.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, z));
		this.movementVelocity = new Vector2(Mathf.Cos(num) * this.grappleSpeed, Mathf.Sin(num) * this.grappleSpeed);
		this.selfRigid.velocity = this.movementVelocity;
		Debug.Log(string.Concat(new string[]
		{
			"Shooting grapple DEG: ",
			z.ToString(),
			"| COS: ",
			Mathf.Cos(num).ToString(),
			"| SIN: ",
			Mathf.Sin(num).ToString()
		}));
	}

	// Token: 0x06000009 RID: 9 RVA: 0x00002318 File Offset: 0x00000518
	public void retractGrapple()
	{
		this.selfFixedJoint.enabled = false;
		this.selfFixedJoint.connectedBody = null;
		this.selfDistanceJoint.enabled = false;
		this.selfRigid.velocity = new Vector2(0f, 0f);
		this.attached = false;
		this.grappleOwner.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0600000A RID: 10 RVA: 0x00002388 File Offset: 0x00000588
	private void attachToObject(GameObject attachingObj)
	{
		this.selfFixedJoint.enabled = true;
		this.selfFixedJoint.connectedBody = attachingObj.GetComponent<Rigidbody2D>();
		this.selfDistanceJoint.enabled = true;
		this.selfDistanceJoint.connectedBody = this.grappleOwner.GetComponent<Rigidbody2D>();
		this.selfRigid.velocity = new Vector2(0f, 0f);
		this.isFlying = false;
		Debug.Log("Grapple connected to " + attachingObj.ToString());
	}

	// Token: 0x0600000B RID: 11 RVA: 0x0000240C File Offset: 0x0000060C
	private void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log("Grapple hit something. " + collision.gameObject.tag);
		string tag = collision.gameObject.tag;
		if (!(tag == "Weight"))
		{
			if (!(tag == "HangAnchor"))
			{
				this.retractGrapple();
				return;
			}
			this.attachToObject(collision.gameObject);
			this.grappleOwner.GetComponent<PlayerBehaviour>().movementStatus = "Hanging";
			return;
		}
		else
		{
			if (!this.grappleOwner.GetComponent<PlayerBehaviour>().isGrounded)
			{
				Debug.Log("Cannot hook weight while not grounded");
				this.retractGrapple();
				return;
			}
			collision.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
			this.attachToObject(collision.gameObject);
			this.grappleOwner.GetComponent<Rigidbody2D>().constraints = (RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation);
			this.grappleOwner.GetComponent<PlayerBehaviour>().movementStatus = "Dragging";
			return;
		}
	}

	// Token: 0x0600000C RID: 12 RVA: 0x000024FB File Offset: 0x000006FB
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("AreaBoundary"))
		{
			this.retractGrapple();
		}
	}

	// Token: 0x04000004 RID: 4
	public GameObject grappleOwner;

	// Token: 0x04000005 RID: 5
	private Rigidbody2D selfRigid;

	// Token: 0x04000006 RID: 6
	private FixedJoint2D selfFixedJoint;

	// Token: 0x04000007 RID: 7
	private DistanceJoint2D selfDistanceJoint;

	// Token: 0x04000008 RID: 8
	public float grappleSpeed;

	// Token: 0x04000009 RID: 9
	public bool attached;

	// Token: 0x0400000A RID: 10
	private bool isFlying;

	// Token: 0x0400000B RID: 11
	private double flightTimer;

	// Token: 0x0400000C RID: 12
	private Vector2 movementVelocity;
}
