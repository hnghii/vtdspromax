using UnityEngine;

public class CreditController : MonoBehaviour
{
    public GameObject creditImage;
    public GameObject prefabToHide; 

    void Start()
    {
        
        creditImage.SetActive(false);
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Ẩn hình ảnh
            creditImage.SetActive(false);
            
            prefabToHide.SetActive(true);
        }
    }

    public void ToggleCreditVisibility()
    {
        
        creditImage.SetActive(!creditImage.activeSelf);

        
        if (creditImage.activeSelf)
        {
            prefabToHide.SetActive(false);
        }
        else
        {
            prefabToHide.SetActive(true);
        }
    }
}
