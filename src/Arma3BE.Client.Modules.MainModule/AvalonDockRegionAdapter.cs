﻿using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.MainModule.ViewModel;
using Prism.Regions;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Arma3BE.Client.Modules.MainModule
{
    public class AvalonDockRegionAdapter : RegionAdapterBase<DockingManager>
    {
        public AvalonDockRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory) : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, DockingManager regionTarget)
        {
            region.Views.CollectionChanged += delegate(
                object sender, NotifyCollectionChangedEventArgs e)
            {
                OnViewsCollectionChanged(e, regionTarget);
            };

            regionTarget.DocumentClosed += async (s, e) =>
            {
                await CloseView(region, regionTarget, e.Document);
            };
        }

        private async Task CloseView(IRegion region, DockingManager regionTarget, LayoutDocument document)
        {
            region.Remove(document.Content);

            var viewModel = (document.Content as FrameworkElement)?.DataContext as ServerMonitorModel;
            if (viewModel != null)
            {
                await viewModel.CloseServerAsync();
                viewModel.Cleanup();
            }

            if (!region.Views.Any())
            {
                regionTarget.ActiveContent = null;
            }
        }

        async void OnViewsCollectionChanged(NotifyCollectionChangedEventArgs e, DockingManager regionTarget)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    foreach (FrameworkElement item in e.NewItems)
                    {
                        //Create a new layout document to be included in the LayoutDocuemntPane (defined in xaml)
                        LayoutDocument newLayoutDocument = new LayoutDocument
                        {
                            Content = item,
                            Title = (item.DataContext as ITitledItem)?.Title
                        };
                        //Set the content of the LayoutDocument

                        var viewModel = item.DataContext as ServerMonitorModel;
                        if (viewModel != null)
                        {
                            //All my viewmodels have properties DisplayName and IconKey
                            newLayoutDocument.Title = viewModel.CurrentServer.Name;
                            await viewModel.OpenServerAsync();
                        }

                        var layoutDocumentPane = new LayoutDocumentPane();

                        if (regionTarget.Layout.RootPanel.Children.Count == 0)
                        {
                            regionTarget.Layout.RootPanel.Children.Add(layoutDocumentPane);
                        }
                        else
                        {
                            layoutDocumentPane = regionTarget.Layout.RootPanel.Children[0] as LayoutDocumentPane;
                        }

                        layoutDocumentPane?.Children?.Add(newLayoutDocument);
                        regionTarget.ActiveContent = newLayoutDocument.Content;
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var eOldItem in e.OldItems)
                    {
                        var disposables = new[]
                        {
                            eOldItem as IDisposable,
                            (eOldItem as FrameworkElement)?.DataContext as IDisposable,
                            (eOldItem as ContentControl)?.Content as IDisposable,
                            ((eOldItem as ContentControl)?.Content as FrameworkElement)?.DataContext as IDisposable
                        };

                        foreach (var disposable in disposables)
                        {
                            disposable?.Dispose();
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}