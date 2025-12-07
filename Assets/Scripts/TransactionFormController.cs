using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TransactionFormController : MonoBehaviour
{
    [Header("Modal")]
    public GameObject modalOverlay;
    public GameObject transactionFormPanel;

    [Header("Input Fields")]
    public TMP_Dropdown typeDropdown;
    public TMP_InputField dateField;
    public TMP_Dropdown categoryDropdown;
    public TMP_InputField amountField;
    public TMP_InputField descriptionField;
    public Toggle taxToggle;

    [Header("Transaction Page")]
    public Transform transactionsContent;
    public GameObject transactionItemPrefab;

    // Tracks which item is being edited
    private GameObject itemBeingEdited = null;

    // ----------------------------------------------------------
    public void OpenForm()
    {
        modalOverlay.SetActive(true);
        transactionFormPanel.SetActive(true);
    }

    public void CloseForm()
    {
        modalOverlay.SetActive(false);
        transactionFormPanel.SetActive(false);
        itemBeingEdited = null; // reset edit mode
    }
    // ----------------------------------------------------------

    public void AddTransaction()
    {
        // VALIDATION ----------------------------------------------------
        if (string.IsNullOrWhiteSpace(amountField.text))
        {
            Debug.Log("Amount is required");
            return;
        }

        if (!float.TryParse(amountField.text, out float amount))
        {
            Debug.Log("Invalid amount");
            return;
        }

        // Build data
        string typeText = typeDropdown.options[typeDropdown.value].text;
        string categoryText = categoryDropdown.options[categoryDropdown.value].text;

        // ===============================================================
        // EDIT MODE — UPDATE EXISTING ITEM
        // ===============================================================
        if (itemBeingEdited != null)
        {
            UpdateExistingItem(itemBeingEdited, typeText, categoryText, amount);
            CloseForm();
            return;
        }

        // ===============================================================
        // ADD MODE — CREATE NEW ITEM
        // ===============================================================
        var item = Instantiate(transactionItemPrefab, transactionsContent);

        // Fill the UI text fields
        item.transform.Find("LeftBlock/TextBlock/Title")
            .GetComponent<TMP_Text>().text = descriptionField.text;

        item.transform.Find("LeftBlock/TextBlock/Description")
            .GetComponent<TMP_Text>().text = $"{typeText} • {categoryText}";

        item.transform.Find("RightBlock/AmountContainer/Amount")
            .GetComponent<TMP_Text>().text =
            (typeText == "Income" ? "+" : "-") + amount.ToString("N2");

        item.transform.Find("LeftBlock/StatusDot")
            .GetComponent<Image>().color =
            (typeText == "Income" ? Color.green : Color.red);

        // DELETE BUTTON HOOKUP ------------------------------------------
        var deleteButton = item.transform.Find("RightBlock/Buttons/DeleteButton")
            .GetComponent<Button>();

        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(() => DeleteItem(item));

        // EDIT BUTTON HOOKUP --------------------------------------------
        var editButton = item.transform.Find("RightBlock/Buttons/EditButton")
            .GetComponent<Button>();

        editButton.onClick.RemoveAllListeners();
        editButton.onClick.AddListener(() => EditItem(item));

        // RESET FORM FIELDS --------------------------------------------
        amountField.text = "";
        descriptionField.text = "";

        CloseForm();
    }

    // ----------------------------------------------------------
    public void DeleteItem(GameObject item)
    {
        Destroy(item);
    }

    // ----------------------------------------------------------
    public void EditItem(GameObject item)
    {
        itemBeingEdited = item;

        // Read current UI values from the item
        string fullDesc = item.transform.Find("LeftBlock/TextBlock/Description")
            .GetComponent<TMP_Text>().text;

        // DESCRIPTION (title)
        descriptionField.text = item.transform.Find("LeftBlock/TextBlock/Title")
            .GetComponent<TMP_Text>().text;

        // AMOUNT
        string amountText = item.transform.Find("RightBlock/AmountContainer/Amount")
            .GetComponent<TMP_Text>().text;

        amountField.text = amountText.Replace("+", "").Replace("-", "");

        // TYPE (Income / Expense)
        if (fullDesc.Contains("Income")) typeDropdown.value = 0;
        if (fullDesc.Contains("Expense")) typeDropdown.value = 1;

        // CATEGORY
        string[] parts = fullDesc.Split('•');
        if (parts.Length > 1)
        {
            string cat = parts[1].Trim();
            int i = categoryDropdown.options.FindIndex(o => o.text == cat);
            if (i != -1) categoryDropdown.value = i;
        }

        // DATE (we can store this later when you add date logic)
        // For now just leave dateField as-is

        OpenForm(); // show form with pre-filled values
    }

    // ----------------------------------------------------------
    private void UpdateExistingItem(GameObject item, string typeText, string categoryText, float amount)
    {
        item.transform.Find("LeftBlock/TextBlock/Title")
            .GetComponent<TMP_Text>().text = descriptionField.text;

        item.transform.Find("LeftBlock/TextBlock/Description")
            .GetComponent<TMP_Text>().text = $"{typeText} • {categoryText}";

        item.transform.Find("RightBlock/AmountContainer/Amount")
            .GetComponent<TMP_Text>().text =
            (typeText == "Income" ? "+" : "-") + amount.ToString("N2");

        item.transform.Find("LeftBlock/StatusDot")
            .GetComponent<Image>().color =
            (typeText == "Income" ? Color.green : Color.red);
    }
}
