﻿using Avalonia.Controls;
using Avalonia.Interactivity;
using Multi_Saves_Backup_Tool.ViewModels;

namespace Multi_Saves_Backup_Tool.Views;

public partial class GamesView : UserControl
{
    private readonly GamesViewModel _viewModel;

    public GamesView()
    {
        InitializeComponent();
        _viewModel = new GamesViewModel();
        DataContext = _viewModel;
        Loaded += GamesView_Loaded;
    }

    private void GamesView_Loaded(object? sender, RoutedEventArgs e)
    {
        foreach (var game in _viewModel.Games)
        {
            _viewModel.UpdateBackupCount(game);
        }
    }

    private void AddGameButton_Click(object sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is MainWindow mainWindow && mainWindow.AddGameOverlay != null)
        {
            var overlay = mainWindow.AddGameOverlay;
            overlay.Initialize(_viewModel);
            overlay.Show();
        }
    }
}