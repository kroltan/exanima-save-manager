using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExanimaSaveManager {
    /// <summary>
    /// Interação lógica para SaveList.xaml
    /// </summary>
    public partial class SaveList {

        public SaveInformation Selected {
            get => (SaveInformation) GetValue(SelectedProperty);
            set => SetValue(SelectedProperty, value);
        }

        public ReadOnlyObservableCollection<SaveInformation> Items {
            get => (ReadOnlyObservableCollection<SaveInformation>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public SaveList() {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items),
            typeof(ReadOnlyObservableCollection<SaveInformation>),
            typeof(SaveList)
        );

        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register(
            nameof(Selected),
            typeof(SaveInformation),
            typeof(SaveList)
        );
    }
}
