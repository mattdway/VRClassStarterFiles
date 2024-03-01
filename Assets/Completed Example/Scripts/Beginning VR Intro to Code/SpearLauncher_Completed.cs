using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SpearLauncher_Completed : MonoBehaviour
{
    // Public variable. Reference to the spear prefab to instantiate
    public GameObject spearPrefab;

    // Public variable. Transform representing the launch point for the spears
    public Transform launchPoint;

    // Public float variable. Force with which the spears will be launched
    public float launchForce = 10f;

    // Public integer variable. Number of spears to instantiate when triggered
    public int numberOfSpearsToInstantiate = 1;

    // Public float variable. Radius within which spears will be randomly distributed
    public float clusterRadius = 1.0f;

    // Private method. Called when a collider enters the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering collider is the player
        if (other.CompareTag("Player"))
        {
            // Trigger the spear launching logic
            LaunchSpears();
        }
    }

    // Private method. Logic to instantiate and launch spears
    private void LaunchSpears()
    {
        for (int i = 0; i < numberOfSpearsToInstantiate; i++)
        {
            // Calculate random offset within the cluster radius
            Vector3 randomOffset = new Vector3(Random.Range(-clusterRadius, clusterRadius),
                                               Random.Range(-clusterRadius, clusterRadius),
                                               0f); // Set Z to 0 or a positive value

            // Calculate the random launch position
            Vector3 randomLaunchPosition = launchPoint.position + randomOffset;

            // Instantiate the spear at the random launch position
            GameObject spear = Instantiate(spearPrefab, randomLaunchPosition, launchPoint.rotation);

            // Get the Rigidbody component of the spear
            Rigidbody spearRigidbody = spear.GetComponent<Rigidbody>();

            // Apply force to launch the spear in the forward direction
            spearRigidbody.AddForce(launchPoint.forward * launchForce, ForceMode.Impulse);
        }
    }
}