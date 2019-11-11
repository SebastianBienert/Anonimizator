using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Anonimizator.WPF
{
    public class CustomListBox : ListBox
    {
        public CustomListBox()
        {
            this.SelectionChanged += CustomDataGrid_SelectionChanged;
            SelectedItemsList = new List<object>();
        }

        void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItemsList = this.SelectedItems;
        }

        public IList SelectedItemsList
        {
            get => (IList)GetValue(SelectedItemsListProperty);
            set => SetValue(SelectedItemsListProperty, value);
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
            DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(CustomListBox), new PropertyMetadata(null));
    }
}
