using UnityEngine;
using UnityEngine.UI;

public class RecipeButton : MonoBehaviour
{
    public string recipeName;
    public Button button;

    public void Setup(string name, System.Action<string> onClick)
    {
        recipeName = name;
            button.onClick.AddListener(() => onClick(recipeName));
    }
}