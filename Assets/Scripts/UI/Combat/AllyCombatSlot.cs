using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AllyCombatSlot : MonoBehaviour
{
    [SerializeField] private Ally ally;
    [SerializeField] private Image allyIcon;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject attackBar;
    public Ally Ally => ally;
    public void Initialize(Ally newAlly)
    {
        ally = newAlly;
        if (ally != null)
        {
            ally.CombatStats.Initialize(ally.Stats);
            allyIcon.sprite = ally.Icon;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
