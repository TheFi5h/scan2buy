﻿using System.Collections.ObjectModel;
using System.Windows;
using Domain;

namespace s2b_core_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>// Trigger event
    public partial class MainWindow : Window
    {
        private readonly ShoppingCart _shoppingCart;
        private ReadOnlyCollection<ShoppingCartEntry> _entries;

        public MainWindow()
        {
            InitializeComponent();

            // Initialising own procedures

            _shoppingCart = new ShoppingCart();                  // create new shoppingCart
            DataGridEntries.ItemsSource = _shoppingCart.GetEntries();
            DataGridEntries.ItemsSource = _entries;
            DataGridEntries.AutoGenerateColumns = true;         // automatically binds all public properties of shopping cart entry to one column each
            DataGridEntries.IsReadOnly = true;                  // So that the user cant move items 
            _shoppingCart.OnEntryChanged += ShoppingCartGuard;  // Subscribe to OnEntryChangedEvent
            _shoppingCart.Start();                              // activate Scanning for targets

        }

        public void ShoppingCartGuard(ShoppingCart.NewEntryEventArgs e)
        {
            // Update labels
            LabelArticleCountVar.Content = _shoppingCart.GetCountAllArticles();
            LabelPriceVar.Content = _shoppingCart.GetPrice() + "€";
            
            // Update table
            _entries = (ReadOnlyCollection<ShoppingCartEntry>) _shoppingCart.GetEntries();   // readonly list of all entries in the cart
        }
    }
}
