using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CheckHardwareInfo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        SystemInfo info = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            info = new SystemInfo();
            foreach (var item in SelectCheck.Items)
            {
                if (item is MenuItem)
                {
                    MenuItem temp = item as MenuItem;
                    if (temp.IsCheckable)
                    {
                        GroupBox groupBox = new GroupBox();
                        groupBox.Visibility = Visibility.Collapsed;
                        info.CheckInfo(temp.Name, groupBox);
                        SystemInfoGrid.Children.Add(groupBox);
                    }
                }
            }
        }

        private void CheckAllMenu_Click(object sender, RoutedEventArgs e)
        {
            foreach(UIElement item in SystemInfoGrid.Children)
            {
                item.Visibility = Visibility.Visible;
            }

            Save.IsEnabled = true;
        }

        private void CheckSelected_Click(object sender, RoutedEventArgs e)
        {
            info = new SystemInfo();
            IEnumerator SystemInfoGrid_enumerator = SystemInfoGrid.Children.GetEnumerator();
            foreach (var item in SelectCheck.Items)
            {
                if(item is MenuItem)
                {
                    MenuItem temp = item as MenuItem;
                    if(temp.IsCheckable)
                    {
                        SystemInfoGrid_enumerator.MoveNext();
                        UIElement SystemInfoGrid_element = SystemInfoGrid_enumerator.Current as UIElement;
                        if (temp.IsChecked)
                        {
                            SystemInfoGrid_element.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            SystemInfoGrid_element.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }

            Save.IsEnabled = true;
        }

        private void ReverseSelected_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in SelectCheck.Items)
            {
                if (item is MenuItem)
                {
                    MenuItem temp = item as MenuItem;
                    if(temp.IsCheckable == true)
                        temp.IsChecked = !temp.IsChecked;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(!Save.IsEnabled)
            {
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                DefaultExt = "xml",
                Filter = "xml Files (*.xml)|*.xml|html Files (*.html)|*.html|json Files (*.json)|*.json|yaml Files(*.yaml)|*.yaml|All Files (*.*)|*.*",
                FilterIndex = 1
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                WriterBase writerBase = WriterBase.CreateWriterInstance(info, saveFileDialog.FileName);
                if (writerBase != null)
                {
                    writerBase.WriteToFile();
                }
            }
        }

        private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
