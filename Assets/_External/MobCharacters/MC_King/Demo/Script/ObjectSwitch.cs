using UnityEngine;
using System.Collections;

namespace MCA_ObjSwitch
{

	public class ObjectSwitch : MonoBehaviour {

		GameObject obj;
		int onoff;

		// Use this for initialization
		void Start () {

			obj = this.gameObject;
		
		}

		public void Switch(){

			if(onoff == 0){			
				obj.SetActive (false);
				onoff = 1;
				return;
			}else{
				obj.SetActive (true);
				onoff = 0;
				return;
			}

		}

	}
}
