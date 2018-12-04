using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class MainCharacter : MonoBehaviour
{
	public const float ORIGINAL_WIDTH = 1280.0f;
	public const float ORIGINAL_HEIGHT = 720.0f;

	public Sprite[] AnimationSprites;

	public ButtonTextures[] SideButtonTextures;
	public MembershipTextures SideMembershipTextures;

	public Texture2D OverlayTexture;
	public Texture2D InstructionsTexture;

	public GUIStyle TextStyle;

	public GameObject[] SoundEffects;
	public GameObject MembershipSFX;
	public GameObject UpgradeSFX;

	public float AnimationLimit;

	private SpriteRenderer spriteComponent;
	private UpgradeSystem upgradeComponent;
	
	private Matrix4x4 scaleMatrix;

	private Rect[] upgradeButtonRects = new Rect[5];

	private Rect colliderRect;
	private Rect instructionsRect;
	private Rect currencyRect;
	private Rect scoreRect;
	private Rect levelNoRect;

	private GUIStyle guiStyle = new GUIStyle();

	private bool mouseDownOnPress = false;

	private int animationCounter = 0;

	private int sfxCounter = 0;
	private int sfxLimit = 5;

	private float animationTimer = 0.0f;

	private ulong currentLevel = 1;
	private ulong nextLevel = 1000;

	private ulong proteinCost = 25;
	private ulong weightCost = 10;
	private int creatineCost = 100;
	private int speedCost = 120;
	private int membershipCost = 1000;

#if USE_LISTS
	private List<byte> normalScore = new List<byte>();
	private List<byte> shopScore = new List<byte>();
#else
	private ulong normalScore = 0;
	private ulong shopScore = 0;
#endif

	void Awake()
	{
		spriteComponent = GetComponent<SpriteRenderer> ();
		upgradeComponent = GetComponent<UpgradeSystem> ();

		Vector3 scale = new Vector3 ();
		scale.x = Screen.width / ORIGINAL_WIDTH;
		scale.y = Screen.height / ORIGINAL_HEIGHT;
		scale.z = 1;
		
		scaleMatrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, scale);
		
		GUI.matrix = scaleMatrix;
	}

	// Use this for initialization
	void Start ()
	{
		Vector2 colliderBounds = GetComponent<BoxCollider2D> ().size;

		Vector3 topLeftWorld = new Vector3 (transform.position.x - (colliderBounds.x),
		                                    transform.position.y - (colliderBounds.y), 1.0f);
		Vector3 bottomRightWorld = new Vector3 (transform.position.x + (colliderBounds.x),
		                                       transform.position.y + (colliderBounds.y), 1.0f);

		Vector2 topLeftScreen = Camera.main.WorldToScreenPoint (topLeftWorld);
		Vector2 bottomRightScreen = Camera.main.WorldToScreenPoint (bottomRightWorld);

		colliderRect = new Rect (topLeftScreen.x, topLeftScreen.y, (bottomRightScreen.x - topLeftScreen.x),
		                        (bottomRightScreen.y - topLeftScreen.y));

		instructionsRect = new Rect (ORIGINAL_WIDTH - 360 - 504, 640 - 78, 504, 78);

		currencyRect = new Rect (178, 650, ORIGINAL_WIDTH - 356, 60);
		levelNoRect = new Rect (ORIGINAL_WIDTH - 215, 634, 300, 60);
		scoreRect = new Rect (357, 13, ORIGINAL_WIDTH - 356, 60);

#if USE_LISTS
		normalScore.Add (0);
		shopScore.Add (0);
#else
		normalScore = 0;
		shopScore = 1000000000000;// 0;
#endif

		float buttonWidth = 356.0f;
		float buttonHeight = 120.0f;
		float buttonBuffer = 10.0f;

		Rect buttonRect = new Rect (ORIGINAL_WIDTH - buttonWidth, 0,// buttonBuffer,
		                            buttonWidth, buttonHeight);

		for (int i = 0; i < SideButtonTextures.Length; ++i)
		{
			SideButtonTextures[i].Rectangle = buttonRect;

			buttonRect.y += buttonHeight;// + buttonBuffer;
		}

		SideMembershipTextures.Rectangle = buttonRect;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown(0) && colliderRect.Contains(new Vector2(Input.mousePosition.x,
		                                  (Screen.height - Input.mousePosition.y))))
		{
			mouseDownOnPress = true;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (mouseDownOnPress)
			{
				addClickScore();
			}

			mouseDownOnPress = false;
		}

		//addAutomaticScore ();
		animationTimer += Time.deltaTime;

		if ((upgradeComponent.SpeedLevelNo == 0 && animationTimer >= AnimationLimit) ||
			(upgradeComponent.SpeedLevelNo > 0 && animationTimer >= (AnimationLimit / (upgradeComponent.SpeedLevelNo * 1.2f))))
		{
			animationTimer = 0.0f;
			animationCounter++;

			if (animationCounter >= AnimationSprites.Length)
			{
				// Add to the score relative to upgrades.
				addAutomaticScore();

				animationCounter = 0;
			}

			// Change sprite.
			spriteComponent.sprite = AnimationSprites[animationCounter];
		}
	}

	private void OnGUI()
	{
		GUI.matrix = scaleMatrix;

		GUI.DrawTexture (new Rect (0, 0, ORIGINAL_WIDTH, ORIGINAL_HEIGHT), OverlayTexture);
		GUI.DrawTexture (instructionsRect, InstructionsTexture);

		GUI.Label (scoreRect, "" + normalScore, TextStyle);
		GUI.Label (currencyRect, "" + shopScore, TextStyle);
		GUI.Label (levelNoRect, "" + currentLevel, TextStyle);

		/*GUI.Label (new Rect (10, 0, 1000, 20), "Normal: " + normalScore);
		GUI.Label (new Rect (10, 20, 1000, 20), "Shop: " + shopScore);

		GUI.Label (new Rect (10, 40, 1000, 20), "Protein Cost: " + (proteinCost * (1 - (upgradeComponent.MembershipNo) * 0.05f)) );
		GUI.Label (new Rect (10, 60, 1000, 20), "Protein No: " + upgradeComponent.ProteinLevelNo);
		GUI.Label (new Rect (10, 80, 1000, 20), "Weight Cost: " + (weightCost * (1 - (upgradeComponent.MembershipNo) * 0.05f)) );
		GUI.Label (new Rect (10, 100, 1000, 20), "Weight No: " + upgradeComponent.WeightLevelNo);
		GUI.Label (new Rect (10, 120, 1000, 20), "Creatine Cost: " + (creatineCost * (1 - (upgradeComponent.MembershipNo) * 0.05f)) );
		GUI.Label (new Rect (10, 140, 1000, 20), "Creatine No: " + upgradeComponent.CreatineLevelNo);
		GUI.Label (new Rect (10, 160, 1000, 20), "Speed Cost: " + (speedCost * (1 - (upgradeComponent.MembershipNo) * 0.05f)) );
		GUI.Label (new Rect (10, 180, 1000, 20), "Speed No: " + upgradeComponent.SpeedLevelNo);
		GUI.Label (new Rect (10, 200, 1000, 20), "Membership Cost: " + (membershipCost * (1 - (upgradeComponent.MembershipNo) * 0.05f)) );
		GUI.Label (new Rect (10, 220, 1000, 20), "Membership No: " + upgradeComponent.MembershipNo);*/

		if (checkMoney(false, proteinCost))
		{
			if (GUI.Button(SideButtonTextures[0].Rectangle, "", guiStyle))
			{
				buyItem(false, proteinCost);
				upgradeComponent.BuyProtein();
				
				proteinCost *= 12;
				proteinCost /= 10;
			}

			GUI.DrawTexture(SideButtonTextures[0].Rectangle, SideButtonTextures[0].CanBuyTexture);
		}
		else
		{
			GUI.DrawTexture(SideButtonTextures[0].Rectangle, SideButtonTextures[0].CantBuyTexture);
		}

		if (checkMoney(false, weightCost))
		{
			if (GUI.Button(SideButtonTextures[1].Rectangle, "", guiStyle))
			{
				buyItem(false, weightCost);
				upgradeComponent.BuyWeight();
				
				weightCost *= 12;
				weightCost /= 10;
			}

			GUI.DrawTexture(SideButtonTextures[1].Rectangle, SideButtonTextures[1].CanBuyTexture);
		}
		else
		{
			GUI.DrawTexture(SideButtonTextures[1].Rectangle, SideButtonTextures[1].CantBuyTexture);
		}

		if (checkMoney(false, creatineCost) && upgradeComponent.CreatineLevelNo <= 10)
		{
			if (GUI.Button(SideButtonTextures[2].Rectangle, "", guiStyle))
			{
				buyItem(false, creatineCost);
				upgradeComponent.BuyCreatine();
				
				creatineCost *= 5;
			}

			GUI.DrawTexture(SideButtonTextures[2].Rectangle, SideButtonTextures[2].CanBuyTexture);
		}
		else if (upgradeComponent.CreatineLevelNo > 10)
		{
			GUI.DrawTexture(SideButtonTextures[2].Rectangle, SideButtonTextures[2].UnavailableTexture);
		}
		else
		{
			GUI.DrawTexture(SideButtonTextures[2].Rectangle, SideButtonTextures[2].CantBuyTexture);
		}



		if (checkMoney(false, speedCost) && upgradeComponent.SpeedLevelNo < 10)
		{
			if (GUI.Button(SideButtonTextures[3].Rectangle, "", guiStyle))
			{
				buyItem(false, speedCost);
				upgradeComponent.BuySpeed();
				
				speedCost *= 3;
			}

			GUI.DrawTexture(SideButtonTextures[3].Rectangle, SideButtonTextures[3].CanBuyTexture);
		}
		else if (upgradeComponent.SpeedLevelNo >= 10)
		{
			GUI.DrawTexture(SideButtonTextures[3].Rectangle, SideButtonTextures[3].UnavailableTexture);
		}
		else
		{
			GUI.DrawTexture(SideButtonTextures[3].Rectangle, SideButtonTextures[3].CantBuyTexture);
		}



		if (checkMoney(true, membershipCost) && upgradeComponent.MembershipNo < 4)
		{
			if (GUI.Button(SideMembershipTextures.Rectangle, "", guiStyle))
			{
				buyItem(true, membershipCost);
				upgradeComponent.BuyMembership();
				
				switch (upgradeComponent.MembershipNo)
				{
				case 0:
					membershipCost = 1000;
					break;
				case 1:
					membershipCost = 6000;
					break;
				case 2:
					membershipCost = 54000;
					break;
				case 3:
					membershipCost = 648000;
					break;
				}
			}

			if (upgradeComponent.MembershipNo < 4)
			{
				GUI.DrawTexture(SideMembershipTextures.Rectangle,
			                SideMembershipTextures.CanBuyTexture[upgradeComponent.MembershipNo]);
			}
			else
			{
				GUI.DrawTexture(SideMembershipTextures.Rectangle,
				                SideMembershipTextures.UnavailableTexture);
			}
		}
		else if (upgradeComponent.MembershipNo >= 4)
		{
			GUI.DrawTexture(SideMembershipTextures.Rectangle,
			                SideMembershipTextures.UnavailableTexture);
		}
		else
		{
			GUI.DrawTexture(SideMembershipTextures.Rectangle,
			                SideMembershipTextures.CantBuyTexture[upgradeComponent.MembershipNo]);
		}
	}

	private bool checkMoney(bool membershipCard, ulong cost)
	{
		if (!membershipCard)
		{
			if (shopScore >= (cost * (1 - (upgradeComponent.MembershipNo) * 0.05f)))
			{
				return true;
			}
		}
		else
		{
			if (shopScore >= cost)
			{
				return true;
			}
		}

		return false;
	}

	private bool checkMoney(bool membershipCard, float cost)
	{
		if (!membershipCard)
		{
			if (shopScore >= (cost * (1 - (upgradeComponent.MembershipNo) * 0.05f)))
			{
				return true;
			}
		}
		else
		{
			if (shopScore >=cost )
			{
				return true;
			}
		}
		
		return false;
	}


	private void buyItem(bool membershipCard, ulong cost)
	{
		//shopScore -= cost;

		if (membershipCard)
		{
			Instantiate(MembershipSFX);
		}
		else
		{
			Instantiate(UpgradeSFX);
		}
	}

	private void buyItem(bool membershipCard, int cost)
	{
		buyItem (membershipCard, (ulong)cost);
	}

	private void addClickScore()
	{
		addToScores (upgradeComponent.ProteinLevelNo * upgradeComponent.CreatineLevelNo);
	}

	private void addAutomaticScore()
	{
		addToScores(upgradeComponent.WeightLevelNo);

		sfxCounter++;

		if (sfxCounter >= sfxLimit + upgradeComponent.SpeedLevelNo)
		{
			sfxCounter = 0;
			Instantiate(SoundEffects[Random.Range(0, SoundEffects.Length)]);
		}
	}

	private void addToScores(ulong levelNo)
	{
		normalScore += levelNo;
		shopScore += levelNo;

		if (normalScore >= nextLevel)
		{
			currentLevel++;
			nextLevel *= 2;
		}
	}

