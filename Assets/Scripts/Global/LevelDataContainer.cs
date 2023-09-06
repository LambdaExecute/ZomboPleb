using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataContainer", menuName = "Create new level data container.")]
public class LevelDataContainer : ScriptableObject
{
    [System.Serializable]
    public struct LevelData
    {
        [Min(10)] public float zombieFieldDepth;
        [Min(0.1f)] public float zombieDensity;
        [Min(100)] public int playerMass;
        [Min(0)] public int countOfSoldiers;
        [Min(15)] public int distanceToZombies;
        [Min(10)] public int distanceFromZombiesToTarget;
    }

    public List<LevelData> levels;
}
