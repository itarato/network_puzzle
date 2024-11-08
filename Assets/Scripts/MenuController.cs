using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour {
    public TMP_Dropdown levelsDropdown;
    public GameController gameController;
    public GameObject menuUI;
    public TextMeshProUGUI titleText;

    private int selectedIndex = 1;

    List<Coord> levels = new List<Coord>{
        new Coord(3, 3),
        new Coord(3, 5),
        new Coord(4, 4),
        new Coord(6, 8),
        new Coord(9, 12),
        new Coord(10, 18),
    };

    // Start is called before the first frame update
    void Start() {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (Coord coord in levels) {
            string name = coord.x + " x " + coord.y;
            options.Add(new TMP_Dropdown.OptionData(name));
        }

        levelsDropdown.AddOptions(options);
        UpdateTitle();
    }

    public void OnClickStartButton() {
        Coord selectedCoord = levels[selectedIndex];

        gameController.Reload(selectedCoord);

        menuUI.SetActive(false);
    }

    public void OnChangeLevelsDropdown(int selectedIndex) {
        this.selectedIndex = selectedIndex;
        UpdateTitle();
    }

    private void UpdateTitle() {
        Coord coord = levels[selectedIndex];
        string name = "Map: " + coord.x + " x " + coord.y;
        titleText.text = name;
    }
}
