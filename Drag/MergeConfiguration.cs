using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configurations/Create MergeConfiguration", fileName = "MergeConfiguration", order = 0)]
public class MergeConfiguration : ScriptableObject
{
    [SerializeField] private List<ItemConfiguration> mergeConfigurations = new List<ItemConfiguration>() {null, null};
    [SerializeField] private ItemConfiguration resultConfiguration;

    public List<ItemConfiguration> MergeConfigurations => mergeConfigurations;

    public ItemConfiguration ResultConfiguration => resultConfiguration;
}