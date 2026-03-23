using UnityEngine;

public class AllyListContainer : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject allySelectButtonPrefab;
    public void PopulateAllyList()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        foreach (Ally ally in AllyManager.Instance.GetAllies())
        {
            GameObject buttonObj = Instantiate(allySelectButtonPrefab, content);
            AllySelectButton button = buttonObj.GetComponent<AllySelectButton>();
            if (button != null)
            {
                button.Initialize(ally);
            }
        }
    }
}
