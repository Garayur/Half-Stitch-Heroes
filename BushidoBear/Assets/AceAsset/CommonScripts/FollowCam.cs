//------------------------------------------------------------
//
//	Example Code - by AceAsset
//
//  email : AceAsset@gmail.com
//
//------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour 
{
	public GameObject	m_target = null;
	public float		m_smooth = 1.0f;
	public float		m_eyeSmooth = 1.0f;
	public float		m_targetDist = 4.0f;
	public float		m_minusDist = -2.0f;
	public float		m_plusDist = 2.0f;

	Vector3 m_lastTargetPosition = Vector3.zero;

	Vector3		m_defaultPosition = Vector3.zero;
	Quaternion	m_defaultRotation = Quaternion.identity;


	public void Follow(bool on)
	{
		if( on == false )
		{
			transform.position = m_defaultPosition;
			transform.rotation = m_defaultRotation;
			enabled = false;
		}
		else
		{
			enabled = true;
		}
	}

	public void SetTarget(GameObject obj)
	{
		m_target = obj;
	}

	void OnChangeCharControl(CharControl character)
	{
		if( character == null )
			return;

		if( character.m_cameraTarget != null )
		{
			SetTarget(character.m_cameraTarget);
		}
		else
		{
			SetTarget(character.gameObject);
		}
	}


	// Use this for initialization
	void Start () 
	{
		m_defaultPosition = transform.position;
		m_defaultRotation = transform.rotation;

		if( m_target == null )
		{
			m_lastTargetPosition = transform.position + transform.forward;
			return;
		}

		m_lastTargetPosition = m_target.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( m_target == null )
			return;

		UpdateDistance();
		UpdateMouseRotation();

		m_lastTargetPosition = Vector3.Lerp(m_lastTargetPosition, m_target.transform.position, m_smooth * Time.deltaTime);
		transform.LookAt(m_lastTargetPosition);
	}

	private void UpdateMouseRotation()
	{
		if(Input.GetMouseButton(0) && GUIUtility.hotControl == 0)
		{
			float dx = Input.GetAxis("Mouse X");
			float dy = Input.GetAxis("Mouse Y");


			if( dx != 0.0f )
			{
				Vector3 pos = transform.position;
				pos += dx * -GetComponent<Camera>().transform.right * m_smooth * Time.deltaTime * 10.0f;
				GetComponent<Camera>().transform.position = pos;
			}

			if( dy != 0.0f )
			{
				Vector3 pos = transform.position;
				pos += dy * -GetComponent<Camera>().transform.up * m_smooth * Time.deltaTime * 10.0f;
				GetComponent<Camera>().transform.position = pos;
			}
		}


	}


	private void UpdateDistance()
	{
		Vector3 v1 = m_lastTargetPosition;
		Vector3 v2 = transform.position;

		v1.y = 0.0f;
		v2.y = 0.0f;

		Vector3 dir = (v2 - v1).normalized;
	
		float distance = Vector3.Distance(v1, v2);

		Vector3 eye = m_lastTargetPosition;
		eye.y = transform.position.y;

		if( distance < m_targetDist - m_minusDist )
		{
			v1 += dir * (m_targetDist - m_minusDist);
			v1.y = transform.position.y;

			transform.position = Vector3.Lerp(transform.position, v1, m_eyeSmooth * Time.deltaTime); 
			return;
		}


		if( distance > m_targetDist + m_plusDist )
		{
			v1 += dir * (m_targetDist + m_plusDist);
			v1.y = transform.position.y;

			transform.position = Vector3.Lerp(transform.position, v1, m_eyeSmooth * Time.deltaTime);
			return;
		}
	}
}
