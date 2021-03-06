using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using fidoCore.Services.SettingsServices;
using fidoBackend.Models;
using Windows.UI.Xaml.Controls;
using fidoBackend.Services;

namespace fidoCore.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public List<Tasks> myTasks { get; set; }
        public string organisationname { get; set; }
        public string address { get; set; }
        public Tasks selectedTask { get; set; }

        public void ClickItemList(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                var obj = new Temp1();
                obj.task = ((Tasks)e.ClickedItem);
                NavigationService.Navigate(typeof(Views.ProjectManagement.AddTask), obj);
            }
        }
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }
            Services.SettingsServices.SettingsService.Instance.ShowHamburgerButton = true;
        }

        string _Value = "Gas";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                Value = suspensionState[nameof(Value)]?.ToString();
            }
            await Task.CompletedTask;

            var sett = Services.SettingsServices.SettingsService.Instance.UserId;
            organisationname = Services.SettingsServices.SettingsService.Instance.OrganisationName;
            address = Services.SettingsServices.SettingsService.Instance.OrganisationAddress;
            RaisePropertyChanged("organisationname");
            RaisePropertyChanged("address");
            Views.Busy.SetBusy(true, "Loading all Assigned Tasks for You");
            var assigned = await ProjectServices.ListTasks(sett);
            Views.Busy.SetBusy(false);
            if (assigned.result)
            {
                if (assigned.data != null)
                {
                    myTasks = assigned.data as List<Tasks>;
                    RaisePropertyChanged("myTasks");
                }
            }
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void GotoDetailsPage() =>
            NavigationService.Navigate(typeof(Views.DetailPage), Value);
        public void Logout()
        {
            var settings = SettingsService.Instance;
            settings.UserId = string.Empty;
            settings.OrganisationId = string.Empty;
            NavigationService.Navigate(typeof(Views.WelcomePage), 0);
            NavigationService.ClearHistory();
        }


        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

    }
}

