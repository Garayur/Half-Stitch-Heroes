//script written by Michael Withers
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UI;

//----------
//Enums and support classes

//stores a transition type, to be used to either transition in or out
public enum TransitionStyle
{
	//no transition, time denotes delay before cut occurs
	Cut,
	//fades in or out
	Fade,
	//slides in or out in direction specified
	Slide,
	//similar to slide but with a bit of bounce
	Bounce,
	//fade from one edge of the screen to the oppisite
	Wipe
}

//enum for storing a simple direction
public enum Direction
{
	Up,
	Down,
	Left,
	Right
}
//enum for storing the state of this script
public enum TransitionState
{
	Inactive,
	TransitioningOut,
	TransitioningIn,
	Active
}

//stores all information regarding a transition, each script contains two of these, for a transition in, or a transition out
[System.Serializable]
public class TransitionType
{
	public TransitionStyle style = TransitionStyle.Cut;
	public Direction direction = Direction.Up;
	public float time = 0.0f;

	public Vector2 GetDirectionAsVector2()
	{
		if(direction == Direction.Up) return Vector2.up;
		else if(direction == Direction.Down) return Vector2.down;
		else if(direction == Direction.Left) return Vector2.left;
		else return Vector2.right;
	}
}

//Core class for this script
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class MenuTransitioner : NoTimescaleInvokeable 
{
	//---------
	//public
	public TransitionType transitionInType;
	public TransitionType transitionOutType;

	//----------
	//private
	private CanvasGroup canvasGroup = null;
	private RectTransform localRectTransform = null;
	private TransitionState transitionState = TransitionState.Inactive;
	private float transitionProgress = 0.0f;

	//begins the transition in process, this is continued in update, also resets some variables that may have unpredictable values
	public void TransitionIn()
	{
		if(transitionState == TransitionState.Inactive || transitionState == TransitionState.TransitioningOut)
		{
			gameObject.SetActive(true);
			GetCanvasGroup().interactable = true;
			GetCanvasGroup().alpha = 0;
			transitionState = TransitionState.TransitioningIn;
			transitionProgress = 0.0f;
			transform.localPosition = Vector2.zero;
			if(GetComponent<Image>() && GetComponent<Image>().type == Image.Type.Filled)
			{
				GetComponent<Image>().fillAmount = 1.0f;
			}
		}
	}

	//begins the transition out process, this is continued in update
	public void TransitionOut()
	{
		if(transitionState == TransitionState.Active || transitionState == TransitionState.TransitioningIn)
		{
			transitionState = TransitionState.TransitioningOut;
			transitionProgress = 0.0f;
		}
	}

	//ensures a canvasgroup has been assigned, creates one if not
	private CanvasGroup GetCanvasGroup()
	{
		if(canvasGroup == null)
		{
			if(GetComponent<CanvasGroup>() == null)
			{
				canvasGroup = gameObject.AddComponent<CanvasGroup>();
			}
			else
			{
				canvasGroup = GetComponent<CanvasGroup>();
			}
		}
		return canvasGroup;
	}
		
	void Start()
	{
		GetCanvasGroup();
		localRectTransform = GetComponent<RectTransform>();
		if(transitionInType.style == TransitionStyle.Wipe || transitionOutType.style == TransitionStyle.Wipe)
		{
			if(GetComponent<Mask>() == null)
			{
				gameObject.AddComponent<Mask>();
			}
			if(GetComponent<Image>() == null)
			{
				gameObject.AddComponent<Image>();
				GetComponent<Mask>().showMaskGraphic = false;
			}
			if(GetComponent<Image>().type != Image.Type.Filled)
			{
				GetComponent<Image>().type = Image.Type.Filled;
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		//run transition in logic
		if(transitionState == TransitionState.TransitioningIn)
		{
			switch(transitionInType.style)
			{
				case TransitionStyle.Cut:
				{
					if(transitionProgress >= transitionInType.time)
					{
						GetCanvasGroup().alpha = 1;
					}
					break;
				}
				case TransitionStyle.Fade:
				{
					GetCanvasGroup().alpha = transitionProgress / transitionInType.time;
					break;
				}
				case TransitionStyle.Slide:
				{
					transform.localPosition = Vector2.Lerp(
						Vector2.Scale(transitionInType.GetDirectionAsVector2(), new Vector2(localRectTransform.rect.width, localRectTransform.rect.height)) 
						, new Vector2(0,0)
						, transitionProgress / transitionInType.time);
					GetCanvasGroup().alpha = 1;
					break;
				}
				case TransitionStyle.Bounce:
				{
					float bounceDist = 0.95f;
					float bounceStart = 0.8f;
					float transitionProgressOverride = 0.0f;
					if(transitionProgress / transitionInType.time > bounceStart)
					{
						transitionProgressOverride = Mathf.Lerp(1.0f,bounceDist,Mathf.Sin(Mathf.PI * (((transitionProgress / transitionInType.time) - bounceStart)/(1 - bounceStart))));
					}
					else
					{
						transitionProgressOverride = (1.0f/bounceStart)*(transitionProgress / transitionInType.time);
					}
					transform.localPosition = Vector2.Lerp(
						Vector2.Scale(transitionInType.GetDirectionAsVector2(), new Vector2(localRectTransform.rect.width, localRectTransform.rect.height)) 
						, new Vector2(0,0)
						, transitionProgressOverride);
					GetComponent<CanvasGroup>().alpha = 1;
					break;
				}
				case TransitionStyle.Wipe:
				{
					GetComponent<Image>().fillAmount = transitionProgress / transitionInType.time;
					break;
				}
			}

			if(transitionProgress >= transitionInType.time)
			{
				transitionState = TransitionState.Active;
			}
			else
			{
				transitionProgress += Time.unscaledDeltaTime;
			}
		}
		//run transition out logic
		else if(transitionState == TransitionState.TransitioningOut)
		{
			switch(transitionOutType.style)
			{
				case TransitionStyle.Cut:
				{
					//Do nothing
					break;
				}
				case TransitionStyle.Fade:
				{
					GetCanvasGroup().alpha = 1 - (transitionProgress / transitionOutType.time);
					break;
				}
				case TransitionStyle.Slide:
				{
					transform.localPosition = Vector2.Lerp(
						new Vector2(0,0)
						, Vector2.Scale(transitionOutType.GetDirectionAsVector2(), new Vector2(localRectTransform.rect.width, localRectTransform.rect.height)) 
						, transitionProgress / transitionOutType.time);
					break;
				}
				case TransitionStyle.Bounce:
				{
					float bounceDist = 0.95f;
					float bounceStart = 0.8f;
					float transitionProgressOverride = 0.0f;
					if((1-transitionProgress) / transitionOutType.time > bounceStart)
					{
						transitionProgressOverride = Mathf.Lerp(1.0f,bounceDist,Mathf.Sin(Mathf.PI * ((((1-transitionProgress) / transitionOutType.time) - bounceStart)/(1 - bounceStart))));
					}
					else
					{
						transitionProgressOverride = (1.0f/bounceStart)*((1-transitionProgress) / transitionOutType.time);
					}
					transform.localPosition = Vector2.Lerp(
						Vector2.Scale(transitionOutType.GetDirectionAsVector2(), new Vector2(localRectTransform.rect.width, localRectTransform.rect.height)) 
						, new Vector2(0,0)
						, transitionProgressOverride);
					break;
				}
				case TransitionStyle.Wipe:
				{
					GetComponent<Image>().fillAmount = 1 - (transitionProgress / transitionInType.time);
					break;
				}
			}
				
			if(transitionProgress >= transitionOutType.time)
			{
				transitionState = TransitionState.Inactive;
				GetCanvasGroup().interactable = false;
				GetCanvasGroup().alpha = 0;
				gameObject.SetActive(false);
			}
			else
			{
				transitionProgress += Time.unscaledDeltaTime;
			}
		}
	}
}

//Custom Property drawer for TransitionType class, so that variables are only shown in the inspector when they are relevent
[CustomPropertyDrawer(typeof(TransitionType))]
public class TransitionTypePropertyDrawer : PropertyDrawer
{
	private float rows = 2;
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		SerializedProperty style = property.FindPropertyRelative("style");
		float heightAdd = 16;
		rows = 2;
		Rect adjustHeight = position;
		adjustHeight.height = heightAdd;

		EditorGUI.LabelField(position, label.text);
		EditorGUI.indentLevel += 2;

		adjustHeight.y += heightAdd;
		EditorGUI.PropertyField(adjustHeight,style,  new GUIContent{text = "Style"});
		adjustHeight.y += heightAdd;

		switch((TransitionStyle)style.enumValueIndex)
		{
			default:
				EditorGUI.PropertyField(adjustHeight, property.FindPropertyRelative("direction"), new GUIContent{text = "Direction"});
				adjustHeight.y += heightAdd;
				rows++;
				break;
			case TransitionStyle.Cut:
				break;
			case TransitionStyle.Fade:
				break;
			case TransitionStyle.Wipe:
				break;
		}

		EditorGUI.PropertyField(adjustHeight, property.FindPropertyRelative("time"), new GUIContent{text = "Transition Time"});
		EditorGUI.indentLevel -= 1;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight(property, label) + (rows * 16);
	}
}


