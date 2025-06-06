using System;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Multi_Saves_Backup_Tool.Models;
using Properties;

namespace Multi_Saves_Backup_Tool.ViewModels;

public partial class AddGameOverlayViewModel : ViewModelBase
{
    private readonly GamesViewModel _gamesViewModel;

    [ObservableProperty] private string _addPath = string.Empty;

    [ObservableProperty] private int _backupInterval = 60;

    [ObservableProperty] private int _backupMode;

    [ObservableProperty] private int _daysForKeep;

    [ObservableProperty] private string _gameExe = string.Empty;

    [ObservableProperty] private string _gameExeAlt = string.Empty;

    [ObservableProperty] private string _gameExeError = string.Empty;

    [ObservableProperty] private string _gameName = string.Empty;

    [ObservableProperty] private string _gameNameError = string.Empty;

    [ObservableProperty] private bool _includeTimestamp = true;

    [ObservableProperty] private string _modPath = string.Empty;

    [ObservableProperty] private int _oldFilesStatus;

    [ObservableProperty] private string _saveLocation = string.Empty;

    [ObservableProperty] private string _saveLocationError = string.Empty;

    [ObservableProperty] private string _errorMessage = string.Empty;

    public AddGameOverlayViewModel(GamesViewModel gamesViewModel)
    {
        _gamesViewModel = gamesViewModel;
    }

    public event EventHandler? CloseRequested;
    public event EventHandler<GameModel>? GameAdded;

    [RelayCommand]
    private async Task BrowseSaveLocation(IStorageProvider? storageProvider)
    {
        var folderPath = await BrowseFolder(storageProvider);
        if (!string.IsNullOrEmpty(folderPath))
            SaveLocation = folderPath;
    }

    [RelayCommand]
    private async Task BrowseModPath(IStorageProvider? storageProvider)
    {
        var folderPath = await BrowseFolder(storageProvider);
        if (!string.IsNullOrEmpty(folderPath))
            ModPath = folderPath;
    }

    [RelayCommand]
    private async Task BrowseAddPath(IStorageProvider? storageProvider)
    {
        var folderPath = await BrowseFolder(storageProvider);
        if (!string.IsNullOrEmpty(folderPath))
            AddPath = folderPath;
    }

    [RelayCommand]
    private async Task BrowseGameExe(IStorageProvider? storageProvider)
    {
        var filePath = await BrowseExecutableFile(storageProvider);
        if (!string.IsNullOrEmpty(filePath))
            GameExe = filePath;
    }

    [RelayCommand]
    private async Task BrowseGameExeAlt(IStorageProvider? storageProvider)
    {
        var filePath = await BrowseExecutableFile(storageProvider);
        if (!string.IsNullOrEmpty(filePath))
            GameExeAlt = filePath;
    }

    private void ClearErrors()
    {
        GameNameError = string.Empty;
        GameExeError = string.Empty;
        SaveLocationError = string.Empty;
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(GameName))
        {
            ErrorMessage = Resources.ErrorGameNameRequired;
            return false;
        }

        if (string.IsNullOrWhiteSpace(GameExe))
        {
            ErrorMessage = Resources.ErrorGameExeRequired;
            return false;
        }

        if (string.IsNullOrWhiteSpace(SaveLocation))
        {
            ErrorMessage = Resources.ErrorSaveLocationRequired;
            return false;
        }

        ErrorMessage = string.Empty;
        return true;
    }

    [RelayCommand]
    private void AddGame()
    {
        if (!ValidateForm())
        {
            return;
        }

        var newGame = new GameModel
        {
            GameName = GameName,
            GameExe = GameExe,
            GameExeAlt = string.IsNullOrEmpty(GameExeAlt) ? null : GameExeAlt,
            SavePath = SaveLocation,
            ModPath = string.IsNullOrEmpty(ModPath) ? null : ModPath,
            AddPath = string.IsNullOrEmpty(AddPath) ? null : AddPath,
            DaysForKeep = DaysForKeep,
            SetOldFilesStatus = OldFilesStatus,
            BackupInterval = BackupInterval,
            IsEnabled = true
        };

        AddGame(newGame);
        GameAdded?.Invoke(this, newGame);
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private async Task<string> BrowseFolder(IStorageProvider? storageProvider)
    {
        if (storageProvider != null)
        {
            var folder = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Folder",
                AllowMultiple = false
            });

            return folder.Count > 0 ? folder[0].Path.LocalPath : string.Empty;
        }

        return string.Empty;
    }

    private async Task<string> BrowseExecutableFile(IStorageProvider? storageProvider)
    {
        if (storageProvider != null)
        {
            var options = new FilePickerOpenOptions
            {
                Title = "Select Game Executable",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Executable Files")
                    {
                        Patterns = new[] { "*.exe" },
                        MimeTypes = new[] { "application/x-msdownload" }
                    }
                }
            };

            var files = await storageProvider.OpenFilePickerAsync(options);
            return files.Count > 0 ? files[0].Path.LocalPath : string.Empty;
        }

        return string.Empty;
    }

    private void AddGame(GameModel newGame)
    {
        _gamesViewModel.AddGame(newGame);
    }

    public void ClearForm()
    {
        GameName = string.Empty;
        GameExe = string.Empty;
        GameExeAlt = string.Empty;
        SaveLocation = string.Empty;
        ModPath = string.Empty;
        AddPath = string.Empty;
        DaysForKeep = 0;
        OldFilesStatus = 0;
        IncludeTimestamp = true;
        BackupMode = 0;
        BackupInterval = 60;
    }
}