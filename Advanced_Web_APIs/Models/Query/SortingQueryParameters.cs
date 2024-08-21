namespace Advanced_Web_APIs.Models.Query;

public class SortingQueryParameters
{
    public string SortBy { get; set; } = "Id";
    private string _sortorder { get; set; } = "asc";

    public string SortOrder
    {
        get => _sortorder;
        set
        {
            if (value == "asc" || value == "desc")
            {
                _sortorder = value;
            }
        }
    }
}