using Blazored.LocalStorage;
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

            //Register Syncfusion community license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt/QHRqVVhkVFpFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF5jS39Qd0ZmXX9ec3ddTg==;Mgo+DSMBPh8sVXJ0S0J+XE9AflRDX3xKf0x/" +
                "TGpQb19xflBPallYVBYiSV9jS31TdERgWHxbdHRVRWVeWQ==;ORg4AjUWIQA/Gnt2VVhkQlFacldJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxQdkdiWn5ddXBVRmNaV0w=;MTEyMTk1NUAzMjMwMmUzNDJlMzBsWU0yait4dHhiNVZaN" +
                "ktLRTYxaEI1eU5vVFFJRXgwZktFWDFpbndab0ZNPQ==;MTEyMTk1NkAzMjMwMmUzNDJlMzBNbTdXY2RBYmZQZlBEZFVPbTNFalA2VG5MTzUydUdKc3NQVTM4TXlFK3ZrPQ==;NRAiBiAaIQQuGjN/V0Z+WE9EaFtKVmJLYVB3WmpQdldgdVRM" +
                "ZVVbQX9PIiBoS35RdUVhWHxfc3FRRmBYVU1+;MTEyMTk1OEAzMjMwMmUzNDJlMzBlWXZhRjN4Y2orREJNdDZXOVlDa2dVQzU0WGRoYmtreTVuWUNib0w4VmNRPQ==;MTEyMTk1OUAzMjMwMmUzNDJlMzBnMWx6Qld6Wnl4Z1BjQ2ZnL25icnNl" +
                "N0s4S0k0USs4UDFZZXRUZzRRWGVvPQ==;Mgo+DSMBMAY9C3t2VVhkQlFacldJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxQdkdiWn5ddXBVRmVeUUw=;MTEyMTk2MUAzMjMwMmUzNDJlMzBnbXNJeUVuZEdqL200RXlHQzhqWVBjb2JKRW0wUD" +
                "MzVU5VNTRJT2IxZ2dNPQ==;MTEyMTk2MkAzMjMwMmUzNDJlMzBwRG1ncmxnL1JVSWNxbzczSlhDaW12U3E5VG1rWnc4aDNQVHVLNWx2WkNvPQ==;MTEyMTk2M0AzMjMwMmUzNDJlMzBlWXZhRjN4Y2orREJNdDZXOVlDa2dVQzU0WGRoYmtreTVuWUN" +
                "ib0w4VmNRPQ==");


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