//  --------------------------------
//  Copyright (c) Huy Pham. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.opensource.org/licenses/ms-pl.html
//  ---------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace GemScopeWPF.UI
{
    public class DropDownButton : ToggleButton
    {
        #region Dependency Properties

        public static readonly DependencyProperty DropDownContextMenuProperty = DependencyProperty.Register("DropDownContextMenu", typeof(ContextMenu), typeof(DropDownButton), new UIPropertyMetadata(null));
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(DropDownButton));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DropDownButton));
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(UIElement), typeof(DropDownButton));
        public static readonly DependencyProperty DropDownButtonCommandProperty = DependencyProperty.Register("DropDownButtonCommand", typeof(ICommand), typeof(DropDownButton), new FrameworkPropertyMetadata(null));
       
        #endregion

        #region Constructors

        public DropDownButton()
        {
            // Bind the ToogleButton.IsChecked property to the drop-down's IsOpen property 
            var binding = new Binding("DropDownContextMenu.IsOpen") {Source = this};         
            SetBinding(IsCheckedProperty, binding);
        }

        #endregion

        #region Properties

        public ContextMenu DropDownContextMenu
        {
            get { return GetValue(DropDownContextMenuProperty) as ContextMenu; }
            set { SetValue(DropDownContextMenuProperty, value); }
        }

        public ImageSource Image
        {
            get { return GetValue(ImageProperty) as ImageSource; }
            set { SetValue(ImageProperty, value); }
        }

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        public UIElement Target
        {
            get { return GetValue(TargetProperty) as UIElement; }
            set { SetValue(TargetProperty, value); }
        }

        public ICommand DropDownButtonCommand
        {
            get { return GetValue(DropDownButtonCommandProperty) as ICommand; }
            set { SetValue(DropDownButtonCommandProperty, value); }
        }

        #endregion

        #region Protected Override Methods

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == DropDownButtonCommandProperty)
                Command = DropDownButtonCommand;
        }

        protected override void OnClick()
        {
            if (DropDownContextMenu == null) return;

            if (DropDownButtonCommand != null) DropDownButtonCommand.Execute(null);

            // If there is a drop-down assigned to this button, then position and display it 
            DropDownContextMenu.PlacementTarget = this;
            DropDownContextMenu.Placement = PlacementMode.Bottom;
            DropDownContextMenu.IsOpen = !DropDownContextMenu.IsOpen;
        }

        #endregion
    }
}