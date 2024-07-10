using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace PwdManager.spa.Shared
{
    public partial class FaIconSelect
    {
        public string iconString { get; set; } = "";
        [Parameter]
        public EventCallback<string> OnIconClicked { get; set; }

        public bool FA5 { get; set; } = false;
        protected string? rawString { get; set; } = "";
        StringBuilder rawStringBuilder = new StringBuilder("");
        protected MatchCollection? matches { get; set; }
        protected List<string> icons { get; set; } = new List<string>();
        protected List<string> ficons { get; set; } = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_configuration.GetValue<string>("PwdManager.spa") ?? throw new ArgumentException());
            var result = await client.GetStreamAsync("/css/fa.css");
            using (StreamReader readtext = new StreamReader(result))
            {
                while (!readtext.EndOfStream)
                {

                    rawStringBuilder.Append("\r " + readtext.ReadLine());

                }
            }
            string regexexpr = @"fa-[^:]+(?=:before)";

            matches = Regex.Matches(rawStringBuilder.ToString(), @regexexpr);
            
            foreach (var m in matches)
            {
                if (!string.IsNullOrEmpty(m.ToString()))
                {
                 icons.Add(m.ToString());

                }
            }

            ficons = icons.ToList();


            await InvokeAsync(StateHasChanged);
        }
        protected async Task InputChanged(string searchWord)
        {
            ficons = icons.Where(x => x.Contains(searchWord)).ToList();
 
            await InvokeAsync(StateHasChanged);

        }

        public async Task GetIconString(string icon)
        {
            iconString = icon;
            await OnIconClicked.InvokeAsync(iconString);            
        }
    }
}
