using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.MainModule.ViewModel;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
// ReSharper disable PossibleInvalidCastExceptionInForeachLoop

namespace Arma3BE.Client.Modules.MainModule
{
    public class AvalonDockRegionAdapter : RegionAdapterBase<DockingManager>
    {
        #region Constructor

        public AvalonDockRegionAdapter(IRegionBehaviorFactory factory)
            : base(factory)
        {
        }

        #endregion  //Constructor


        #region Overrides

        protected override IRegion CreateRegion()
        {
            return new AllActiveRegion();
        }

        protected override void Adapt(IRegion region, DockingManager regionTarget)
        {
            region.Views.CollectionChanged += delegate (
                object sender, NotifyCollectionChangedEventArgs e)
            {
                OnViewsCollectionChanged(sender, e, regionTarget);
            };

            regionTarget.DocumentClosed += delegate (
                            object sender, DocumentClosedEventArgs e)
            {
                OnDocumentClosedEventArgs(sender, e, region);
            };
        }

        #endregion  //Overrides


        #region Event Handlers

        /// <summary>
        /// Handles the NotifyCollectionChangedEventArgs event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        /// <param name="regionTarget">The region target.</param>
        // ReSharper disable once UnusedParameter.Local
        void OnViewsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e, DockingManager regionTarget)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
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
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (FrameworkElement item in e.NewItems)
                {
                    UIElement view = item;

                    if (view != null)
                    {
                        //Create a new layout document to be included in the LayoutDocuemntPane (defined in xaml)
                        LayoutDocument newLayoutDocument = new LayoutDocument();
                        //Set the content of the LayoutDocument
                        newLayoutDocument.Content = item;
                        newLayoutDocument.Title = (item.DataContext as ITitledItem)?.Title;

                        ServerMonitorModel viewModel = item.DataContext as ServerMonitorModel;
                        if (viewModel != null)
                        {
                            //All my viewmodels have properties DisplayName and IconKey
                            newLayoutDocument.Title = viewModel.CurrentServer.Name;
                            viewModel.OpenServer();
                            newLayoutDocument.Closed += (s, a) =>
                            {
                                Task.Factory.StartNew(() =>
                                {
                                    viewModel.CloseServer();
                                    viewModel.Cleanup();
                                });
                            };
                        }

                        //Store all LayoutDocuments already pertaining to the LayoutDocumentPane (defined in xaml)
                        List<LayoutDocument> oldLayoutDocuments = new List<LayoutDocument>();
                        //Get the current ILayoutDocumentPane ... Depending on the arrangement of the views this can be either 
                        //a simple LayoutDocumentPane or a LayoutDocumentPaneGroup
                        ILayoutDocumentPane currentILayoutDocumentPane = (ILayoutDocumentPane)regionTarget.Layout.RootPanel.Children[0];

                        if (currentILayoutDocumentPane.GetType() == typeof(LayoutDocumentPaneGroup))
                        {
                            //If the current ILayoutDocumentPane turns out to be a group
                            //Get the children (LayoutDocuments) of the first pane
                            LayoutDocumentPane oldLayoutDocumentPane = (LayoutDocumentPane)currentILayoutDocumentPane.Children.ToList()[0];
                            foreach (LayoutDocument child in oldLayoutDocumentPane.Children)
                            {
                                oldLayoutDocuments.Insert(0, child);
                            }
                        }
                        else if (currentILayoutDocumentPane.GetType() == typeof(LayoutDocumentPane))
                        {
                            //If the current ILayoutDocumentPane turns out to be a simple pane
                            //Get the children (LayoutDocuments) of the single existing pane.
                            foreach (LayoutDocument child in currentILayoutDocumentPane.Children)
                            {
                                oldLayoutDocuments.Insert(0, child);
                            }
                        }

                        //Create a new LayoutDocumentPane and inserts your new LayoutDocument
                        LayoutDocumentPane newLayoutDocumentPane = new LayoutDocumentPane();
                        newLayoutDocumentPane.InsertChildAt(0, newLayoutDocument);

                        //Append to the new LayoutDocumentPane the old LayoutDocuments
                        foreach (LayoutDocument doc in oldLayoutDocuments)
                        {
                            newLayoutDocumentPane.InsertChildAt(0, doc);
                        }

                        //Traverse the visual tree of the xaml and replace the LayoutDocumentPane (or LayoutDocumentPaneGroup) in xaml
                        //with your new LayoutDocumentPane (or LayoutDocumentPaneGroup)
                        if (currentILayoutDocumentPane.GetType() == typeof(LayoutDocumentPane))
                            regionTarget.Layout.RootPanel.ReplaceChildAt(0, newLayoutDocumentPane);
                        else if (currentILayoutDocumentPane.GetType() == typeof(LayoutDocumentPaneGroup))
                        {
                            currentILayoutDocumentPane.ReplaceChild(currentILayoutDocumentPane.Children.ToList()[0], newLayoutDocumentPane);
                            regionTarget.Layout.RootPanel.ReplaceChildAt(0, currentILayoutDocumentPane);
                        }

                        newLayoutDocument.IsActive = true;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the DocumentClosedEventArgs event raised by the DockingNanager when
        /// one of the LayoutContent it hosts is closed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event.</param>
        /// <param name="region">The region.</param>
        // ReSharper disable once UnusedParameter.Local
        void OnDocumentClosedEventArgs(object sender, DocumentClosedEventArgs e, IRegion region)
        {
            region.Remove(e.Document.Content);
        }

        #endregion  //Event handlers
    }
}