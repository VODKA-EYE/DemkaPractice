using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PracticeIK.Context;
using PracticeIK.Models;

namespace PracticeIK;

public partial class MainWindow : Window
{
  private User907Context dbcontext = new();
  private int page = 0;
  private int maxPage = 0;
  private int pageAmount = 0;
  private int clientsAmount;
  private int clientsAmountAfterFilters;

  public MainWindow()
  {
    InitializeComponent();
    SearchTextBox.AddHandler(KeyUpEvent, OnSearchBoxTextChanging, RoutingStrategies.Tunnel);
    
    PaginationComboBox.SelectionChanged += PaginationComboBoxSelectionChanged;
    AdditionalFiltersComboBox.SelectionChanged += ComboBoxSelectionChanged;
    GenderComboBox.SelectionChanged += ComboBoxSelectionChanged;
    BDThisMonthCB.Click += CheckBoxSelectionChanged;
    
    LoadGenders();
    LoadClients();
  }

  private void LoadClients()
  {
    List<Client> clients = dbcontext.Clients.ToList();
    
    // Overall amount of clients
    clientsAmount = clients.Count();
    ClientsTotalTB.Text = clientsAmount.ToString();
    
    // Sort by genders
    if (GenderComboBox.SelectedIndex != 0)
    {
      clients = clients.Where(c => c.Gendercode == GenderComboBox.SelectedIndex).ToList();
    }

    // Sort by this month's bdays
    if (BDThisMonthCB.IsChecked.Value)
    {
      clients = clients.Where(c => c.Birthday.Value.Month == DateTime.Now.Month).ToList();
    }

    // Search bar
    string searchString = SearchTextBox.Text ?? "";
    searchString = searchString.ToLower();
    string[] searchStringElements = searchString.Split(' ');

    if (!string.IsNullOrEmpty(searchString))
    {
      foreach (var element in searchStringElements)
      {
        clients = clients.Where(c =>
          c.Firstname.ToLower().Contains(element) ||
          c.Lastname.ToLower().Contains(element) ||
          c.Patronymic.ToLower().Contains(element) ||
          c.Email.ToLower().Contains(element) ||
          c.Phone.ToLower().Contains(element)
        ).ToList();
      }
    }
    
    // Filters
    switch (AdditionalFiltersComboBox.SelectedIndex)
    {
      case 0:
        clients = clients.OrderBy(c => c.Id).ToList();
        break;
      case 1:
        clients = clients.OrderBy(c => c.Lastname).ToList();
        break;
      case 2:
        clients = clients.OrderByDescending(c => c.Clientservices.Count).ToList();
        break;
      case 3:
        clients = clients.OrderByDescending(c => c.LastVisit).ToList();
        break;
    }

    clientsAmountAfterFilters = clients.Count();
    
    switch (PaginationComboBox.SelectedIndex)
    {
      case 0:
        pageAmount = 0;
        PageNumberMaxTB.Text = "1";
        break;
      case 1:
        MaxPageSetup(10);
        break;
      case 2:
        MaxPageSetup(50);
        break;
      case 3:
        MaxPageSetup(100);
        break;
    }
    
    
    // Client pagination
    if (pageAmount != 0)
    {
      clients = clients.Skip(page * pageAmount).Take(pageAmount).ToList();
      
      ClientsShownFromTB.Text = (page * pageAmount).ToString();
      ClientsShownToTB.Text = (page * pageAmount + clients.Count).ToString();
    }
    else
    {
      ClientsShownFromTB.Text = "0";
      ClientsShownToTB.Text = clients.Count.ToString();
    }
    
    // Load filtered clients into listbox
    ClientsListBox.ItemsSource = clients;
  }

  private void LoadGenders()
  {
    List<Gender> genders = dbcontext.Genders.ToList();
    genders.Insert(0, new Gender()
    {
      Code = 0,
      Name = "Любой пол"
    });
    GenderComboBox.ItemsSource = genders;
    GenderComboBox.SelectedIndex = 0;
  }

  private void NextPageClick(object? sender, RoutedEventArgs e)
  {
    if (maxPage != page + 1 & PaginationComboBox.SelectedIndex != 0)
    {
      page++;
      PageNumberTB.Text = (page + 1).ToString();
      LoadClients();
    }
  }

  private void PreviousPageClick(object? sender, RoutedEventArgs e)
  {
    if (page != 0 & PaginationComboBox.SelectedIndex != 0)
    {
      page--;
      PageNumberTB.Text = (page + 1).ToString();
      LoadClients();
    }
  }
  
  private async void AddClientButtonClick(object? sender, RoutedEventArgs e)
  {
    AddClient addClient = new();
    await addClient.ShowDialog(this);
    dbcontext = new();
    LoadClients();
  }

  private void EditClientClick(object sender, SelectionChangedEventArgs e)
  {
    var listboxObject = ClientsListBox.SelectedItem;
    if (listboxObject != null)
    {
      System.Reflection.PropertyInfo pi = listboxObject.GetType().GetProperty("Id");
      int id = (int)(pi.GetValue(listboxObject, null));
      AddClient addClient = new(id);
      addClient.ShowDialog(this);
      addClient.Closed += (o, arg) =>
      {
        dbcontext = new();
        LoadClients();
      };
    }
    ClientsListBox.UnselectAll();
  }
  
  private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    RefreshData();
  }
  
  private void PaginationComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    RefreshData();
  }

  private void MaxPageSetup(int pageAmount)
  {
    int remainder = clientsAmountAfterFilters % pageAmount;
    if (remainder != 0)
    {
      this.pageAmount = pageAmount;
      maxPage = clientsAmountAfterFilters / pageAmount + 1;
      PageNumberMaxTB.Text = maxPage.ToString();
    }
    else
    {
      this.pageAmount = pageAmount;
      maxPage = clientsAmountAfterFilters / pageAmount;
      PageNumberMaxTB.Text = maxPage.ToString();
    }
  }
  
  private async void ViewVisits(object? sender, RoutedEventArgs e)
  {
    var selectedClient = (sender as Button).Tag;
    ViewVisits viewVisits = new(Convert.ToInt32(selectedClient));
    await viewVisits.ShowDialog(this);
  }
  
  private void CheckBoxSelectionChanged(object sender, RoutedEventArgs e)
  {
    RefreshData();
  }

  private async void OnSearchBoxTextChanging(object? sender, KeyEventArgs e)
  {
    RefreshData();
  }

  private void RefreshData()
  {
    page = 0;
    PageNumberTB.Text = (page + 1).ToString();
    LoadClients();
  }
  
}