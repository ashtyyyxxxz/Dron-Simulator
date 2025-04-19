using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject defaultMenu;
    [SerializeField] private GameObject selectMapMenu;

    public void SelectDefaultMenu()
    {
        defaultMenu.SetActive(true);
        selectMapMenu.SetActive(false);
    }

    public void SelectMapMenu()
    {
        defaultMenu.SetActive(false);
        selectMapMenu.SetActive(true);
    }

    public void LoadScene(int number)
    {
        SceneManager.LoadScene(number);
    }
}
