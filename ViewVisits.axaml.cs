using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PracticeIK.Context;
using PracticeIK.Models;

namespace PracticeIK;

public partial class ViewVisits : Window
{
  private int _clientId;
  private User907Context dbcontext = new();

  public ViewVisits()
  {
    // For sdk preview
    InitializeComponent();
    LoadVisits();
  }
  public ViewVisits(int clientId)
  {
    InitializeComponent();
    _clientId = clientId;
    LoadVisits();
  }

  private void LoadVisits()
  {
    List<Clientservice> visits = dbcontext.Clientservices.Where(c => c.Clientid == _clientId).ToList();
    if (visits.Count != 0)
    {
      visits = visits.OrderByDescending(v => v.Starttime).ToList();
      VisitsListBox.ItemsSource = visits;
      return;
    }

    VisitsListBox.IsVisible = false;
    NothingLoadedTB.Text = "У клиента не было посещений";
  }
}