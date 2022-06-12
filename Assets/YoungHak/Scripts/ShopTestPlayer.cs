using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.YongHak;

namespace UnityChanAdventure.YongHak
{
    public class ShopTestPlayer : MonoBehaviour
    {
        private float speed = 10.0f;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Move();
        }

        void Move()
        {
            if(Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            if(Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime);
            }
            if(Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * speed * Time.deltaTime);
            }
            if(Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime);
            }
        }

        /*private void onTriggerEnter(Collider other)
        {
            Debug.Log("체크1");
            if(other.tag == "Shop")
            {
                Debug.Log("체크2");
                Shop shop = other.GetComponent<Shop>();
                shop.Enter(this);
            }
        }

        private void onTriggerExit(Collider other)
        {
            if(other.tag == "Shop")
            {
                Shop shop = other.GetComponent<Shop>();
                shop.Exit();
            }
        }*/
    }
}
