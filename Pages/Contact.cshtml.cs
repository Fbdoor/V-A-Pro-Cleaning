using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Text;
using System.Threading.Tasks;


namespace MyFirstSite.Pages
{
    public class ContactModel : PageModel
    {
        [BindProperty] public string CleaningType { get; set; } // Deep / Basic
        [BindProperty] public string CleaningArea { get; set; } // FullHouse / SpecificArea
        [BindProperty] public int? SquareFootage { get; set; }
        [BindProperty] public int? Rooms { get; set; }
        [BindProperty] public int? Bathrooms { get; set; }
        [BindProperty] public string Description { get; set; }
        [BindProperty] public string[] Services { get; set; }
        [BindProperty] public DateTime? PreferredDate { get; set; }
        [BindProperty] public string TimeFrom { get; set; }
        [BindProperty] public string TimeTo { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Phone { get; set; }
        [BindProperty] public bool HasPets { get; set; }
        [BindProperty] public string SpecialInstructions { get; set; }

        public string StatusMessage { get; set; }
        public bool IsSuccess { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPost()
        {
            // === Validation ===
            if (string.IsNullOrWhiteSpace(CleaningType))
            {
                StatusMessage = "Please select a cleaning type (Deep or Basic).";
                IsSuccess = false;
                return Page();
            }

            if (string.IsNullOrWhiteSpace(CleaningArea))
            {
                StatusMessage = "Please select the area to clean.";
                IsSuccess = false;
                return Page();
            }

            if (CleaningArea == "FullHouse" && (!SquareFootage.HasValue || !Rooms.HasValue || !Bathrooms.HasValue))
            {
                StatusMessage = "Please complete all Full House Cleaning fields.";
                IsSuccess = false;
                return Page();
            }

            if (CleaningArea == "SpecificArea" && string.IsNullOrWhiteSpace(Description))
            {
                StatusMessage = "Please describe the area you want cleaned.";
                IsSuccess = false;
                return Page();
            }

            if (!PreferredDate.HasValue || string.IsNullOrWhiteSpace(TimeFrom) || string.IsNullOrWhiteSpace(TimeTo))
            {
                StatusMessage = "Please provide a preferred date and time range.";
                IsSuccess = false;
                return Page();
            }

            try
            {
                // Format services list
                string servicesList = (Services != null && Services.Length > 0)
                    ? string.Join(", ", Services)
                    : "None";

                // === Build the email body in HTML format ===
                var bodyBuilder = new StringBuilder();
                bodyBuilder.Append("<html><body style='font-family:Arial, sans-serif; line-height:1.6;'>");
                bodyBuilder.Append("<h2 style='color:#6d4a4b;'>New Cleaning Quote Request</h2>");
                bodyBuilder.Append("<p>You have received a new cleaning quote request. Below are the details:</p>");
                bodyBuilder.Append("<table style='width:100%; border-collapse:collapse;'>");

                bodyBuilder.AppendFormat("<tr><td><strong>Cleaning Type:</strong></td><td>{0}</td></tr>", CleaningType);
                bodyBuilder.AppendFormat("<tr><td><strong>Area to Clean:</strong></td><td>{0}</td></tr>", 
                    CleaningArea == "FullHouse" ? "Full House" : "Specific Area");
                bodyBuilder.AppendFormat("<tr><td><strong>Square Footage:</strong></td><td>{0}</td></tr>", SquareFootage?.ToString() ?? "N/A");
                bodyBuilder.AppendFormat("<tr><td><strong>Number of Rooms:</strong></td><td>{0}</td></tr>", Rooms?.ToString() ?? "N/A");
                bodyBuilder.AppendFormat("<tr><td><strong>Number of Bathrooms:</strong></td><td>{0}</td></tr>", Bathrooms?.ToString() ?? "N/A");
                bodyBuilder.AppendFormat("<tr><td><strong>Description:</strong></td><td>{0}</td></tr>", !string.IsNullOrEmpty(Description) ? Description : "N/A");
                bodyBuilder.AppendFormat("<tr><td><strong>Additional Services:</strong></td><td>{0}</td></tr>", servicesList);
                bodyBuilder.AppendFormat("<tr><td><strong>Preferred Date:</strong></td><td>{0}</td></tr>", PreferredDate?.ToString("MM/dd/yyyy"));
                bodyBuilder.AppendFormat("<tr><td><strong>Preferred Time:</strong></td><td>{0} - {1}</td></tr>", TimeFrom, TimeTo);
                bodyBuilder.AppendFormat("<tr><td><strong>Contact Email:</strong></td><td>{0}</td></tr>", Email);
                bodyBuilder.AppendFormat("<tr><td><strong>Phone Number:</strong></td><td>{0}</td></tr>", Phone);
                bodyBuilder.AppendFormat("<tr><td><strong>Has Pets:</strong></td><td>{0}</td></tr>", HasPets ? "Yes" : "No");
                bodyBuilder.AppendFormat("<tr><td><strong>Special Instructions:</strong></td><td>{0}</td></tr>",
                    !string.IsNullOrWhiteSpace(SpecialInstructions) ? SpecialInstructions : "None");

                bodyBuilder.Append("</table>");
                bodyBuilder.Append("<br><p style='font-size:12px; color:#666;'>This message was sent automatically from your website form.</p>");
                bodyBuilder.Append("</body></html>");

                // === Send the email via SendGrid ===
                // === Send the email via Resend ===
                using var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add(
                    "Authorization",
                    $"Bearer {Environment.GetEnvironmentVariable("RESEND_API_KEY")}"
                );

                var subject = $"New Quote Request - {PreferredDate?.ToString("MM/dd/yyyy")} {DateTime.Now:hh:mm tt}";

                var payload = new
                {
                    from = "VNA Pro Cleaning <onboarding@resend.dev>",
                    to = new[] { "vnaproestimates@gmail.com" },
                    subject = subject,
                    html = bodyBuilder.ToString()
                };

                var response = await httpClient.PostAsJsonAsync(
                    "https://api.resend.com/emails",
                    payload
                );

                if (response.IsSuccessStatusCode)
                {
                    StatusMessage = "Your request has been sent successfully!";
                    IsSuccess = true;
                    ModelState.Clear();
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    StatusMessage = $"Error sending email: {response.StatusCode} - {errorBody}";
                    IsSuccess = false;
                }

            }
            catch (Exception ex)
            {
                // === Error handling ===
                StatusMessage = $"Error sending email: {ex.Message}";
                IsSuccess = false;
            }

            return Page();
        }
    }
}
