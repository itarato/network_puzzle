using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    public TMP_Dropdown levelsDropdown;
    public GameController gameController;
    public GameObject menuUI;
    public TextMeshProUGUI titleText;
    public GameObject acrossBordersToggleGameObject;

    private int selectedIndex = 1;
    private bool isAcrossBorders = false;
    private CellType cellType = CellType.Square;

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
            string name = coord.x + " x " + coord.y + " board";
            options.Add(new TMP_Dropdown.OptionData(name));
        }

        levelsDropdown.AddOptions(options);
        UpdateTitle();
    }

    public void OnClickStartButton() {
        Coord selectedCoord = levels[selectedIndex];

        gameController.Reload(selectedCoord, isAcrossBorders, cellType);

        menuUI.SetActive(false);
    }

    public void OnChangeLevelsDropdown(int selectedIndex) {
        this.selectedIndex = selectedIndex;
        UpdateTitle();
    }

    public void OnToggleAcrossBorders(bool newValue) {
        isAcrossBorders = newValue;
    }

    private void UpdateTitle() {
        Coord coord = levels[selectedIndex];
        string name = "Map: " + coord.x + " x " + coord.y;
        titleText.text = name;
    }

    public void OnClickSquareToggle(bool value) {
        cellType = CellType.Square;
        //acrossBordersToggleGameObject.SetActive(true);
    }

    public void OnClickHexToggle(bool value) {
        cellType = CellType.Hex;
        //acrossBordersToggleGameObject.SetActive(false);
    }
}
