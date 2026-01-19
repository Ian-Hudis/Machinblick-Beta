using Microsoft.AspNetCore.Components;

namespace Maschinblick.Shared
{
    public class GuiAssist
    {


        //! Theme
            public string selectedTheme = "Normal";// = DataSave.Savedata.Theme;
            public List<string> themes = new () { "Normal", "Printer Friendly" };
            public string BarBackground = "#2d2d2d";
        //!

        //! for menu status
            public string menustatus = "Hide";
            public bool _navBarVisible = true;


        //get additional css class for nav bar div
        public string NavBarClass
        {
            get
            {
                if (_navBarVisible) return string.Empty;//No additional css class for show nav bar
                return "d-none";//d-none class will hide the div
            }
        }
        //! for menu status

        public System.Action? OnChanged { get; set; }

        //Change state by click on the button
        public void Toggle()
        {
            _navBarVisible = !_navBarVisible;//Change

            if (_navBarVisible)
            {
                menustatus = "Hide";
            }
            else
            {
                menustatus = "Show";
            }

            OnChanged?.Invoke();//Callback for reload//Callback for reload
        }

        public bool IsSelectedTheme(string theme)
        {
            return theme == selectedTheme;
        }

        public void ChangingTheme(ChangeEventArgs e)
        {
            #pragma warning disable CS8601 // Dereference of a possibly null reference.
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            selectedTheme = e.Value.ToString();
        }
        //! Theme


    }
}
