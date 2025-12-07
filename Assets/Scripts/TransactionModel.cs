using System;

[Serializable]
public class TransactionModel
{
    public string type;        // Income or Expense
    public string date;        // MM/DD/YYYY
    public string category;    // Sales, Food, etc
    public float amount;       // Always positive
    public string description; // Text box
    public bool taxDeductible; // Checkbox
}
