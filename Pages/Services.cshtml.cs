using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyFirstSite.Pages
{
    public class ServicesModel : PageModel
    {
        public List<ServiceItem> ServiceList { get; set; }

        public void OnGet()
        {
            // Preload service items
            ServiceList = new List<ServiceItem>
            {
                new ServiceItem
                {
                    Title = "Standard Cleaning",
                    Description = "Perfect for regular upkeep to keep your home looking fresh and tidy.",
                    Image = "/images/standard-cleaning.jpg",
                    Icon = "fas fa-broom"
                },
                new ServiceItem
                {
                    Title = "Deep Cleaning",
                    Description = "Thorough cleaning for hard-to-reach areas and deep grime removal.",
                    Image = "/images/deep-cleaning.jpg",
                    Icon = "fas fa-pump-soap"
                },
                new ServiceItem
                {
                    Title = "Move-In / Move-Out Cleaning",
                    Description = "Make your new or old home spotless and ready for the next chapter.",
                    Image = "/images/move-in-out-cleaning.jpg",
                    Icon = "fas fa-box-open"
                },
                new ServiceItem
                {
                    Title = "Fridge & Oven Cleaning",
                    Description = "We handle those tough-to-clean appliances with care and precision.",
                    Image = "/images/fridge-oven-cleaning.jpg",
                    Icon = "fas fa-utensils"
                },
                new ServiceItem
                {
                    Title = "Custom Cleaning Plans",
                    Description = "Tailored cleaning services to match your unique needs and schedule.",
                    Image = "/images/custom-cleaning.jpg",
                    Icon = "fas fa-clipboard-list"
                }
            };
        }
    }

    // Model to hold service data
    public class ServiceItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Icon { get; set; }
    }
}
