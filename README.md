[通用插件开发文档（以SiteCompare为例）.md](https://github.com/user-attachments/files/23955313/SiteCompare.md)
# 通用插件开发文档（以SiteCompare为例）

# 第一章 文档引言

## 1.1 文档目的

本文档为StdfLab通用插件开发指导规范，以SiteCompare插件为实际案例，详细阐述插件开发的流程。旨在为开发人员提供标准化的开发指引，确保插件具备良好的可扩展性、兼容性及可维护性，同时帮助新手快速理解插件开发逻辑并上手实践。

## 1.2 术语与定义

- **插件（Plugin）**：依附于基础框架运行，可独立开发、部署，用于扩展框架核心功能的模块，具备低耦合、可插拔特性。

- **StdfLab框架**：专注于STDF（Standard Test Data Format）文件处理的基础框架，提供STDF文件解析、数据提取等核心能力。

- **STDF文件**：半导体测试领域常用的标准测试数据格式文件，包含测试项目、测试结果、站点信息等关键数据。

- **MVVM模式**：模型-视图-视图模型（Model-View-ViewModel）的架构模式，用于分离视图与业务逻辑，实现数据驱动开发。

## 1.3 示例工程

SiteCompare 工程源码：https://github.com/RickyHe7/SiteCompare.git



# 第二章 开发前期准备

## 2.1 需求分析与明确

插件开发前需清晰界定核心需求，包括功能目标、适用场景、输入输出规范及性能要求。以SiteCompare为例，需求分析结果如下：

- 核心功能：对比多个STDF文件中指定测试站点的测试项均值，生成包含差异分析的报告。

- 输入：STDF文件（支持多文件批量导入）、待分析的测试站点编号。

- 输出：可视化错误分析报告（含测试项名称、各站点均值、均值差异值）。

## 2.3 开发环境搭建

### 2.3.1 基础环境配置

通用插件开发基础环境需包含编程语言环境、IDE工具及基础框架依赖。以SiteCompare开发环境为例，配置步骤如下：

1. 安装IDE工具：推荐使用Visual Studio 2022（需勾选“.NET桌面开发”工作负载）。

    1. 安装.NET 10.0 SDK：从Visual Studio Installer安装.NET 10

### 2.3.2 插件项目初始化

基于基础框架的插件项目通常需遵循特定的项目结构规范，以实现与框架的无缝集成。SiteCompare项目初始化步骤参考：

1. 在Visual Studio中创建“WPF类库（.NET 10）”项目，项目名称命名为“SiteCompare.Plugin”。

2. 配置基础框架：在Visual Studio中通过NuGet包管理器安装StdfLab.Core、StdfLab.Extensions依赖库，确保版本兼容。

3. 配置项目输出：将输出类型设置为“类库”，输出目录指定为框架插件加载目录C:\ProgramData\StdfLab\Extensions（便于调试）也可以编译好后手动Copy到此目录下。

# 第三章 插件架构设计

插件架构设计需遵循“高内聚、低耦合、可扩展、易维护”的核心原则，同时满足与基础框架的交互规范。具体原则如下：

- **职责单一**：每个模块仅负责一项核心功能，如数据解析模块仅处理STDF文件读取。

- **依赖倒置**：依赖于抽象接口而非具体实现，便于后续替换模块（如更换报表生成工具）。

- **接口隔离**：插件与框架的交互通过专用接口实现，避免暴露内部细节。

# 第五章 插件使用

![Image](https://p3-flow-imagex-sign.byteimg.com/tos-cn-i-a9rns2rl98/c889d2ff4d834a4f8dda9fe6e7d32228.png~tplv-noop.jpeg?rk3s=49177a0b&x-expires=1764921864&x-signature=UAKOnqaOJcffbqrr0rgGtiZBMCY%3D&resource_key=c2e2b5df-5d51-4ba2-964e-788964bc4179&resource_key=c2e2b5df-5d51-4ba2-964e-788964bc4179)

在Extensions菜单栏打开定制插件

# 第四章 核心功能实现（以SiteCompare为例）

插件需严格实现基础框架定义的插件接口，确保框架能正确识别并加载。通用接口通常包含插件信息属性和生命周期方法。

### 4.1.1 实现框架插件接口



```Plain Text

using StdfLab.Core;
using StdfLab.Extensions;

/// <summary>
/// SiteCompare插件核心入口类
/// 实现StdfLab框架的IPlugin接口，完成插件注册、初始化及功能暴露
/// 核心功能：对比指定站点在两个STDF文件中测试项的均值
/// </summary>
/// <param name="dataFactory">框架注入的STDF数据工厂，用于获取STDF文件解析能力</param>
/// <param name="dialogService">框架注入的对话框服务，用于提供文件选择、提示等UI交互能力</param>
public class SiteErrorCompare(IDataFactory dataFactory, IDialogService dialogService) : IPlugin
{
    // 私有字段：存储框架注入的STDF数据工厂实例（用于后续数据解析）
    private readonly IDataFactory _dataFactory = dataFactory;
    // 私有字段：存储框架注入的对话框服务实例（用于后续UI交互）
    private readonly IDialogService _dialogService = dialogService;

    /// <summary>
    /// 插件名称（框架用于展示和识别插件）
    /// </summary>
    public string Name => "SiteCompare";

    /// <summary>
    /// 插件版本
    /// </summary>
    public string Version => "1.0";

    /// <summary>
    /// 插件功能描述
    /// 功能：对比指定站点在两个STDF文件中测试项的均值
    /// </summary>
    public string Description => "Compares average values of test items between two STDF files for a specific site.";

    /// <summary>
    /// 插件类型（指定插件在框架中的呈现形式：页面式）
    /// 框架根据此类型决定插件的加载和展示方式
    /// </summary>
    public PluginType PluginType => PluginType.Page;

    // 私有字段：插件视图模型实例
    private SiteCompareVM? viewModel;

    /// <summary>
    /// 插件初始化方法（框架加载插件时调用）
    /// 核心逻辑：创建视图模型实例并注入依赖服务
    /// </summary>
    /// <returns>异步任务（支持框架异步加载流程）</returns>
    public Task InitializeAsync()
    {
        // 初始化视图模型，将数据工厂和对话框服务注入（依赖注入解耦）
        viewModel = new SiteCompareVM(_dataFactory, _dialogService);
        // 返回已完成任务（无耗时操作时直接返回）
        return Task.CompletedTask;
    }

    /// <summary>
    /// 插件关闭方法（框架退出或卸载插件时调用）
    /// 此处无资源释放需求，留空便于后续扩展（如释放文件句柄、停止线程等）
    /// </summary>
    /// <returns>异步任务（支持框架异步卸载流程）</returns>
    public Task ShutdownAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 创建插件UI控件（框架用于展示插件界面）
    /// 核心逻辑：创建视图实例并绑定已初始化的视图模型
    /// </summary>
    /// <returns>插件页面接口实例（框架统一管理UI控件）</returns>
    public IPluginPage CreateControl()
    {
        // 实例化插件视图，由宿主程序导航到主窗口
        return new SiteErrorView { DataContext = viewModel };
    }

    /// <summary>
    /// 插件功能执行方法（框架触发插件功能时调用，如用户点击插件菜单）
    /// 核心逻辑：触发视图模型的初始化逻辑（如加载默认配置、初始化数据）
    /// </summary>
    /// <returns>异步任务（支持视图模型中异步初始化操作）</returns>
    public Task Execute()
    {
        viewModel?.Initalize();
        return Task.CompletedTask;
    }
}
```

SiteCompare代码示例（实现IPlugin接口）：

采用依赖注入（DI）模式管理服务实例，降低模块间耦合，便于后续扩展和单元测试。

可以在插件构造中引入服务，目前Core中已经实现的服务有：

**Dialog Service（对话框服务）**：框架提供的标准化UI交互服务，用于统一插件的对话框风格与交互逻辑，避免插件自行实现导致的界面不一致问题。核心作用是封装文件选择、消息提示、确认弹窗等常见UI交互场景，降低插件与UI框架的耦合。

该服务的核心方法及说明：

- `Task<string[]?> ShowOpenFileDialogAsync(string filter, bool multiselect = false)`：打开文件选择对话框，支持指定文件筛选规则（如“STDF文件 (*.stdf)|*.stdf”）和是否允许多选，返回选中的文件路径数组，若用户取消则返回null。

- `Task<string?> ShowSaveFileDialogAsync(string filter, string defaultFileName = "")`：打开文件保存对话框，支持指定文件格式和默认文件名，返回保存路径，取消则返回null。

- `Task ShowInformationDialogAsync(string title, string message)`：显示信息提示弹窗，用于反馈操作成功等非关键信息。

- `Task ShowErrorDialogAsync(string title, string message)`：显示错误提示弹窗，用于反馈解析失败、参数错误等异常场景。

- `Task<bool> ShowConfirmDialogAsync(string title, string message)`：显示确认弹窗，返回用户选择的结果（true为确认，false为取消），适用于危险操作前的二次确认。

在SiteCompare中的使用示例（ViewModel层调用）：

```csharp

// 注入对话框服务
private readonly IDialogService _dialogService;

// 选择STDF文件的方法（绑定到UI按钮）
public async Task SelectStdfFilesAsync()
{
    // 调用服务打开多文件选择对话框，仅允许选择.stdf格式
    var filePaths = await _dialogService.ShowOpenFileDialogAsync(
        filter: "STDF文件 (*.stdf)|*.stdf", 
        multiselect: true
    );
    
    if (filePaths != null && filePaths.Any())
    {
        // 处理选中的文件路径（如更新文件列表属性）
        SelectedFilePaths = filePaths.ToList();
    }
    else
    {
        // 提示用户未选择文件
        await _dialogService.ShowInformationDialogAsync("提示", "未选择任何STDF文件");
    }
}
```

**Data Factory Service（数据工厂服务）**：StdfLab框架核心数据服务，是插件与STDF文件数据交互的“桥梁”。该服务封装了STDF文件解析、数据提取、格式校验等底层能力，插件无需关注STDF文件的复杂结构，仅通过调用服务接口即可高效获取标准化的测试数据，大幅降低数据处理门槛。

作为框架的核心服务之一，Data Factory Service的核心价值在于提供统一的数据访问入口，确保不同插件解析STDF文件的逻辑一致性，同时屏蔽底层解析引擎的差异（如后续框架升级解析算法时，插件无需修改代码即可兼容）。



## 4.2 核心业务功能实现

SiteCompare代码示例：

功能：读取多个STDF文件，提取指定站点的测试项数据（测试项名称、测试结果值）。核心依赖StdfLab框架的解析能力，需处理文件格式校验、数据筛选逻辑。

功能：基于解析后的测试数据，计算每个测试项在各站点的均值，并对比站点间的均值差异（如最大值与最小值之差、与基准站点的差值等）。需处理数据为空、除数为零等异常场景。

SiteCompare代码示例（均值计算与差异对比方法）：

```csharp

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
```

### 4.2.3 视图与ViewModel交互实现

通过数据绑定和命令绑定实现视图与ViewModel的交互，确保UI操作能触发业务逻辑，数据更新能同步到UI。核心依赖CommunityToolkit.Mvvm的ObservableObject和RelayCommand。

```XML

<UserControl
    x:Class="SiteCompare.SiteErrorView"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:SiteCompare"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    d:DataContext="{d:DesignInstance Type=local:SiteCompareVM}"
    d:DesignHeight="450"
    d:DesignWidth="800"
    mc:Ignorable="d">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>
        <DataGrid
            Margin="10"
            AutoGenerateColumns="True"
            ItemsSource="{Binding SiteErrorTable}" />
        <StackPanel Grid.Column="1">
            <Button
                Width="180"
                Height="40"
                Margin="10"
                Command="{Binding GenerateSiteErrorReportCommand}"
                Content="GenerateSiteErrorReport" />
            <DataGrid AutoGenerateColumns="False" ItemsSource="{Binding FileDataList}">
                <DataGrid.Columns>
                    <DataGridCheckBoxColumn Binding="{Binding IsSelected}" Width="120" Header="Selected"/>
                    <DataGridTemplateColumn Header="SiteNum" Width="120">
                        <DataGridTemplateColumn.CellTemplate>
                            <DataTemplate>
                                <ComboBox ItemsSource="{Binding SitesList}" SelectedItem="{Binding SiteNum, Mode=TwoWay}" />
                            </DataTemplate>
                        </DataGridTemplateColumn.CellTemplate>
                    </DataGridTemplateColumn>
                    <DataGridTextColumn Binding="{Binding FileName}" Width="auto" Header="FileName" />
                </DataGrid.Columns>
            </DataGrid>
        </StackPanel>
    </Grid>
</UserControl>

```

## 5.2 调试方法

### 5.2.1 插件调试配置

在Visual Studio中配置调试参数，指定基础框架可执行文件作为启动程序，便于直接调试插件代码：

1. 启动外部程序StdfLab.exe。

2. Debug模式下Attach到目标程序。

3. 打断点开始调试。

# 第六章 插件打包与部署

## 6.1 插件打包规范

插件打包需遵循基础框架的要求，通常打包为独立的程序集文件（.dll），如需依赖第三方库，需明确依赖项管理方式。

### 6.1.1 打包步骤（暂时）

1. 右键插件项目 → “生成”，确保编译通过，生成目标.dll文件（位于bin/Debug或bin/Release目录）。

2. 在插件目录下新建文件夹，将输出目录下的dll复制到这里
> （注：文档部分内容可能由 AI 生成）