#if USING_LISTS
	private void addToScores(List<byte> levelList)
	{
		for (int i = normalScore.Count - 1; i >= 0; --i)
		{
			if (Mathf.Abs(normalScore.Count - levelList.Count - i) < levelList.Count)
			{
				addRecursiveList(ref normalScore, i,
				                 levelList[Mathf.Abs(normalScore.Count - levelList.Count - i)]);
			}
			else
			{
				break;
			}
		}

		for (int i = shopScore.Count - 1; i >= 0; --i)
		{
			if (Mathf.Abs(shopScore.Count - levelList.Count - i) < levelList.Count)
			{
				addRecursiveList(ref shopScore, i,
				                 levelList[Mathf.Abs(shopScore.Count - levelList.Count - i)]);
			}
			else
			{
				break;
			}
		}

		if (normalScore.Count < levelList.Count)
		{
			for (int i = 0; i < levelList.Count - normalScore.Count; ++i)
			{
				normalScore.Insert(i, levelList[i]);
			}
		}

		if (shopScore.Count < levelList.Count)
		{
			for (int i = 0; i < levelList.Count - shopScore.Count; ++i)
			{
				shopScore.Insert(i, levelList[i]);
			}
		}
	}

	private void addRecursiveList(ref List<byte> list, int position, byte amount)
	{
		list [position] += amount;
		
		if (list[position] >= 10)
		{
			if (position == 0)
			{
				list[position] = 0;
				list.Insert(0, 1);
			}
			else
			{
				list[position] = 0;
				addRecursiveList(ref list, position - 1, amount);
			}
		}
	}
#endif
}

[System.Serializable]
public class ButtonTextures
{
	public Texture2D CanBuyTexture;
	public Texture2D CantBuyTexture;
	public Texture2D UnavailableTexture;

	[HideInInspector]
	public Rect Rectangle;

	public ButtonTextures (Rect rectangle)
	{
		Rectangle = rectangle;
	}
}

[System.Serializable]
public class MembershipTextures
{
	public Texture2D[] CanBuyTexture;
	public Texture2D[] CantBuyTexture;
	public Texture2D UnavailableTexture;
	
	[HideInInspector]
	public Rect Rectangle;
	
	public MembershipTextures (Rect rectangle)
	{
		Rectangle = rectangle;
	}
}