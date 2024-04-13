namespace Barbu
{
    using System;
    using UnityEngine;

    public static class GameObjectExtensions
    {
        public static GameObject FindGameObjectByName(string name, bool findInactive = false)
        {
            var gameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(findInactive);
            foreach (var gameObject in gameObjects)
            {
                if (gameObject.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return gameObject;
                }
            }

            return null;
        }
    }
}
