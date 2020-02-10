using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILodHelper : MonoBehaviour
{
    private Image m_image;
    private string m_lod0ResName;
    private int m_curLodLevel = 0;

    private Camera m_UICamera;

    void Start()
    {
        m_image = GetComponent<Image>();
        m_lod0ResName = m_image.sprite.name;

        m_UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
    }

    public void OnZoom(int curLodLevel)
    {
        if (curLodLevel != m_curLodLevel)
        {
            string curLevelResName;
            if (curLodLevel > 0)
                curLevelResName = m_lod0ResName + "-lod" + curLodLevel.ToString();
            else
                curLevelResName = m_lod0ResName;

            m_image.sprite = Resources.Load<Sprite>("Textures/" + curLevelResName);

            m_curLodLevel = curLodLevel;

            bool isVisible = IsVisible(gameObject);
            if (!isVisible)
                m_image.sprite = null;

            string log = string.Format("==== {0}  {1}", m_lod0ResName, isVisible);
            Debug.Log(log);
        }
    }

    bool IsVisible(GameObject Object)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(m_UICamera);

        if (GeometryUtility.TestPlanesAABB(planes, Object.GetComponent<Collider>().bounds))
            return true;
        else
            return false;
    }
}
