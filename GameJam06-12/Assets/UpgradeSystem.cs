using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class UpgradeSystem : MonoBehaviour
{
#if USE_LISTS
	// Infinite - auto, go up by one.
	[HideInInspector]
	public List<byte> ProteinLevelNo = new List<byte>();
	// Infinite - click, go up by one.
	[HideInInspector]
	public List<byte> WeightLevelNo = new List<byte>();
#else
	[HideInInspector]	
	public ulong ProteinLevelNo;
	[HideInInspector]
	public ulong WeightLevelNo;
#endif
	// 10 max.
	[HideInInspector]
	public ulong CreatineLevelNo = 1;
	// 10 max - increase by 0.3.
	[HideInInspector]
	public int SpeedLevelNo = 0;
	// 5 max.
	[HideInInspector]
	public ulong MembershipNo = 0;

	// Use this for initialization
	void Start ()
	{
		// 357 x 120

#if USING_LISTS
		ProteinLevelNo.Add (1);
		WeightLevelNo.Add (1);
#else
		ProteinLevelNo = 1;
		WeightLevelNo = 1;
#endif

	}

	// Update is called once per frame
	void Update ()
	{
	}

	void OnGUI()
	{		
		/*if (GUI.Button(proteinRect, "Protein"))
		{
			BuyProtein();
		}
		if (GUI.Button(weightRect, "Weight"))
		{
			BuyWeight();
		}
		if (GUI.Button(creatineRect, "Creatine"))
		{
			BuyCreatine();
		}
		if (GUI.Button(speedRect, "Speed"))
		{
			BuySpeed();
		}
		if (GUI.Button(membershipRect, "Membership"))
		{
			BuyMembership();
		}*/
	}

	public void BuyProtein()
	{
#if USING_LISTS
		addRecursiveProtein (ProteinLevelNo.Count - 1);
#else
		ProteinLevelNo++;
#endif
	}

	public void BuyWeight()
	{
#if USING_LISTS
		addRecursiveWeight (WeightLevelNo.Count - 1);
#else
		WeightLevelNo++;
#endif
	}

#if USING_LISTS
	private void addRecursiveProtein(int position)
	{
		ProteinLevelNo [position]++;

		if (ProteinLevelNo[position] >= 10)
		{
			if (position == 0)
			{
				ProteinLevelNo[position] = 0;
				ProteinLevelNo.Insert(0, 1);
			}
			else
			{
				ProteinLevelNo[position] = 0;
				addRecursiveProtein(position - 1);
			}
		}
	}

	private void addRecursiveWeight(int position)
	{
		WeightLevelNo [position]++;
		
		if (WeightLevelNo[position] >= 10)
		{
			if (position == 0)
			{
				WeightLevelNo[position] = 0;
				WeightLevelNo.Insert(0, 1);
			}
			else
			{
				WeightLevelNo[position] = 0;
				addRecursiveWeight(position - 1);
			}
		}
	}
#endif

	public void BuyCreatine()
	{
		CreatineLevelNo++;
	}

	public void BuySpeed()
	{
		SpeedLevelNo++;
	}

	public void BuyMembership()
	{
		MembershipNo++;
	}
}

public struct UpgradeButton
{
	public Rect ImageRect;
	public Texture2D Image;

	public Action Method;

	public UpgradeButton(Rect imageRect, Texture2D image, Action method)
	{
		ImageRect = imageRect;

		Image = image;

		Method = method;
	}
}
