using StdfLab.Core;
using System.ComponentModel;

namespace SiteCompare;

public class PickData : INotifyPropertyChanged
{
    private bool _isSelected = false;
    private List<int> _sitesList = [];

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }

    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public IFileDataInfo? FileData { get; set; }

    public int? SiteNum
    {
        get => field;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SiteNum)));
        }
    }
    public List<int> SitesList
    {
        get => _sitesList;
        set
        {
            _sitesList = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SitesList)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
