using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Apply forward force to instantiated prefab
/// </summary>
public class Launch_Dart_With_Count_Completed : MonoBehaviour
{
    [Tooltip("The projectile that's created")]
    public GameObject projectilePrefab = null;

    [Tooltip("The point that the project is created")]
    public Transform startPoint = null;

    [Tooltip("The speed at which the projectile is launched")]
    public float launchSpeed = 1.0f;

    // Create variable to hold dart count
    [SerializeField] int dartCount = 25;
    // Create varialbe to link and populate TextMeshPro
    [SerializeField] TextMeshProUGUI Text;
    [SerializeField] GameObject nerfGunTrigger;
    Animator m_animator;
    
    // Start is called before the first frame update
    void Start()
    {
        // Finds and adds the first active loaded object that matches the specified type - deprecated (original) code
        // Text = FindObjectOfType<TextMeshProUGUI>();

        // Per https://gamedev.stackexchange.com/questions/132569/how-do-i-find-an-object-by-type-and-name-in-unity-using-c/132575
        // Find GameObject by both name and type
        Text = GameObject.Find("Darts_Remaining_Text (TMP)").GetComponent<TextMeshProUGUI>();

        //Assign the animator at Start
        m_animator = nerfGunTrigger.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Write text and variable value to TextMeshPro, updated once per frame
        Text.text = "Darts Remaining: " + dartCount;
    }

    public void Fire()
    {
        //Trigger nerfGunTrigger pull and release animation
        m_animator.SetTrigger("Fire");

        // Check to see if dartCount is greater than 0
        if (dartCount > 0)
        {
            // Instantiate projectile prefab with start position and start rotation
            GameObject newObject = Instantiate(projectilePrefab, startPoint.position, startPoint.rotation);

            if (newObject.TryGetComponent(out Rigidbody rigidBody))
            {
                // Call ApplyForce method
                ApplyForce(rigidBody);
            }
        }
    }

    private void ApplyForce(Rigidbody rigidBody)
    {
        // Apply force and speed to projectile
        Vector3 force = startPoint.forward * launchSpeed;
        rigidBody.AddForce(force);

        // Reduce dartCount by one
        dartCount -= 1;
    }

    public void Reload()
    {
        // When called set dartCount back to starting value of 25.  Set Reload as your Select Exited and/or Select events.
        dartCount = 25;
    }
}