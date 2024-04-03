using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;

namespace PracticeIK.Models;

public partial class Client
{
  public int Id { get; set; }

  public string Firstname { get; set; } = null!;

  public string Lastname { get; set; } = null!;

  public string? Patronymic { get; set; }

  public DateOnly? Birthday { get; set; }

  public DateOnly Registrationdate { get; set; }

  public string? Email { get; set; }

  public string Phone { get; set; } = null!;

  public int Gendercode { get; set; }

  public string? Photopath { get; set; }

  public int Amountofvisits { get; set; }

  public DateOnly? Lastvisit { get; set; }

  public virtual ICollection<Clientservice> Clientservices { get; set; } = new List<Clientservice>();

  public virtual Gender GendercodeNavigation { get; set; } = null!;

  public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

  public Bitmap Photo
  {
    get
    {
      Bitmap image;
      try
      {
        try
        {
          if (Photopath == "")
          {
            image = new Bitmap(@"service_logo.exe");
            return image;
          }
          image = new Bitmap(Photopath);
          return image;
        }
        catch (Exception e)
        {
          image = new Bitmap(@"service_logo.exe");
          return image;
        }
      }
      catch (Exception e)
      {
        return null;
      }
    }
  }
}