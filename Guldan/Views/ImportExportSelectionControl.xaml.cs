﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Guldan.ViewModel;

namespace Guldan.Views
{
    /// <summary>
    /// ImportExportSelectionControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ImportExportSelectionControl : UserControl
    {
        #region Fields

        bool _headerFlag = false;
        bool _itemFlag = false;

        #endregion

        #region Events

        #region SelectionChanged

        /// <summary>
        /// SelectionChanged Routed Event
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImportExportSelectionControl));

        /// <summary>
        /// Occurs when the user selects/deselects an item in the list
        /// </summary>
        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the SelectionChanged event.
        /// </summary>
        protected RoutedEventArgs RaiseSelectionChangedEvent()
        {
            return RaiseSelectionChangedEvent(this);
        }

        /// <summary>
        /// A static helper method to raise the SelectionChanged event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        public static RoutedEventArgs RaiseSelectionChangedEvent(DependencyObject target)
        {
            if (target == null)
                return null;

            var args = new RoutedEventArgs
            {
                RoutedEvent = SelectionChangedEvent
            };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #endregion

        #region Dependency Properties

        #region Header

        /// <summary>
        /// Header Dependency Property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ImportExportSelectionControl),
                new PropertyMetadata(string.Empty,
                    new PropertyChangedCallback(OnHeaderChanged)));

        /// <summary>
        /// Gets or sets the Header property. This dependency property 
        /// indicates the Name to be displayed in the Header.
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Header property.
        /// </summary>
        /// <param name="d">ImportExportSelectionControl</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (ImportExportSelectionControl)d;
            var oldHeader = (string)e.OldValue;
            var newHeader = ctrl.Header;
            ctrl.OnHeaderChanged(oldHeader, newHeader);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Header property.
        /// </summary>
        /// <param name="oldHeader">Old Value</param>
        /// <param name="newHeader">New Value</param>
        void OnHeaderChanged(string oldHeader, string newHeader)
        {

        }

        #endregion

        #region Data

        /// <summary>
        /// Data Dependency Property
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(IEnumerable<ImportExportVM>), typeof(ImportExportSelectionControl),
                new PropertyMetadata(null,
                    new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Gets or sets the Data property. This dependency property 
        /// indicates the list of Environment Variables to be displayed.
        /// </summary>
        public IEnumerable<ImportExportVM> Data
        {
            get { return (IEnumerable<ImportExportVM>)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Data property.
        /// </summary>
        /// <param name="d">ImportExportSelectionControl</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (ImportExportSelectionControl)d;
            var oldData = (IEnumerable<ImportExportVM>)e.OldValue;
            var newData = ctrl.Data;
            ctrl.OnDataChanged(oldData, newData);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Data property.
        /// </summary>
        /// <param name="oldData">Old Value</param>
        /// <param name="newData">New Value</param>
        void OnDataChanged(IEnumerable<ImportExportVM> oldData, IEnumerable<ImportExportVM> newData)
        {
            DataList.ItemsSource = newData;
        }

        #endregion

        #endregion

        #region Construction / Initialization

        /// <summary>
        /// Ctor
        /// </summary>
        public ImportExportSelectionControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the checked/unchecked events of the Header checkbox
        /// </summary>
        /// <param name="sender">Header Checkbox</param>
        /// <param name="e">RoutedEventArgs</param>
        private void HandleHeaderSelection(object sender, RoutedEventArgs e)
        {
            if (_itemFlag)
                return;

            _headerFlag = true;

            foreach (ImportExportVM item in DataList.Items)
            {
                item.IsSelected = HeaderCB.IsChecked;
            }

            // Raise the event to notify that the selection has changed
            RaiseSelectionChangedEvent(this);

            _headerFlag = false;
        }

        /// <summary>
        /// Handles the checked/unchecked events of the item checkbox
        /// in the items list
        /// </summary>
        /// <param name="sender">Item Checkbox</param>
        /// <param name="e">RoutedEventArgs</param>
        private void HandleItemSelection(object sender, RoutedEventArgs e)
        {
            if (_headerFlag)
                return;

            _itemFlag = true;

            UpdateHeaderSelection();
            // Raise the event to notify that the selection has changed
            RaiseSelectionChangedEvent(this);

            _itemFlag = false;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Updates the check state of the Header based on the check states
        /// of the items.
        /// </summary>
        private void UpdateHeaderSelection()
        {
            bool? state = null;

            var firstCheck = true;
            foreach (var currentState in from ImportExportVM item in DataList.Items
                select item.IsSelected)
            {
                if (firstCheck)
                {
                    state = currentState;
                    firstCheck = false;
                }
                else if (state != currentState)
                {
                    state = null;
                    break;
                }
            }

            HeaderCB.IsChecked = state;
        }

        #endregion
    }
}
