using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements; 
public class UIControllerTest : MonoBehaviour
{
    public Button biteButton;
    public Label label;
    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        biteButton = root.Q<Button>("bitebutton");
        label = root.Q<Label>("textbite");

        biteButton.clicked += bitebutton;

    }

    public void bitebutton()
    {
        label.text = "cum";
        label.style.display = DisplayStyle.Flex;

    }
}
