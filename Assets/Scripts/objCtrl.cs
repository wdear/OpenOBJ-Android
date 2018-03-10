using UnityEngine;
using System.Collections;
using System.IO;

public class objCtrl : MonoBehaviour
{
    Touch preTouch0;  
    Touch preTouch1;    

    void Start()
    {

    }

    void Update()
    {

        if (Input.touchCount <= 0)
        {
            return;
        }

        //单点触摸控制旋转  
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 deltaPos = touch.deltaPosition;
            transform.Rotate(Vector3.down * deltaPos.x, Space.Self);
            transform.Rotate(Vector3.right * deltaPos.y, Space.Self);
        }

        //多点触摸控制缩放  
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        if (touch1.phase == TouchPhase.Began)
        {
            preTouch1 = touch1;
            preTouch0 = touch0;
            return;
        }

        float preDistance = Vector2.Distance(preTouch0.position, preTouch1.position);
        float distance = Vector2.Distance(touch0.position, touch1.position);

        //两次touch的距离差来计算放大或缩小  
        float offset = distance - preDistance;

        float scaleFactor = offset / 100f;
        Vector3 localScale = transform.localScale;
        Vector3 scale = new Vector3(localScale.x + scaleFactor,
                                    localScale.y + scaleFactor,
                                    localScale.z + scaleFactor);

        //最小缩放  
        if (scale.x > 0.1f && scale.y > 0.1f && scale.z > 0.1f)
        {
            transform.localScale = scale;
        }

        preTouch0 = touch0;
        preTouch1 = touch1;
    }
}