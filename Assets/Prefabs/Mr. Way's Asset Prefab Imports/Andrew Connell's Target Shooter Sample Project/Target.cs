using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform center;
    [SerializeField] private TextMesh output;

    [Header("Settings")]
    [SerializeField] private int pointsPerThreshold = 5;
    [SerializeField] private List<float> thresholdDistances = new List<float>();

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
            CalculateScore(collision.transform.position);
    }

    private void CalculateScore(Vector3 hitPosition)
    {
        float distanceFromCenter = Vector3.Distance(center.position, hitPosition);
        output.text = CalculatePoints(distanceFromCenter).ToString();
    }

    private int CalculatePoints(float distance)
    {
        int totalPoints = 0;

        foreach (float threshold in thresholdDistances)
        {
            if (distance < threshold)
                totalPoints += pointsPerThreshold;
        }

        return totalPoints;
    }
}
