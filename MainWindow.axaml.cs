using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using PracticeIK.Context;
using PracticeIK.Models;

namespace PracticeIK;

public partial class MainWindow : Window
{
  private User907Context dbcontext = new();
  public MainWindow()
  {
    InitializeComponent();
    LoadClients();
    LoadGenders();
  }
  
  public void LoadClients()
  {
    List<Client> clients = dbcontext.Clients.ToList();
    ClientsListBox.ItemsSource = clients;
  }
  
  public void LoadGenders()
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
}