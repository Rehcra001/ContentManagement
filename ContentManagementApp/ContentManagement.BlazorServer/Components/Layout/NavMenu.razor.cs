using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Antiforgery;

namespace ContentManagement.BlazorServer.Components.Layout
{
    public partial class NavMenu : IDisposable
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public IAntiforgery Antiforgery { get; set; }

        private int ViewportWidth { get; set; }
        private int ViewportHeight { get; set; }

        private string? currentUrl;
        private bool _menuIsOpen = false;
        private const int CHANGE_WIDTH = 992;

        protected override void OnInitialized()
        {
            currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
            StateHasChanged();
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }

        [JSInvokable]
        public void OnResize(int width, int height)
        {
            if (ViewportWidth == width && ViewportHeight == height) return;
            ViewportWidth = width;
            ViewportHeight = height;
            StateHasChanged();
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("window.registerViewportChangeCallback", DotNetObjectReference.Create(this));
                //Explicitly set ViewportWidth and call state has changed to load the window width
                ViewportWidth = await JSRuntime.InvokeAsync<int>("getWidth");
                StateHasChanged();
            }
            
        }

        private void Menu_Click()
        {
            _menuIsOpen = !_menuIsOpen;
        }

    }
}
