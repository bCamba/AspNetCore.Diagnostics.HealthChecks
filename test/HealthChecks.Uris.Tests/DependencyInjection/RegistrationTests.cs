using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Xunit;

namespace HealthChecks.Uris.Tests.DependencyInjection
{
    public class uris_registration_should
    {
        [Fact]
        public void add_health_check_when_properly_configured()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddUrlGroup(new Uri("http://httpbin.org/status/200"));

            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("uri-group");
            check.Should().BeOfType<UriHealthCheck>();
        }

        [Fact]
        public void add_named_health_check_when_properly_configured()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddUrlGroup(new Uri("http://httpbin.org/status/200"), name: "my-uri-group");

            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("my-uri-group");
            check.Should().BeOfType<UriHealthCheck>();
        }

        [Fact]
        public void add_health_check_when_configured_through_service_provider()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddUrlGroup(sp => new Uri("http://httpbin.org/status/200"));

            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            check.Should().BeOfType<UriHealthCheck>();
        }
    }
}
