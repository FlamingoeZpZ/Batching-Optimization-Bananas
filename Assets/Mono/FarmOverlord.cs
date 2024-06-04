using System.Collections.Generic;
using UnityEngine;

namespace Mono
{
    public static class FarmOverlord
    {
        public static readonly Queue<GameObject> Objects = new();
        
        public static void AddItem(GameObject go)
        {
            Objects.Enqueue(go);
        }

        public static void RemoveItem(int num)
        {
            while (num++ < 0) Object.Destroy(Objects.Dequeue());
        }

    }
}