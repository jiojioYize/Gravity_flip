using System;
using System.Collections.Generic;
using GravityFlip.Level;
using UnityEngine;

namespace GravityFlip.Core
{
    public sealed class ProgressManager : MonoBehaviour
    {
        public event Action<int, int> ProgressChanged;
        public event Action ProgressCompleted;

        private readonly List<Collectible> collectibles = new List<Collectible>();

        public int CollectedCount { get; private set; }
        public int TotalCount => collectibles.Count;
        public bool IsComplete => TotalCount > 0 && CollectedCount >= TotalCount;

        public void RegisterCollectible(Collectible collectible)
        {
            if (collectible == null || collectibles.Contains(collectible))
            {
                return;
            }

            collectibles.Add(collectible);
            ProgressChanged?.Invoke(CollectedCount, TotalCount);
        }

        public void Collect(Collectible collectible)
        {
            if (collectible == null || !collectibles.Contains(collectible) || collectible.IsCollected)
            {
                return;
            }

            collectible.MarkCollected();
            CollectedCount++;
            ProgressChanged?.Invoke(CollectedCount, TotalCount);

            if (IsComplete)
            {
                ProgressCompleted?.Invoke();
            }
        }

        public void ResetProgress()
        {
            CollectedCount = 0;

            foreach (Collectible collectible in collectibles)
            {
                collectible.ResetCollectible();
            }

            ProgressChanged?.Invoke(CollectedCount, TotalCount);
        }
    }
}
