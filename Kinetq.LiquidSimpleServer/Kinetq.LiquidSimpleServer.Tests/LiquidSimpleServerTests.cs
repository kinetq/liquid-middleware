using System.Text.RegularExpressions;
using Kinetq.LiquidSimpleServer.Interfaces;
using Kinetq.LiquidSimpleServer.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kinetq.LiquidSimpleServer.Tests
{
    public class LiquidSimpleServerTests : IAsyncLifetime
    {
        private ILiquidSimpleServer _liquidSimpleServer;
        private Mock<ILiquidRoutesManager> _liquidRoutesManagerMock;
        private Mock<IHtmlRenderer> _htmlRendererMock;
        private IFileProvider _embeddedFileProvider;

        public async Task InitializeAsync()
        {
            _liquidRoutesManagerMock = new Mock<ILiquidRoutesManager>();
            _htmlRendererMock = new Mock<IHtmlRenderer>();

            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection
                .AddSingleton(_liquidRoutesManagerMock.Object)
                .AddSingleton(_htmlRendererMock.Object)
                .AddSingleton<ILiquidSimpleServer, LiquidSimpleServer>()
                .AddLogging(builder => builder.AddConsole())
                .BuildServiceProvider();

            _liquidSimpleServer = serviceProvider.GetRequiredService<ILiquidSimpleServer>();
            _embeddedFileProvider = new EmbeddedFileProvider(typeof(LiquidSimpleServerTests).Assembly, "Kinetq.LiquidSimpleServer.Tests.Templates");

            Task.Run(async () => await _liquidSimpleServer.StartAsync());
        }

        public Task DisposeAsync()
        {
            _liquidSimpleServer.Stop();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetHomePageAsync_ShouldReturnRenderedHtml_WhenRouteExists()
        {
            // Arrange
            const string expectedRoute = "/";
            const string expectedRenderedHtml = "<html><body>Welcome to Home Page</body></html>";

            _liquidRoutesManagerMock
                .Setup(x => x.GetRouteForPath(expectedRoute))
                .Returns(new LiquidRoute
                {
                    RoutePattern = new Regex("^/$"),
                    LiquidTemplatePath = "index.liquid",
                    FileProvider = null // Not needed for this test
                });

            _htmlRendererMock
                .Setup(x => x.RenderHtml(It.IsAny<RenderModel>(), It.IsAny<LiquidRoute>()))
                .ReturnsAsync(expectedRenderedHtml);

            // Act
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{_liquidSimpleServer.Prefix}");
            var actualHtml = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(expectedRenderedHtml, actualHtml);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetHomePageAsync_ShouldReturnCSS_WhenRouteExists()
        {
            // Arrange
            const string expectedRoute = "/";

            _liquidRoutesManagerMock
                .Setup(x => x.GetRouteForPath(expectedRoute))
                .Returns(new LiquidRoute
                {
                    RoutePattern = new Regex("^/$"),
                    LiquidTemplatePath = "index.liquid",
                    FileProvider = _embeddedFileProvider // Not needed for this test
                });

            _htmlRendererMock
                .Setup(x => x.RenderHtml(It.IsAny<RenderModel>(), It.IsAny<LiquidRoute>()))
                .ReturnsAsync((string)null);

            // Act
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_liquidSimpleServer.Prefix}styles/styles.css");
            request.Headers.Add("Referer", _liquidSimpleServer.Prefix.ToString());
            var response = await httpClient.SendAsync(request);
            var cssContents = await response.Content.ReadAsStringAsync();

            // Load expected CSS content from embedded file
            var cssFileInfo = _embeddedFileProvider.GetFileInfo("styles/styles.css");
            string expectedCssContent = string.Empty;
            if (cssFileInfo.Exists)
            {
                using var stream = cssFileInfo.CreateReadStream();
                using var reader = new StreamReader(stream);
                expectedCssContent = await reader.ReadToEndAsync();
            }

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(expectedCssContent, cssContents);
        }
    }
}
