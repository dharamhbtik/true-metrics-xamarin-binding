using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Android.Views;
using Android.Widget;
using TrueMetricsDemo.ViewModels;

namespace TrueMetricsDemo.Droid
{
    // This class is never actually executed, but when Xamarin linking is enabled it does 
    // ensure types and properties are preserved in the deployed app
    public class LinkerPleaseInclude
    {
        public void Include(Button button)
        {
            button.Click += (s, e) => button.Text = button.Text + "";
        }

        public void Include(CheckBox checkBox)
        {
            checkBox.CheckedChange += (sender, args) => checkBox.Checked = !checkBox.Checked;
        }

        public void Include(Switch @switch)
        {
            @switch.CheckedChange += (sender, args) => @switch.Checked = !@switch.Checked;
        }

        public void Include(View view)
        {
            view.Click += (s, e) => view.ContentDescription = view.ContentDescription + "";
        }

        public void Include(TextView text)
        {
            text.TextChanged += (sender, args) => text.Text = "" + text.Text;
            text.Hint = "" + text.Hint;
        }

        public void Include(CompoundButton cb)
        {
            cb.CheckedChange += (sender, args) => cb.Checked = !cb.Checked;
        }

        public void Include(SeekBar sb)
        {
            sb.ProgressChanged += (sender, args) => sb.Progress = sb.Progress + 1;
        }

        public void Include(Activity act)
        {
            act.Title = act.Title + "";
        }

        public void Include(INotifyCollectionChanged changed)
        {
            changed.CollectionChanged += (s, e) => { var test = string.Format("{0}{1}{2}{3}{4}", e.Action, e.NewItems, e.NewStartingIndex, e.OldItems, e.OldStartingIndex); };
        }

        public void Include(ICommand command)
        {
            command.CanExecuteChanged += (s, e) => { if (command.CanExecute(null)) command.Execute(null); };
        }

        public void Include(System.Windows.Input.ICommand command)
        {
            command.CanExecuteChanged += (s, e) => { if (command.CanExecute(null)) command.Execute(null); };
        }

        public void Include(MainPageViewModel vm)
        {
            // Ensure ViewModel is included
            vm.ApiKey = vm.ApiKey + "";
            vm.UserId = vm.UserId + "";
            vm.EventName = vm.EventName + "";
            vm.ScreenName = vm.ScreenName + "";
            vm.PropertyKey = vm.PropertyKey + "";
            vm.PropertyValue = vm.PropertyValue + "";
            vm.ExerciseType = vm.ExerciseType + "";
            vm.IsInitialized = !vm.IsInitialized;
            vm.StatusMessage = vm.StatusMessage + "";
            vm.IsBusy = !vm.IsBusy;
        }
    }
}
