using UnityEngine;

[System.Serializable]
public class StatValue
{
    [SerializeField] private Stat stat;
    [SerializeField] private float value;
    public Stat Stat => stat;
    public float Value => value;
    public StatValue(Stat stat, float value)
    {
        this.stat = stat;
        this.value = value;
    }
    public void SetValue(float newValue)
    {
        value = newValue;
    }
    public void AddValue(float amount)
    {
        value += amount;
    }

}
