//using Blazored.LocalStorage;
using Maschinblick.Shared;
using Maschinblick.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Syncfusion.Blazor;
using Maschinblick.Pages;


//viewing the sql data... wasted to much time getting graphs to work

namespace Maschinblick
{
    class Init  // main thread
    {
        static void Main(string[] args)
        {

            // insert  license here<>

            

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddBlazoredLocalStorage(); // local storage
            builder.Services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);  // local storage
            builder.Services.AddSyncfusionBlazor();

            // data
            //builder.Services.AddSingleton<DeviceData>();
            builder.Services.AddSingleton<RuntimeDataDisplay>(); // for pie charts
            builder.Services.AddSingleton<Datalog_Service>(); // for datalogging

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }

    }

    // Read Client data 
    public class ReadClient
    {
        public TimeFormatting timeFormatting = new();
        public GuiAssist gui = new();

        public async Task Read_Client(ILocalStorageService localStorage)
        {
            //! start date save
            var startdate = await localStorage.GetItemAsStringAsync("StartDate");
            startdate = startdate.Trim('"');
            if (startdate == null)
            {
                Console.WriteLine("Cookie is blank");
                timeFormatting.StartDate = "";
            }
            else
            {
                timeFormatting.StartDate = startdate;
            }

            //! end date save
            var enddate = await localStorage.GetItemAsStringAsync("EndDate");
            enddate = enddate.Trim('"');
            if (enddate == null)
            {
                Console.WriteLine("Cookie is blank");
                timeFormatting.EndDate = "";
            }
            else
            {
                timeFormatting.EndDate = enddate;
            }

            //! Theme save
            var theme = await localStorage.GetItemAsStringAsync("Theme");
            theme = theme.Trim('"');
            if (theme == null)
            {
                Console.WriteLine("Cookie is blank");
                gui.selectedTheme = "";
            }
            else
            {
                gui.selectedTheme = theme;
            }

        }

    }


}
