﻿using MyUILibrary;
using MyUILibrary.Temp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace MyUIGenerator.UIControlHelper
{
    public abstract class BaseControlHelper
    {
        public Grid theGrid;
        public abstract FrameworkElement MainControl { get; }
        public FrameworkElement WholeControl { get { return theGrid; } }
        RadDropDownButton dropDownButton { set; get; }
        ListBox listBox { set; get; }
        public Brush DefaultBorderBrush { set; get; }
        public Thickness DefaultBorderThickness { set; get; }
        public Brush DefaultBackground { set; get; }
        public Brush DefaultForeground { set; get; }

        public BaseControlHelper()
        {
            if (MainControl is Control)
            {
            
            }
            else if (MainControl is TextBlock)
            {
                DefaultBackground = (MainControl as TextBlock).Background;
                DefaultForeground = (MainControl as TextBlock).Foreground;
            }
        }
        public void AddButtonMenu(ConrolPackageMenu menu)
        {
            if (dropDownButton == null)
            {
                dropDownButton = new RadDropDownButton();
                dropDownButton.Height = 22;
                dropDownButton.Width = 60;
                //dropDownButton.HorizontalAlignment = HorizontalAlignment.Left;
                dropDownButton.Margin = new System.Windows.Thickness(2);
                listBox = new ListBox();
                dropDownButton.DropDownContent = listBox;
                dropDownButton.Content = "منو";
                var listitem = new ListBoxItem();
                dropDownButton.HorizontalAlignment = HorizontalAlignment.Right;

                theGrid.ColumnDefinitions.Add(new ColumnDefinition());
                Grid.SetColumn(dropDownButton, theGrid.ColumnDefinitions.Count);
                theGrid.Children.Add(dropDownButton);



            }

            var menuButton = new Button();
            menuButton.Name = menu.Name;
            menuButton.Content = menu.Title;
            if (!string.IsNullOrEmpty(menu.Tooltip))
                ToolTipService.SetToolTip(menuButton, menu.Tooltip);
            menuButton.Click += (sender, e) => MenuButton_Click(sender, e, menu);
            listBox.Items.Add(menuButton);
            //(item as CommandManager).Button.HorizontalContentAlignment = HorizontalAlignment.Left;


            //theGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
            //var menuButton = new Button();
            //menuButton.Name = menu.Name;
            //menuButton.Content = menu.Title;
            //menuButton.Click += (sender, e) => MenuButton_Click(sender, e, menu);
            //Grid.SetColumn(menuButton, theGrid.ColumnDefinitions.Count);
            //theGrid.Children.Add(menuButton);
        }
        private void MenuButton_Click(object sender, RoutedEventArgs e, ConrolPackageMenu menu)
        {
            ConrolPackageMenuArg arg = new ConrolPackageMenuArg();
            menu.OnMenuClicked(sender, arg);
        }

        public void RemoveButtonMenu(string name)
        {
            List<object> removeButtons = new List<object>();

            foreach (var item in listBox.Items)
            {
                if (item is Button)
                {
                    if ((item as Button).Name == name)
                    {
                        removeButtons.Add(item);
                    }
                }
            }
            removeButtons.ForEach(x => listBox.Items.Remove(x));
        }

        public void Visiblity(bool visible)
        {
            (WholeControl as UIElement).Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        //public void SetBorderColor(InfoColor color)
        //{
        //    (MainControl as Control).BorderBrush = UIManager.GetColorFromInfoColor(color);
        //    (MainControl as Control).BorderThickness = new Thickness(1);
        //}
        //public void SetBackgroundColor(InfoColor color)
        //{
        //    (MainControl as Control).Background = UIManager.GetColorFromInfoColor(color);
        //}
        //public void SetForegroundColor(InfoColor color)
        //{
        //    (MainControl as Control).Foreground = UIManager.GetColorFromInfoColor(color);
        //}


     
     

        public object GetUIControl()
        {
            return WholeControl;
        }

        public void SetTooltip(string tooltip)
        {
            if (!string.IsNullOrEmpty(tooltip))
                ToolTipService.SetToolTip(WholeControl, tooltip);
            else
                ToolTipService.SetToolTip(WholeControl, null);

        }

        //public void SetColor(InfoColor color)
        //{
        //    (MainControl as Control).BorderBrush = UIManager.GetColorFromInfoColor(color);
        //    (MainControl as Control).BorderThickness = new Thickness(1);
        //}
    }
}
