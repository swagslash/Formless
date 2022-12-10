using UnityEngine;
using UnityEngine.InputSystem;

public class VictoryScreen : MonoBehaviour
{
    private static Color GREEN = new Color(0 / 255f, 197 / 255f, 21 / 255f);
    private static Color RED = new Color(255 / 255f, 66 / 255f, 0 / 255f);
    private static Color NEUTRAL = Color.white;


    public Item LeftItem;
    public Item RightItem;
    public GameManager gameManager;

#region selection
    public Item SelectedItem;
    public int SelectedItemIndex = 1;

    public GameObject LeftIndicator;
    public GameObject MiddleIndicator;
    public GameObject RightIndicator;

#endregion

#region left and right item
    public TMPro.TextMeshProUGUI LeftGoodValue;
    public TMPro.TextMeshProUGUI LeftBadValue;
    public UnityEngine.UI.Image LeftGoodImage; 
    public UnityEngine.UI.Image LeftBadImage; 


    public TMPro.TextMeshProUGUI RightGoodValue;
    public TMPro.TextMeshProUGUI RightBadValue;
    public UnityEngine.UI.Image RightGoodImage; 
    public UnityEngine.UI.Image RightBadImage; 
#endregion

#region current stats
    public TMPro.TextMeshProUGUI HealthText;
    public TMPro.TextMeshProUGUI MovementSpeedText;
    public TMPro.TextMeshProUGUI DamageText;
    public TMPro.TextMeshProUGUI TimeToReloadText;
    public TMPro.TextMeshProUGUI RateOfFireText;
    public TMPro.TextMeshProUGUI MagazineSizeText;
#endregion

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        UpdateIndicators();

        // Debug code
        SetItems(LeftItem, RightItem);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnSelectLeft(new InputValue());
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            OnSelectRight(new InputValue());
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSelectItem(new InputValue());
        }
    }

    void OnSelectItem(InputValue value) {
        if (SelectedItemIndex == 0) {
            gameManager.SelectItem(LeftItem);
        } else if (SelectedItemIndex == 2) {
            gameManager.SelectItem(RightItem);
        }

        gameManager.StartNextLevel();
    }

    void OnSelectLeft(InputValue value) {
        SelectedItemIndex = Mathf.Max(0, SelectedItemIndex - 1);
        UpdateIndicators();
    }

    void OnSelectRight(InputValue value) {
        SelectedItemIndex = Mathf.Min(2, SelectedItemIndex + 1);
        UpdateIndicators();
    }

    void UpdateIndicators() {
        LeftIndicator.SetActive(SelectedItemIndex == 0);
        MiddleIndicator.SetActive(SelectedItemIndex == 1);
        RightIndicator.SetActive(SelectedItemIndex == 2);

        if (SelectedItemIndex == 0) {
            SelectedItem = LeftItem;
        } else if (SelectedItemIndex == 2) {
            SelectedItem = RightItem;
        } else {
            SelectedItem = null;
        }

        var health = gameManager.Player.MaxHealthModifier + (SelectedItem?.MaxHealthModifier ?? 0);
        SetModifier(HealthText, health, false, false);

        var movementSpeed = gameManager.Player.MovementSpeedModifier + (SelectedItem?.MovementSpeedModifier ?? 0);
        SetModifier(MovementSpeedText, movementSpeed, true, false);

        var damage = gameManager.Player.DamagePerBulletModifier + (SelectedItem?.DamagePerBulletModifier ?? 0);
        SetModifier(DamageText, damage, true, false);

        var timeToReload = gameManager.Player.TimeToReloadModifier + (SelectedItem?.TimeToReloadModifier ?? 0);
        SetModifier(TimeToReloadText, timeToReload, true, true);

        var rateOfFire = gameManager.Player.RateOfFireModifier + (SelectedItem?.RateOfFireModifier ?? 0);
        SetModifier(RateOfFireText, rateOfFire, true, false);

        var magazineSize = gameManager.Player.MagazineSizeModifier + (SelectedItem?.MagazineSizeModifier ?? 0);
        SetModifier(MagazineSizeText, magazineSize, false, false);
    }

    void SetModifier(TMPro.TextMeshProUGUI textElement, float value, bool isRelative, bool isPositiveBad) {
        var text = "" + (value < 0 ? "-" : "+");
        text += (int) (isRelative ? Mathf.Abs(value) * 100 : Mathf.Abs(value));
        text += isRelative ? "%" : "";

        textElement.text = text;

        if (value == 0)
        {
            textElement.color = NEUTRAL;
        }
        else if (isPositiveBad) {
            if (value > 0) {
                textElement.color = RED;
            } else {
                textElement.color = GREEN;
            }
        } else {
            if (value > 0) {
                textElement.color = GREEN;
            } else {
                textElement.color = RED;
            }
        }
    }

    public void SetItems(Item leftItem, Item rightItem) {
        this.LeftItem = leftItem;
        this.RightItem = rightItem;

        LeftGoodImage.sprite = leftItem.GoodIcon;
        LeftGoodValue.text = leftItem.GoodValue + "";

        LeftBadImage.sprite = leftItem.BadIcon;
        LeftBadValue.text = leftItem.BadValue + "";

        RightGoodImage.sprite = rightItem.GoodIcon;
        RightGoodValue.text = rightItem.GoodValue + "";

        RightBadImage.sprite = rightItem.BadIcon;
        RightBadValue.text = rightItem.BadValue + "";
    }
}
