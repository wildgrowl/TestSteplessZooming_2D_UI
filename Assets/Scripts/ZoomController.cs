using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomController : MonoBehaviour
{
    public GameObject m_mapContainer;
    public int m_lodLevels;
    public float m_zoomMin;
    public float m_zoomMax;

    private float m_scale;

    #region constant variable
    private const float SCALE_FACTOR = 0.2f;
    private Vector2 m_old_position1;
    private Vector2 m_old_position2;
    private bool m_is_double_finger = false;
    #endregion

    void Update()
    {
        float delta = processZoom();
        m_scale = Mathf.Clamp(m_scale + delta, m_zoomMin, m_zoomMax);

        var rectTrans = m_mapContainer.GetComponent<RectTransform>();
        if (rectTrans != null)
        {
            rectTrans.localScale = new Vector3(m_scale, m_scale, 1.0f);
        }

        int curLodLevel = m_lodLevels -  (int)(m_scale / (m_zoomMax - m_zoomMin) * m_lodLevels) - 1;
        m_mapContainer.BroadcastMessage ("OnZoom", curLodLevel);
    }

    float processZoom()
    {
        float delta = 0;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
        delta = Input.GetAxis("Mouse ScrollWheel") * SCALE_FACTOR;
#elif UNITY_ANDROID || UNITY_IPHONE || UNITY_IPAD
        if (Input.touchCount == 2)
        {
            // 当从单指或两指以上触摸进入两指触摸的时候,记录一下触摸的位置
            // 保证计算缩放都是从两指手指触碰开始的
            if (!m_is_double_finger)
            {
                m_old_position1 = Input.GetTouch(0).position;
                m_old_position2 = Input.GetTouch(1).position;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                // 计算出当前两点触摸点的位置
                var tempPosition1 = Input.GetTouch(0).position;
                var tempPosition2 = Input.GetTouch(1).position;

                float currentTouchDistance = Vector2.Distance(tempPosition1, tempPosition2);
                float lastTouchDistance = Vector2.Distance(m_old_position1, m_old_position2);

                //备份上一次触摸点的位置，用于对比
                m_old_position1 = tempPosition1;
                m_old_position2 = tempPosition2;

                delta = (lastTouchDistance - currentTouchDistance) * 0.25f * Time.deltaTime;
            }

            m_is_double_finger = true;
        }
        else
            m_is_double_finger = false;
#endif
        return delta;
    }
}
