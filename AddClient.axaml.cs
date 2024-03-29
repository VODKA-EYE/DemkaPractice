using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
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
  }

  public void SaveClient(object sender, RoutedEventArgs e)
  {
    try
    {
      
      
      if (client.Photopath == @"service_logo.png" || url == @"service_logo.png")
      {
        File.Delete(@$"./Клиенты/{client.Id + imageFormat}");
        client.Photopath = "";
      }
      else if (client.Photopath != @"service_logo.png" || url != @"service_logo.png")
      {
        if (client.Photopath == "")
        {
          File.Copy(url, @$"./Клиенты/{client.Id + imageFormat}", true);
          client.Photopath = client.Id + imageFormat;
        }
      }
      else
      {
        client.Photopath = "";
      }


      client.Firstname = FirstNameTB.Text;
      client.Lastname = LastNameTB.Text;
      client.Patronymic = PatronymicTB.Text;
      client.Email = EMailTB.Text;
      client.Gendercode = GenderComboBox.SelectedIndex;
      client.Birthday = DateOnly.FromDateTime(BDayCalendar.SelectedDate.Value);
      client.Registrationdate = DateOnly.FromDateTime(DateTime.Now);
    }
    catch (Exception exception)
    {
      ErrorTB.Text = exception.ToString();
    }
  }
  
  private async void SelectImage(object sender, RoutedEventArgs e)
  {
    var file = await StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
    {
      Title = "Загрузить картинку",
      AllowMultiple = false,
      FileTypeFilter = new FilePickerFileType[]{new("Элементы"){Patterns = new[]{"*.png", "*.jpg", "*.jpeg", "*.bmp"}}}
    });
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