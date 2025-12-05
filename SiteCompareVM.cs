using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StdfLab.Core;
using System.Collections.ObjectModel;
using System.Data;

namespace SiteCompare;

public partial class SiteCompareVM : ObservableObject
{
    private readonly IDataFactory _dataFactory;
    private readonly IDialogService _dialogService;

    public ObservableCollection<PickData> FileDataList { get; set; } = [];

    public DataTable? SiteErrorTable
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(SiteErrorTable));
        }
    }

    public string ViewKey => "SiteCompare";

    public string PluginName => "SiteCompare";

    public SiteCompareVM(IDataFactory dataFactory, IDialogService dialogService)
    {
        this._dataFactory = dataFactory;
        this._dialogService = dialogService;
    }

    public void Initalize()
    {
        var pickDatas = _dataFactory
            .GetAllFileData()
            .Select(fd => new PickData
            {
                FilePath = fd.FilePath,
                FileName = fd.FileName,
                FileData = fd,
                SiteNum = fd.SitesInfo.Keys.FirstOrDefault(),
                SitesList = fd.SitesInfo.Keys.Select(b => (int)b).ToList()
            });

        FileDataList = new ObservableCollection<PickData>(pickDatas);
    }

    [RelayCommand]
    private void GenerateSiteErrorReport()
    {
        var selectedFiles = FileDataList.Where(f => f.IsSelected == true).ToList();

        if (selectedFiles.Count == 0)
            return;

        if (selectedFiles.GroupBy(f => f.SiteNum).Any(g => g.Count() > 1))
        {
            _dialogService.ShowError("Please select only one site for each file.");
            return;
        }

        var dt = new DataTable();

        try
        {
            dt.Columns.Add("Index", typeof(string));
            dt.Columns.Add("TestId", typeof(string));
            dt.Columns.Add("TestName", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("LowLimit", typeof(float));
            dt.Columns.Add("HighLimit", typeof(float));

            var siteNumbers = new List<int>();

            foreach (var (i, pickData) in selectedFiles.Index())
            {
                if (pickData.SiteNum is null || pickData.FileData is null)
                    continue;
                var siteNum = pickData.SiteNum.Value;
                var fileData = pickData.FileData;
                dt.Columns.Add($"S{siteNum}_Mean", typeof(float));
                siteNumbers.Add(siteNum);
            }

            siteNumbers.Sort();

            for (int i = 0; i < siteNumbers.Count; i++)
            {
                for (int j = i + 1; j < siteNumbers.Count; j++)
                {
                    int site1 = siteNumbers[i];
                    int site2 = siteNumbers[j];

                    dt.Columns.Add($"S{site1}_vs_S{site2}_Diff", typeof(float));
                }
            }

            var firstFileData = selectedFiles.First().FileData;

            foreach (var item in firstFileData!.TestItemStatisticInfo)
            {
                if (
                    selectedFiles.Any(f =>
                        f.FileData.TestItemsInfo[item.Key].TestName != item.Value.TestName
                    )
                )
                {
                    _dialogService.ShowError("Please select files with same test items.");
                    return;
                }

                var testItem = item.Value;
                DataRow row = dt.NewRow();
                row["Index"] = testItem.Index;
                row["TestId"] = testItem.TestId;
                row["TestName"] = testItem.TestName;
                row["Unit"] = testItem.Unit;
                row["LowLimit"] = testItem.LowLimit;
                row["HighLimit"] = testItem.HighLimit;

                foreach (var (i, pickData) in FileDataList.Index())
                {
                    if (pickData.SiteNum is null || pickData.FileData is null)
                        continue;
                    var siteNum = pickData.SiteNum.Value;
                    var fileData = pickData.FileData;
                    row[$"S{siteNum}_Mean"] = testItem.SiteStatiscsInfo[(int)siteNum].Mean;
                }

                for (int i = 0; i < siteNumbers.Count; i++)
                {
                    for (int j = i + 1; j < siteNumbers.Count; j++)
                    {
                        int site1 = siteNumbers[i];
                        int site2 = siteNumbers[j];
                        row[$"S{site1}_vs_S{site2}_Diff"] =
                            (float)row[$"S{site1}_Mean"] - (float)row[$"S{site2}_Mean"];
                    }
                }

                dt.Rows.Add(row);
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError(ex.Message);
        }

        SiteErrorTable = dt;
    }
}
