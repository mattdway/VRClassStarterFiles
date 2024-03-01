// Importing required libraries
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Adding a required component to the game object to which this script is attached
[RequireComponent(typeof(ActionBasedController))]

// Declaring the class "HandController"
public class HandController_Completed : MonoBehaviour
{
    // Declaring a variable "controller" of type "ActionBasedController"
    ActionBasedController controller;

    // Declaring a public variable "hand" of type "Hand_Completed"  "Hand_Completed" is a reference to the script Hand.cs
    // and the public variable hand will be used to access variables and methods from that script.
    public Hand_Completed hand;

    // Start is called before the first frame update
    // This method is called once at the start of the script
    void Start()
    {
        // Getting the "ActionBasedController" component attached to the game object
        controller = GetComponent<ActionBasedController>();
    }

    // Update is called once per frame
    // This method is called every frame
    void Update()
    {
        // Setting the grip value of the "Hand" component to the value of the select action of the controller
        hand.SetGrip(controller.selectAction.action.ReadValue<float>());

        // Setting the trigger value of the "Hand" component to the value of the activate action of the controller
        hand.SetTrigger(controller.activateAction.action.ReadValue<float>());
    }
}