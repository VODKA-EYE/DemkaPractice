using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Microsoft.EntityFrameworkCore;
using PracticeIK.Context;
using PracticeIK.Models;
using static System.Net.Mime.MediaTypeNames;

namespace PracticeIK;

public partial class AddClient : Window
{
  private User907Context dbcontext = new();
  private bool editClient;
  string url;
  string imageFormat;
  Client client;
  public AddClient()
  {
    InitializeComponent();
    LoadGenders();
    LoadTags();
    client = new();
    editClient = false;
    DelteButton.IsVisible = false;
    DelteButton.Click += null;
  }

  public AddClient(int clientId)
  {
    InitializeComponent();
    LoadGenders();
    LoadTags();
    editClient = true;
    client = dbcontext.Clients.Find(clientId);
    WindowGrid.DataContext = client;
    BDayCalendar.SelectedDate = DateTime.Parse(client.Birthday.ToString());
    TagsListBox.SelectedItems = client.Tags.ToList();
    Title = "Изменение клиента";
  }

  public void SaveClient(object sender, RoutedEventArgs e)
  {
    try
    {
      bool noErrors = true;

      ClearErrorBackground();
      // Content check
      if (FirstNameTB.Text == null)
      {
        FirstNameTB.Background = Brushes.Red;
        noErrors = false;
      }

      if (LastNameTB.Text == null)
      {
        LastNameTB.Background = Brushes.Red;
        noErrors = false;
      }

      if (PatronymicTB.Text == null)
      {
        PatronymicTB.Background = Brushes.Red;
        noErrors = false;
      }

      var email = new EmailAddressAttribute();
      if (!email.IsValid(EMailTB.Text) || EMailTB.Text == null)
      {
        EMailTB.Background = Brushes.Red;
        noErrors = false;
      }

      string phoneSymbols = "1234567890()+- ";
      bool phoneCorrect = true;
      foreach (var symbol in phoneSymbols)
      {
        if (!phoneSymbols.Contains(symbol.ToString()))
        {
          phoneCorrect = false;
        }
      }
      
      if (!phoneCorrect || PhoneTB.Text == null)
      {
        PhoneTB.Background = Brushes.Red;
        noErrors = false;
      }

      if (GenderComboBox.SelectedIndex == 0)
      {
        GenderComboBox.Background = Brushes.Red;
        noErrors = false;
      }

      if (BDayCalendar.SelectedDate == null)
      {
        BDayCalendar.Background = Brushes.Red;
        noErrors = false;
      }
      
      // If errors marked, not needed to save stuff after that
      if (!noErrors)
      {
        return;
      }

      // Save selected tags to client
      List<Tag> tags = new();
      foreach (Tag item in TagsListBox.SelectedItems)
      {
        tags.Add(item);
      }
      
      // Save parameters
      client.Firstname = FirstNameTB.Text;
      client.Lastname = LastNameTB.Text;
      client.Patronymic = PatronymicTB.Text;
      client.Email = EMailTB.Text;
      client.Phone = PhoneTB.Text;
      client.Gendercode = GenderComboBox.SelectedIndex;
      client.Birthday = DateOnly.FromDateTime(BDayCalendar.SelectedDate.Value);
      client.Registrationdate = DateOnly.FromDateTime(DateTime.Now);
      client.Tags = tags;
      
      if (editClient)
      {
        dbcontext.Clients.Update(client);
        dbcontext.SaveChanges();
      }
      else
      {
        dbcontext.Clients.Add(client);
        dbcontext.SaveChanges();
      }
      
      // Copy selected image and save its path. Takes ID from newly created client
      // No image was selected, and there already exists path
      if (string.IsNullOrEmpty(url) && (client.Photopath != "" || client.Photopath != null))
      {
        // do not change anything
      }
      
      // if selected another image
      else if(url != client.Photopath && url != "")
      {
        try
        {
          File.Delete(client.Photopath);
        }
        catch (Exception exception)
        {
          
        }
        File.Copy(url, @$"./Клиенты/i{client.Id + imageFormat}", true);
        client.Photopath = @"Клиенты\i" + client.Id + imageFormat;
        dbcontext.Clients.Update(client);
        dbcontext.SaveChanges();
      }
      else
      {
        client.Photopath = "";
        dbcontext.Clients.Update(client);
        dbcontext.SaveChanges();
      }

      Close();
    }
    catch (Exception exception)
    {
      ErrorTB.Text = exception.ToString();
    }
  }

  private void ClearErrorBackground()
  {
    FirstNameTB.Background = Brushes.White;

    LastNameTB.Background = Brushes.White;

    PatronymicTB.Background = Brushes.White;

    EMailTB.Background = Brushes.White;

    PhoneTB.Background = Brushes.White;

    GenderComboBox.Background = Brushes.White;

    BDayCalendar.Background = Brushes.White;
  }
  
  private void DeleteClient(object sender, RoutedEventArgs e)
  {
    client.Tags = new List<Tag>();
    dbcontext.Clients.Remove(client);
    dbcontext.SaveChanges();
    Close();
  }

  private void DeselectImage(object sender, RoutedEventArgs e)
  {
    if (client.Photopath != "")
    {
      File.Delete(client.Photopath);
    }
    url = "";
    image.Source = TryLoadSelectedImage(url);
    client.Photopath = "";
  }
  
  private async void SelectImage(object sender, RoutedEventArgs e)
  {
    // Popup menu with selection
    var file = await StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
    {
      Title = "Загрузить картинку",
      AllowMultiple = false,
      FileTypeFilter = new FilePickerFileType[]{new("Элементы"){Patterns = new[]{"*.png", "*.jpg", "*.jpeg", "*.bmp"}}}
    });
    // When image selected, try load it
    if(file.Count >= 1)
    {
      await using var stream = await file[0].OpenReadAsync();
      using var streamReader = new StreamReader(stream);
      string url = (streamReader.BaseStream as FileStream).Name;
      image.Source = TryLoadSelectedImage(url); 
    }
  }

  Bitmap TryLoadSelectedImage(string url)
  {
    Bitmap link;
    try
    {
      try
      {
        link = new(url);
        this.url = url;
        imageFormat = Path.GetExtension(url);
      }
      catch
      {
        link = new(@"service_logo.png");
        this.url = @"service_logo.png";
      }
    }
    catch
    {
      link = null;
      this.url = null;
    }
    return link;
  }

  
  private void LoadGenders()
  {
    List<Gender> genders = dbcontext.Genders.ToList();
    genders.Insert(0, new Gender()
    {
      Code = 0,
      Name = "Выберите пол"
    });
    GenderComboBox.ItemsSource = genders;
    GenderComboBox.SelectedIndex = 0;
  }

  private void LoadTags()
  {
    List<Tag> tags = dbcontext.Tags.ToList();
    TagsListBox.ItemsSource = tags;
  }
}