using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using GemScopeWPF.Repository;
using GemScopeWPF.UI;
using GemScopeWPF.WebcamFacade;
using System.Text.RegularExpressions;

namespace GemScopeWPF
{
	/// <summary>
	/// Interaction logic for NewStone.xaml
	/// </summary>
	public partial class NewStone : Window
	{
		public string FolderUponCaptureEvent { get; set; }
		public Stone CurrentStone { get; set; }
		public bool EditMode { get; set; }
		public NewStone()
		{
			CurrentStone = null;
			EditMode = false;
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			

			if (CurrentStone != null)
			{
				 foreach (StackPanel panel in this.InfoPartsInputsContainer.Children)
				 {

					 if (panel.Children.OfType<TextBox>().SingleOrDefault() != null)
					 {
						 string tag = (string)panel.Children.OfType<TextBox>().SingleOrDefault().Tag;
						 var value = CurrentStone.InfoList.Where(m => m.Title == tag).SingleOrDefault();
						 if (value != null)
						 {
							 panel.Children.OfType<TextBox>().SingleOrDefault().Text = value.Value;
						 }

					 }
					 else if (panel.Children.OfType<ComboBox>().SingleOrDefault() != null)
					 {
						 string tag = (string)panel.Children.OfType<ComboBox>().SingleOrDefault().Tag;
						 var value = CurrentStone.InfoList.Where(m => m.Title == tag).SingleOrDefault();

						 if (value != null)
						 {
							 ComboBox combo = panel.Children.OfType<ComboBox>().SingleOrDefault();

							 combo.SelectedItem = combo.Items.OfType<ComboBoxItem>().Where(m => (string)m.Tag == value.Value).Single();
							//  = combo.Items.Count - 1;
						   //  combo.Items.OfType<ComboBoxItem>().Where(m => m.Tag == value.Value).Single().IsSelected = true;
						   //  combo.SelectedIndex = combo.Items.IndexOf(combo.Items.OfType<ComboBoxItem>().Where(m => m.Tag == value.Value).Single());
						 }
						


					 }
					 
					// string tag = (string)panel.Children.OfType<TextBox>().SingleOrDefault().Tag;

				   //  var value = CurrentStone.InfoList.Where(m=> m.Title == tag).SingleOrDefault() ;

				   //  if (value != null)
					// {
					//     panel.Children.OfType<TextBox>().SingleOrDefault().Text = value.Value;
//}

					 


				 }

				//load iamge

				 BitmapImage src = new BitmapImage(new Uri(CurrentStone.FullFilePath, UriKind.Absolute));
				 //src.BeginInit();
				 this.CapturedImage.Source = src;
				// src.EndInit();

				 this.Filename.IsEnabled = false;
				 
			}
		}
		private void SaveDiamond_Click(object sender, RoutedEventArgs e)
		{
			StonesRepository rep = new StonesRepository(FolderUponCaptureEvent);
			
			//Create the infoparts from input controls

			
			
			string filename = Path.Combine(FolderUponCaptureEvent, this.Filename.Text);
			filename = Path.ChangeExtension(filename, "jpg");

			if (rep.IsStoneExists(filename) && this.EditMode == false) 
			{
				MessageBoxResult result = MessageBox.Show("The filename already exists, do you want to override the existing file", "File exists", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.No)
				{
					return;
				}
			}

			BitmapSource source = (BitmapSource)this.CapturedImage.Source;

			List<StoneInfoPart> infoparts = new List<StoneInfoPart>();

			foreach (StackPanel panel in this.InfoPartsInputsContainer.Children)
			{
				StoneInfoPart infopart = new StoneInfoPart();
				if (panel.Children.OfType<TextBox>().SingleOrDefault() != null)
				{
					infopart.Title = (string)panel.Children.OfType<TextBox>().SingleOrDefault().Tag;
					infopart.Value = panel.Children.OfType<TextBox>().SingleOrDefault().Text;
                    infopart.TitleForReport = (string)panel.Children.OfType<Label>().SingleOrDefault().Content;
				}
				else if (panel.Children.OfType<ComboBox>().SingleOrDefault() != null) 
				{
					infopart.Title = (string)panel.Children.OfType<ComboBox>().SingleOrDefault().Tag;
					infopart.Value = (string)((ComboBoxItem)panel.Children.OfType<ComboBox>().SingleOrDefault().SelectedItem).Tag;
                    infopart.TitleForReport = (string)panel.Children.OfType<Label>().SingleOrDefault().Content;
				}
				

				infoparts.Add(infopart);

			}

			if (this.ValidateUserInput() == false)
			{
				MessageBox.Show("Filename,Stonetype,Stone Weight,Clarity and color are mendatory fields...");
				return;
			}
			if (this.EditMode)
			{
				rep.UpdateStone(filename, infoparts);
			}
			else
			{
				rep.CreateANewStone(source, filename, infoparts);
			}

			//WebCam webcam = WebCam.GetInstance();
		   // webcam.Start();

			StonesView.RefreshView();

			this.Close();


		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			
			this.Close();
		}

		private bool ValidateUserInput()
		{
			bool isValid = true;

			string filename = this.Filename.Text;
			int istonetype = this.StoneType.SelectedIndex;

			string caratweight = this.CaratWeight.Text;

			int icolor = this.StoneColor.SelectedIndex;
			int iclarity = this.StoneClarity.SelectedIndex;

			//filename required

			if (String.IsNullOrWhiteSpace(filename))
			{
				isValid = false;
			}

			Regex r = new Regex(@"[^/?*:;{}\\]+");
			isValid = r.IsMatch(filename);

			if (istonetype == 1)
			{
				isValid = false;
			}

			r = new Regex(@"^\d{0,8}(\.\d{1,8})?$");
			isValid = r.IsMatch(caratweight);

			if (icolor == 0)
			{
				isValid = false;
			}

			if (iclarity == 0)
			{
				isValid = false;
			}

			return isValid;


		}
	}
}
