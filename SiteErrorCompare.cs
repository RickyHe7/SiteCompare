using StdfLab.Core;
using StdfLab.Extensions;
using System.Windows;

namespace SiteCompare;

public class SiteErrorCompare(IDataFactory dataFactory, IDialogService dialogService) : IPlugin
{
    private readonly IDataFactory _dataFactory = dataFactory;
    private readonly IDialogService _dialogService = dialogService;


    public string Name => "SiteCompare";

    public string Version => "1.0";

    public string Description => "Compares average values of test items between two STDF files for a specific site.";

    public PluginType PluginType => PluginType.Window | PluginType.Page;

    private SiteCompareVM? viewModel;

    public Task InitializeAsync()
    {
        viewModel = new SiteCompareVM(_dataFactory,_dialogService);
        return Task.CompletedTask;
    }

    public Task ShutdownAsync()
    {
        return Task.CompletedTask;
    }

    public IPluginPage CreateControl()
    {
        return new SiteErrorView { DataContext = viewModel };
    }

    public Task Execute()
    {
        viewModel?.Initalize();
        return Task.CompletedTask;
    }
}
