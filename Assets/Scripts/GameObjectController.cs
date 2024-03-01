#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class GameObjectController : MonoBehaviour
{
    void Start()
    {
        // Disable top-level game objects
        DisableTopLevelObjects();

        // Disable third-level game objects
        DisableThirdLevelObjects();

        // Enable specific game object
        EnableSpecificObject();
    }

#if UNITY_EDITOR
    [MenuItem("Custom/Test GameObject Controller")]
    static void TestGameObjectController()
    {
        GameObjectController controller = FindObjectOfType<GameObjectController>();
        if (controller != null)
        {
            controller.Start();
            // Debug.Log("Testing GameObject Controller");
        }
        else
        {
            // Debug.LogError("GameObject Controller not found in the scene.");
        }
    }
#endif

    void DisableTopLevelObjects()
    {
        DisableObjectsByName("-- DYNAMIC --");
        DisableObjectsByName("-- INTERFACES-- ");
        DisableObjectsByName("-- LIGHTS --");
        DisableObjectsByName("-- STATIC INTERACTIVE --");
        DisableObjectsByName("-- XR IN ROOM --");
        DisableObjectsByName("Furniture");
        DisableObjectsByName("Speakers");
    }

    void DisableThirdLevelObjects()
    {
        // Debug.Log("DisableThirdLevelObjects method called.");
        string[] hierarchy = { "-- STATIC --", "Environment", "Inside" };
        GameObject staticObject = FindObjectByName("-- STATIC --");

        if (staticObject != null)
        {
            DisableObjectsInChildren(staticObject, hierarchy);
        }
        else
        {
            // Debug.LogWarning("Parent object (-- STATIC --) not found.");
        }
    }

    void EnableSpecificObject()
    {
        GameObject drivingSimulatorObject = FindObjectByName("-- DRIVING SIMULATOR --");
        if (drivingSimulatorObject != null)
        {
            string[] hierarchy = { "Cartoon-SportCar_Unity_setup Variant", "Cartoon_SportCar_B01", "-- XR IN CAR --" };
            EnableObjectsInChildren(drivingSimulatorObject, hierarchy);
        }
        else
        {
            // Debug.LogWarning("Parent object (-- DRIVING SIMULATOR --) not found.");
        }
    }

    void DisableObjectsByName(string name)
    {
        // Trim leading and trailing spaces
        name = name.Trim();

        GameObject objectToDisable = GameObject.Find(name);
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
            // Debug.Log($"Disabled object: {name}");
        }
        else
        {
            // Debug.Log($"Object not found: {name}");
        }
    }

    void EnableObjectsInChildren(GameObject parent, params string[] hierarchy)
    {
        Transform currentTransform = parent.transform;

        // Traverse the hierarchy
        for (int i = 0; i < hierarchy.Length; i++)
        {
            currentTransform = currentTransform.Find(hierarchy[i]);

            // Debug print for each level
            // Debug.Log($"Level {i + 1} - {hierarchy[i]}: {currentTransform?.gameObject}");

            // If any level is not found, return null
            if (currentTransform == null)
            {
                // Debug.LogWarning($"Child object not found at level {i + 1} - {hierarchy[i]} in the specified hierarchy.");
                return;
            }
        }

        GameObject childObject = currentTransform.gameObject;

        if (childObject != null && !childObject.activeSelf)
        {
            childObject.SetActive(true);
            // Debug.Log("Enabled child object: " + childObject.name);
        }
        else if (childObject == null)
        {
            // Debug.LogWarning("Child object not found in the specified hierarchy.");
        }
        else
        {
            // Debug.Log("Child object is already active: " + childObject.name);
        }
    }

    GameObject FindObjectByName(string name)
    {
        return GameObject.Find(name);
    }

    GameObject FindObjectInChildren(GameObject parent, params string[] hierarchy)
    {
        Transform currentTransform = parent.transform;

        // Check if the first level in the hierarchy matches the direct child
        if (currentTransform.name == hierarchy[0])
        {
            // Debug print for the first level
            // Debug.Log($"Level 1 - {hierarchy[0]}: {currentTransform.gameObject}");
        }
        else
        {
            // Debug print if the first level doesn't match
            // Debug.LogWarning($"Child object not found at level 1 - {hierarchy[0]} in the specified hierarchy. Parent: {parent.name}, Hierarchy: {string.Join(" -> ", hierarchy)}");
            return null;
        }

        // Traverse the remaining hierarchy levels
        for (int i = 1; i < hierarchy.Length; i++)
        {
            currentTransform = currentTransform.Find(hierarchy[i]);

            // Debug print for each level
            // Debug.Log($"Level {i + 1} - {hierarchy[i]}: {currentTransform?.gameObject}");

            // If any level is not found, return null
            if (currentTransform == null)
            {
                // Debug.LogWarning($"Child object not found at level {i + 1} - {hierarchy[i]} in the specified hierarchy. Parent: {parent.name}, Hierarchy: {string.Join(" -> ", hierarchy)}");
                return null;
            }
        }

        return currentTransform.gameObject;
    }

    void DisableObjectsInChildren(GameObject parent, params string[] hierarchy)
    {
        // Debug.Log($"Trying to disable objects in children of {parent.name} with hierarchy: {string.Join(" -> ", hierarchy)}");

        GameObject childObject = FindObjectInChildren(parent, hierarchy);

        if (childObject != null)
        {
            // Debug.Log($"Found child object to disable: {childObject.name}");
            DisableObjectRecursively(childObject);
        }
        else
        {
            // Debug.LogWarning($"Child object not found or already disabled. Parent: {parent.name}, Hierarchy: {string.Join(" -> ", hierarchy)}");
        }
    }

    void DisableObjectRecursively(GameObject obj)
    {
        obj.SetActive(false);

        foreach (Transform child in obj.transform)
        {
            DisableObjectRecursively(child.gameObject);
        }
    }
}

public static class TransformExtensions
{
    public static string GetFullPath(this Transform transform)
    {
        var path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}