using UnityEngine;



public class EquipmentCrafter : MonoBehaviour
{
    [SerializeField] private ItemInventory itemInventory = null;
    [SerializeField] private EquipmentInventory equipmentInventory = null;

    [SerializeField] private EquipmentRecipe recipe = null;
    [SerializeField] private int level = 1;
    [SerializeField] private EquipmentRarity rarity = EquipmentRarity.Common;
    
    public void SetRecipe(EquipmentRecipe newRecipe)
    {
        recipe = newRecipe;
    }
    public void SetLevel(int newLevel)
    {
        level = newLevel;
    }
    public void SetRarity(EquipmentRarity newRarity)
    {
        rarity = newRarity;
    }
    public void ClearCraftingData()
    {
        recipe = null;
        level = 1;
        rarity = EquipmentRarity.Common;
    }
    public void CraftEquipment()
    {
        if (recipe == null)
        {
            // popup UI to notify player to select a recipe
            Debug.LogError("No recipe assigned for crafting.");
            return;
        }
        // check if player has required materials
        if (!HasRequiredMaterials())
        {
            // popup UI to notify player of missing materials
            Debug.LogError("Not enough materials to craft the equipment.");
            return;
        }
        Equipment newEquipment = EquipmentFactory.GetCraftedEquipment(recipe, level, rarity);
        if (newEquipment != null)
        {
            equipmentInventory.AddEquipment(newEquipment);
            // popup UI to notify player of successful crafting
        }
        else
        {
            Debug.LogError("Failed to craft equipment. Check the recipe and materials.");
        }
    }

    public bool HasRequiredMaterials()
    {
        if (recipe == null)
            return false;
        foreach (var itemSlot in recipe.ItemSlots)
        {
            if (!itemInventory.HasItem(itemSlot.Item, itemSlot.Quantity))
                return false;
        }
        foreach (var baseMaterial in recipe.BaseMaterials)
        {
            // base material cost implementation
        }
        return true;
    }
    void Start()
    {
        CheckDependencies();
    }
    private void CheckDependencies()
    {
        if (itemInventory == null)
        {
            Debug.LogError("ItemInventory not assigned. Please assign the ItemInventory asset in the Inspector.");
        }
        if (equipmentInventory == null)
        {
            Debug.LogError("EquipmentInventory not assigned. Please assign the EquipmentInventory asset in the Inspector.");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
