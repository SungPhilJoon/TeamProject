
//////////////////////////////////////////////////////////////////////////////////////////////////////////
//This script was created with reference to "https://gist.github.com/TAK-EMI/d67a13b6f73bed32075d".		//
//The creator of the reference script is Mr.TAK-EMI.													//
//////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

namespace MCA_KingViewerCamera
{

	enum MouseButtonDown
	{
		MBD_LEFT = 0,
		MBD_RIGHT,
		MBD_MIDDLE,
	};
	
	public class ViewerCamera : MonoBehaviour
	{
		[SerializeField]	
		[HideInInspector]
		private GameObject pivotObj = null;
		
		private Vector3 oldPos;
		
		void setupFocusObject(string name)
		{
			GameObject obj = this.pivotObj = new GameObject(name);
			obj.transform.position = (new Vector3 (0.0f, 1.0f, 0.0f));
			
			return;
		}
		
		void Start()
		{

			if (this.pivotObj == null)
				this.setupFocusObject("CameraFocusObject");

			Transform trans = this.transform;
			transform.parent = this.pivotObj.transform;

			trans.LookAt(this.pivotObj.transform.position);
			
			return;
		}
		
		void Update()
		{
			this.mouseEvent();
			
			return;
		}

		void mouseEvent()
		{

			float delta = Input.GetAxis("Mouse ScrollWheel");

			if (delta != 0.0f)
				this.mouseWheelEvent(delta);

			if (Input.GetMouseButtonDown((int)MouseButtonDown.MBD_LEFT) ||
			    Input.GetMouseButtonDown((int)MouseButtonDown.MBD_MIDDLE) ||
			    Input.GetMouseButtonDown((int)MouseButtonDown.MBD_RIGHT))
				this.oldPos = Input.mousePosition;

			this.mouseDragEvent(Input.mousePosition);
			
			return;
		}

		void mouseWheelEvent(float delta)
		{
			Vector3 focusToPosition = this.transform.position - this.pivotObj.transform.position;
			Vector3 post = focusToPosition * (1.0f - delta);

			if (post.magnitude > 0.01f)
				this.transform.position = this.pivotObj.transform.position + post;
			
			return;
		}
		
		void mouseDragEvent(Vector3 mousePos)
		{
			Vector3 diff = mousePos - oldPos;

			if (diff.magnitude < Vector3.kEpsilon)
				return;
			
			if (Input.GetMouseButton((int)MouseButtonDown.MBD_MIDDLE))
			{
				this.cameraTranslate(-diff / 120.0f);				
			}
			else if (Input.GetMouseButton((int)MouseButtonDown.MBD_RIGHT))
			{
				this.cameraRotate(new Vector3(diff.y, diff.x, 0.0f));
			}
	
			this.oldPos = mousePos;
			
			return;
		}

		void cameraTranslate(Vector3 vec)
		{
			Transform focusTrans = this.pivotObj.transform;
			Transform trans = this.transform;

			focusTrans.Translate((trans.right * vec.x) - (trans.up * -vec.y));
			
			return;
		}

		public void cameraRotate(Vector3 eulerAngle)
		{
			Vector3 focusPos = this.pivotObj.transform.position;
			Transform trans = this.transform;

			Vector3 preUpV, preAngle, prePos;
			preUpV = trans.up;
			preAngle = trans.localEulerAngles;
			prePos = trans.position;
			
			trans.RotateAround(focusPos, Vector3.up, eulerAngle.y);
			trans.RotateAround(focusPos, trans.right, -eulerAngle.x);
			trans.LookAt(focusPos);

			Vector3 up = trans.up;
			if(Vector3.Angle(preUpV, up) > 90.0f)
			{
				trans.localEulerAngles = preAngle;
				trans.position = prePos;
			}			
			return;
		}
	}
}