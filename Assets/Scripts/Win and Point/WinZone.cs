using UnityEngine;

public class WinZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CatController>() != null)
            GameManager.Instance.Win();
    }
}