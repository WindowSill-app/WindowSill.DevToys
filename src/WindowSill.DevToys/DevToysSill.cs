﻿using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Microsoft.UI.Xaml.Media.Imaging;
using WindowSill.API;
using WindowSill.DevToys.Activators;
using WindowSill.DevToys.Core;
using Path = System.IO.Path;

namespace WindowSill.DevToys;

[Export(typeof(ISill))]
[Name("DevToys")]
internal sealed class DevToysSill : ISillActivatedByTextSelection, ISillListView
{
    private readonly DevToysSillViewModel _viewModel = new();

    [Import]
    private IPluginInfo _pluginInfo = null!;

    public string[] TextSelectionActivatorTypeNames { get; }
        = [
            Base64ImageSelection.ActivationTypeName,
            Base64TextSelection.ActivationTypeName,
            EscapedCharactersSelection.ActivationTypeName,
            PredefinedActivationTypeNames.JsonSelection,
            JsonArraySelection.ActivationTypeName,
            XmlSelection.ActivationTypeName,
            XsdSelection.ActivationTypeName
            ];

    public string DisplayName => "/WindowSill.DevToys/Misc/DisplayName".GetLocalizedString();

    public SillSettingsView[]? SettingsViews => throw new NotImplementedException();

    public ObservableCollection<SillListViewItem> ViewList { get; } = new();

    public SillView? PlaceholderView => throw new NotImplementedException();

    public IconElement CreateIcon()
        => new ImageIcon
        {
            Source = new SvgImageSource(new Uri(Path.Combine(_pluginInfo.GetPluginContentDirectory(), "Assets", "devtoys.svg")))
        };

    public async ValueTask OnActivatedAsync(string textSelectionActivatorTypeName, WindowTextSelection currentSelection)
    {
        await ThreadHelper.RunOnUIThreadAsync(() =>
        {
            ViewList.Clear();

            switch (textSelectionActivatorTypeName)
            {
                case PredefinedActivationTypeNames.JsonSelection:
                case JsonArraySelection.ActivationTypeName:
                    if (!currentSelection.IsReadOnly)
                    {
                        ViewList.Add(
                            new SillListViewMenuFlyoutItem(
                                "/WindowSill.DevToys/Misc/Format".GetLocalizedString(),
                                null,
                                new MenuFlyout
                                {
                                    Items =
                                    {
                                    new MenuFlyoutItem
                                    {
                                        Text = "/WindowSill.DevToys/Misc/Minified".GetLocalizedString(),
                                        Command = _viewModel.FormatJsonCommand,
                                        CommandParameter = (currentSelection, Indentation.Minified)
                                    },
                                    new MenuFlyoutItem
                                    {
                                        Text = "/WindowSill.DevToys/Misc/TwoSpaces".GetLocalizedString(),
                                        Command = _viewModel.FormatJsonCommand,
                                        CommandParameter = (currentSelection, Indentation.TwoSpaces)
                                    },
                                    new MenuFlyoutItem
                                    {
                                        Text = "/WindowSill.DevToys/Misc/FourSpaces".GetLocalizedString(),
                                        Command = _viewModel.FormatJsonCommand,
                                        CommandParameter = (currentSelection, Indentation.FourSpaces)
                                    },
                                    new MenuFlyoutItem
                                    {
                                        Text = "/WindowSill.DevToys/Misc/OneTab".GetLocalizedString(),
                                        Command = _viewModel.FormatJsonCommand,
                                        CommandParameter = (currentSelection, Indentation.OneTab)
                                    }
                                    }
                                }));
                    }
                    break;
            }
        });
    }

    public async ValueTask OnDeactivatedAsync()
    {
        await ThreadHelper.RunOnUIThreadAsync(() =>
        {
            ViewList.Clear();
        });
    }
}
