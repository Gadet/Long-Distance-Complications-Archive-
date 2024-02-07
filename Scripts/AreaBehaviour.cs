using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000002 RID: 2
public class AreaBehaviour : MonoBehaviour
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	private void Start()
	{
	}

	// Token: 0x06000002 RID: 2 RVA: 0x00002054 File Offset: 0x00000254
	private void Awake()
	{
		this.childObjects = new List<Transform>();
		this.childStartPositions = new List<Vector2>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			this.childStartPositions.Add(base.transform.GetChild(i).transform.position);
			this.childObjects.Add(base.transform.GetChild(i));
		}
	}

	// Token: 0x06000003 RID: 3 RVA: 0x000020CC File Offset: 0x000002CC
	public void resetArea()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			this.childObjects[i].position = this.childStartPositions[i];
		}
	}

	// Token: 0x04000001 RID: 1
	public bool areaSolved;

	// Token: 0x04000002 RID: 2
	private List<Vector2> childStartPositions;

	// Token: 0x04000003 RID: 3
	private List<Transform> childObjects;
}
