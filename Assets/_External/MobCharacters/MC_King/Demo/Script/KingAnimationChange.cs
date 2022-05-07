using UnityEngine;
using System.Collections;

namespace MCA_KingAnimationChange
{
	public class KingAnimationChange : MonoBehaviour {

		Animator kingAnim;

		int id;
		int wa;
		int ru;
		int si;

		void Start () {
			kingAnim = GetComponent<Animator> ();
			id = 1; wa = 0; ru = 0; si = 0;
		}
		
		void Update () {
		
		}

		public void Idle () {
			if(id == 0){
				kingAnim.SetTrigger("idle");
				id = 1; wa = 0; ru = 0; si = 0;
			}
		}
		
		public void Walk () {
			if(wa == 0){
				kingAnim.SetTrigger("walk");
				id = 0; wa = 1; ru = 0; si = 0;
			}
		}
		
		public void Run () {
			if(ru == 0){
				kingAnim.SetTrigger("run");
				id = 0; wa = 0; ru = 1; si = 0;
			}
		}
		
		public void Sit () {
			if(si == 0){
				kingAnim.SetTrigger("sit");
				id = 0; wa = 0; ru = 0; si = 1;
			}
		}

	}
}
