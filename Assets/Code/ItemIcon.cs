using UnityEngine;
using UnityEngine.UI;

public class ItemIcon: MonoBehaviour
{
    Image Icon;
    public void SetActive(bool active)
    {
        Icon.fillCenter = true; 
    }

    private void Awake()
    {
        Icon = GetComponent<Image>();
    }
}
