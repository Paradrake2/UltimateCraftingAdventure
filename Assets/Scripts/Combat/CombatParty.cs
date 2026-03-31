using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatParty
{
    [SerializeField] private List<Ally> members = new List<Ally>();

    public IReadOnlyList<Ally> Members => members;

    public bool Contains(Ally ally)
    {
        return ally != null && members.Contains(ally);
    }

    public bool TryAdd(Ally ally, IReadOnlyList<Ally> ownedAllies, int maxPartySize, out string failureReason)
    {
        failureReason = null;

        if (ally == null)
        {
            failureReason = "Cannot add null ally to combat party.";
            return false;
        }

        if (!IsOwnedAlly(ally, ownedAllies))
        {
            failureReason = "Cannot add ally to combat party because the ally is not owned.";
            return false;
        }

        if (members.Contains(ally))
        {
            return true;
        }

        if (members.Count >= Mathf.Max(1, maxPartySize))
        {
            failureReason = "Combat party is already at the selection limit.";
            return false;
        }

        members.Add(ally);
        return true;
    }

    public bool Remove(Ally ally)
    {
        if (ally == null)
        {
            return false;
        }

        return members.Remove(ally);
    }

    public bool ToggleSelection(Ally ally, IReadOnlyList<Ally> ownedAllies, int maxPartySize, out bool isSelected, out string failureReason)
    {
        failureReason = null;

        if (Contains(ally))
        {
            Remove(ally);
            isSelected = false;
            return true;
        }

        bool added = TryAdd(ally, ownedAllies, maxPartySize, out failureReason);
        isSelected = added;
        return added;
    }

    public void Clear()
    {
        members.Clear();
    }

    public void Prune(IReadOnlyList<Ally> ownedAllies)
    {
        for (int i = members.Count - 1; i >= 0; i--)
        {
            if (!IsOwnedAlly(members[i], ownedAllies))
            {
                members.RemoveAt(i);
            }
        }
    }

    public Ally[] BuildBattleParty(IReadOnlyList<Ally> ownedAllies, int maxPartySize)
    {
        if (members == null || members.Count == 0)
        {
            return System.Array.Empty<Ally>();
        }

        int limit = Mathf.Max(1, maxPartySize);
        List<Ally> validAllies = new List<Ally>();

        for (int i = 0; i < members.Count && validAllies.Count < limit; i++)
        {
            Ally ally = members[i];
            if (IsOwnedAlly(ally, ownedAllies))
            {
                validAllies.Add(ally);
            }
        }

        return validAllies.ToArray();
    }

    private bool IsOwnedAlly(Ally ally, IReadOnlyList<Ally> ownedAllies)
    {
        if (ally == null || ownedAllies == null)
        {
            return false;
        }

        for (int i = 0; i < ownedAllies.Count; i++)
        {
            if (ownedAllies[i] == ally)
            {
                return true;
            }
        }

        return false;
    }
}