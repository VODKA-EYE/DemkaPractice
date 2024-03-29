using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PracticeIK.Context;
using PracticeIK.Models;

namespace PracticeIK;

public partial class MainWindow : Window
{
  private User907Context dbcontext = new();
  private int page = 0;
  private int clientsAmount;

  public MainWindow()
  {
    InitializeComponent();
    GenderComboBox.SelectionChanged += ComboBoxSelectionChanged;
    PaginationComboBox.SelectionChanged += ComboBoxSelectionChanged;
    BDThisMonthCB.Click += CheckBoxSelectionChanged;
    
    LoadGenders();
    LoadClients();
  }

  private void LoadClients()
  {
    List<Client> clients = dbcontext.Clients.ToList();
    
    // Подсчёт общего количества клиентов
    clientsAmount = clients.Count();
    ClientsTotalTB.Text = clientsAmount.ToString();
    
    // Сортировка по гендеру
    if (GenderComboBox.SelectedIndex != 0)
    {
      clients = clients.Where(c => c.Gendercode == GenderComboBox.SelectedIndex).ToList();
    }

    // Сортировка подням рождениям в этом месяце
    if (BDThisMonthCB.IsChecked.Value)
    {
      clients = clients.Where(c => c.Birthday.Value.Month == DateTime.Now.Month).ToList();
    }

    // Поисковая строка
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
    
    // Сортировка по фамилии, последнему посещению и количеству
    //clients = clients.OrderBy(c => c.Lastname).ThenBy(c => c.Lastvisit).ThenBy(c => c.Amountofvisits).ToList();
    clients = clients.OrderBy(c => c.Id).ToList();
    
    // Пагинация клиентов
    switch (PaginationComboBox.SelectedIndex)
    {
      case 0:
        break;
      case 1:
        clients = clients.Skip(page * 10).Take(10).ToList();
        break;
      case 2:
        clients = clients.Skip(page * 50).Take(50).ToList();
        break;
      case 3:
        clients = clients.Skip(page * 100).Take(100).ToList();
        break;
    }
    ClientsShownTB.Text = clients.Count.ToString();
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
    switch (PaginationComboBox.SelectedIndex)
    {
      case 0:
        break;
      case 1:
        if (clientsAmount / 10 != page+1)
        {
          NextPage();
        }
        break;
      case 2:
        if (clientsAmount / 50 != page+1)
        {
          NextPage();
        }
        break;
      case 3:
        if (clientsAmount / 100 != page+1)
        {
          NextPage();
        }
        break;
    }

    void NextPage()
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
        LoadClients();
      };
    }
    ClientsListBox.UnselectAll();
  }
  
  private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    LoadClients();
  }

  private void CheckBoxSelectionChanged(object sender, RoutedEventArgs e)
  {
    LoadClients();
  }
}