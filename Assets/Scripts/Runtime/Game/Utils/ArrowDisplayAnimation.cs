using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimplePoker.Utils
{
    //[ExecuteInEditMode]
    public class ArrowDisplayAnimation : MonoBehaviour
    {
        //adjust this to change speed
        float speed = 5f;
        //adjust this to change how high it goes
        float height = 0.5f;

        [SerializeField] private RectTransform transform;
 
        void Update() {
            //get the objects current position and put it in a variable so we can access it later with less code
            Vector3 pos = transform.anchoredPosition;
            //calculate what the new Y position will be
            float newY = Mathf.Sin(Time.time * speed) * height + pos.y;
            //set the object's Y to the new calculated Y
            transform.anchoredPosition = new Vector3(pos.x, newY, pos.z);
        }
    }
}