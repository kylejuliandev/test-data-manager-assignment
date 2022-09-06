using Manager.Web.Data;
using Manager.Web.Data.Models;
using Manager.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using System.Security.Claims;

namespace Manager.Web.UnitTests;

public class SchemeServiceUnitTests : IDisposable
{
    private readonly string _testUserId = "bda1bc6d-9fdd-42e8-ae38-9bfec57b75da";

    private IHttpContextAccessor _httpContextAccessor = default!;
    private ApplicationDbContext _database = default!;

    [SetUp]
    public async Task SetUp_Defaults()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, _testUserId)
        });
        var user = new ClaimsPrincipal(identity);

        context.User = user;
        _httpContextAccessor.HttpContext = context;

        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(nameof(SchemeServiceUnitTests));

        _database = new ApplicationDbContext(dbContextOptions.Options, _httpContextAccessor);
        await SetupTestDefaults();
    }

    [TearDown]
    public void TearDown_InMemory_Database()
    {
        _database.Users.Clear();
        _database.Schemes.Clear();
        _database.SchemeData.Clear();

        _database.SaveChangesAsync();
    }

	[Test]
	public async Task Get_Paginated_Schemes_Returns_Collection()
	{
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

		var result = await schemeService.GetSchemesAsync(0, 10);

        Assert.That(result.Items, Is.Not.Empty);
	}

    [Test]
    public async Task Get_Paginated_Schemes_With_More_Than_PageSize_Returns_Collection_And_HasMore()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);
        
        _database.Add(CreateScheme("Test title 1"));
        _database.Add(CreateScheme("Test title 2"));
        _database.Add(CreateScheme("Test title 3"));
        _database.Add(CreateScheme("Test title 4"));
        _database.Add(CreateScheme("Test title 5"));
        await _database.SaveChangesAsync();

        // Page size = 2
        // With 6 items, we'd expect 3 pages
        var result = await schemeService.GetSchemesAsync(0, 2);

        Assert.That(result.Items, Is.Not.Empty);
        Assert.That(result.Items.Length, Is.EqualTo(2));
        Assert.That(result.HasMore, Is.True);
    }

    [Test]
    public async Task Get_Paginated_Schemes_With_Page_Less_Than_0_Returns_ManagedError()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

        _database.Schemes.Clear();
        _database.SchemeData.Clear();
        await _database.SaveChangesAsync();

        var result = await schemeService.GetSchemesAsync(-1, 2);

        Assert.That(result.Items, Is.Empty);
        Assert.That(result.HasMore, Is.False);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("You must specify a valid page and pageSize"));
    }

    [Test]
    public async Task Get_Paginated_Schemes_With_PageSize_Less_Than_0_Returns_ManagedError()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

        _database.Schemes.Clear();
        _database.SchemeData.Clear();
        await _database.SaveChangesAsync();

        var result = await schemeService.GetSchemesAsync(0, -1);

        Assert.That(result.Items, Is.Empty);
        Assert.That(result.HasMore, Is.False);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("You must specify a valid page and pageSize"));
    }

    [Test]
    public async Task Get_Scheme_Returns_Scheme()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

        var result = await schemeService.GetSchemeAsync(_database.Schemes.First().Id);

        Assert.That(result.Item, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    public async Task Get_Scheme_With_Random_Guid_Returns_ManagedError()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

        var result = await schemeService.GetSchemeAsync(Guid.NewGuid());

        Assert.That(result.Item, Is.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("Scheme not found"));
    }

    [Test]
    public async Task Create_Scheme_Returns_Scheme()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

        var result = await schemeService.CreateSchemeAsync("My new test scheme!");

        Assert.That(result.Item, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    public async Task Create_Scheme_As_Not_Authenticated_User_Returns_No_Scheme()
    {
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext();

        httpContextAccessor.HttpContext = context;

        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(nameof(SchemeServiceUnitTests));

        using var database = new ApplicationDbContext(dbContextOptions.Options, httpContextAccessor);

        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, database);

        var result = await schemeService.CreateSchemeAsync("My new test scheme!");

        Assert.That(result.Item, Is.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("Unable to save Scheme to the database"));
    }

    [Test]
    public async Task Create_Scheme_As_Authenticated_But_Missing_Claims_User_Returns_No_Scheme()
    {
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        context.User = user;
        httpContextAccessor.HttpContext = context;

        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(nameof(SchemeServiceUnitTests));

        using var database = new ApplicationDbContext(dbContextOptions.Options, httpContextAccessor);

        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, database);

        var result = await schemeService.CreateSchemeAsync("My new test scheme!");

        Assert.That(result.Item, Is.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("Unable to save Scheme to the database"));
    }

    [Test]
    public async Task Create_Scheme_As_Authenticated_With_Claims_Id_Not_Found_Returns_No_Scheme()
    {
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "unknown-claim-id")
        });
        var user = new ClaimsPrincipal(identity);

        context.User = user;
        httpContextAccessor.HttpContext = context;

        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(nameof(SchemeServiceUnitTests));

        using var database = new ApplicationDbContext(dbContextOptions.Options, httpContextAccessor);

        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, database);

        var result = await schemeService.CreateSchemeAsync("My new test scheme!");

        Assert.That(result.Item, Is.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("Unable to save Scheme to the database"));
    }

    [Test]
    public async Task Update_Scheme_Returns_Scheme()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

        var result = await schemeService.UpdateSchemeAsync(_database.Schemes.First().Id, "My new test scheme!");

        Assert.That(result.Item, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    public async Task Update_Scheme_With_Scheme_Not_Found_Returns_ManagedError()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

        var result = await schemeService.UpdateSchemeAsync(Guid.NewGuid(), "My new test scheme!");

        Assert.That(result.Item, Is.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("You cannot update this scheme"));
    }

    [Test]
    public async Task Update_Scheme_As_Not_Authenticated_User_Returns_No_Scheme()
    {
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext();

        httpContextAccessor.HttpContext = context;

        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(nameof(SchemeServiceUnitTests));

        using var database = new ApplicationDbContext(dbContextOptions.Options, httpContextAccessor);

        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, database);

        var result = await schemeService.UpdateSchemeAsync(database.Schemes.First().Id, "My new test scheme!");

        Assert.That(result.Item, Is.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("Unable to update Scheme in the database"));
    }

    [Test]
    public async Task Update_Scheme_As_Authenticated_But_Missing_Claims_User_Returns_No_Scheme()
    {
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        context.User = user;
        httpContextAccessor.HttpContext = context;

        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(nameof(SchemeServiceUnitTests));

        using var database = new ApplicationDbContext(dbContextOptions.Options, httpContextAccessor);

        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, database);

        var result = await schemeService.UpdateSchemeAsync(database.Schemes.First().Id, "My new test scheme!");

        Assert.That(result.Item, Is.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("Unable to update Scheme in the database"));
    }

    [Test]
    public async Task Update_Scheme_As_Authenticated_With_Claims_Id_Not_Found_Returns_No_Scheme()
    {
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "unknown-claim-id")
        });
        var user = new ClaimsPrincipal(identity);

        context.User = user;
        httpContextAccessor.HttpContext = context;

        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(nameof(SchemeServiceUnitTests));

        using var database = new ApplicationDbContext(dbContextOptions.Options, httpContextAccessor);

        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, database);

        var result = await schemeService.UpdateSchemeAsync(database.Schemes.First().Id, "My new test scheme!");

        Assert.That(result.Item, Is.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("Unable to update Scheme in the database"));
    }

    [Test]
    public async Task Create_SchemeData_Returns_Scheme()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

        var result = await schemeService.CreateSchemeDataAsync(_database.Schemes.First().Id, "test_data_key", "{ \"jsonProperty\": \"value\" }");

        Assert.That(result.Item, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    public async Task Create_SchemeData_With_Scheme_Not_Found_Returns_ManagedError()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

        var result = await schemeService.CreateSchemeDataAsync(Guid.NewGuid(), "test_data_key", "{ \"jsonProperty\": \"value\" }");

        Assert.That(result.Item, Is.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("You cannot add scheme data to this scheme"));
    }

    [Test]
    public async Task Create_SchemeData_With_DuplicateKey_Returns_ManagedError()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

        var result = await schemeService.CreateSchemeDataAsync(_database.Schemes.First().Id, _database.SchemeData.First().Key, "{ \"jsonProperty\": \"value\" }");

        Assert.That(result.Item, Is.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("The key field must be unique"));
    }

    [Test]
    public async Task Create_SchemeData_As_Not_Authenticated_User_Returns_No_Scheme()
    {
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext();

        httpContextAccessor.HttpContext = context;

        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(nameof(SchemeServiceUnitTests));

        using var database = new ApplicationDbContext(dbContextOptions.Options, httpContextAccessor);

        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, database);

        var result = await schemeService.CreateSchemeDataAsync(database.Schemes.First().Id, "test_data_key", "{ \"jsonProperty\": \"value\" }");

        Assert.That(result.Item, Is.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("Unable to create Scheme data in database"));
    }

    [Test]
    public async Task Create_SchemeData_As_Authenticated_But_Missing_Claims_User_Returns_No_Scheme()
    {
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        context.User = user;
        httpContextAccessor.HttpContext = context;

        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(nameof(SchemeServiceUnitTests));

        using var database = new ApplicationDbContext(dbContextOptions.Options, httpContextAccessor);

        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, database);

        var result = await schemeService.CreateSchemeDataAsync(database.Schemes.First().Id, "test_data_key", "{ \"jsonProperty\": \"value\" }");

        Assert.That(result.Item, Is.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("Unable to create Scheme data in database"));
    }

    [Test]
    public async Task Create_SchemeData_As_Authenticated_With_Claims_Id_Not_Found_Returns_No_Scheme()
    {
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "unknown-claim-id")
        });
        var user = new ClaimsPrincipal(identity);

        context.User = user;
        httpContextAccessor.HttpContext = context;

        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(nameof(SchemeServiceUnitTests));

        using var database = new ApplicationDbContext(dbContextOptions.Options, httpContextAccessor);

        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, database);

        var result = await schemeService.CreateSchemeDataAsync(database.Schemes.First().Id, "test_data_key", "{ \"jsonProperty\": \"value\" }");

        Assert.That(result.Item, Is.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("Unable to create Scheme data in database"));
    }

    [Test]
    public async Task Delete_SchemeData_Returns_True()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

        var result = await schemeService.RemoveSchemeDataAsync(_database.Schemes.First().Id, _database.SchemeData.First().Id);

        Assert.That(result.Item, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    public async Task Delete_SchemeData_Not_Found_Returns_False()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

        var result = await schemeService.RemoveSchemeDataAsync(Guid.NewGuid(), _database.SchemeData.First().Id);

        Assert.That(result.Item, Is.False);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("You cannot remove this scheme data"));
    }

    [Test]
    public async Task Delete_Scheme_Returns_True()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

        var result = await schemeService.RemoveSchemeAsync(_database.Schemes.First().Id);

        Assert.That(result.Item, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    public async Task Delete_Scheme_Not_Found_Returns_False()
    {
        var schemeService = new SchemeService(NullLogger<SchemeService>.Instance, _database);

        var result = await schemeService.RemoveSchemeAsync(Guid.NewGuid());

        Assert.That(result.Item, Is.False);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors[0].Message, Is.EqualTo("You cannot remove this scheme"));
    }

    private async Task SetupTestDefaults()
    {
        var superUser = new IdentityUser
        {
            Id = _testUserId,
            UserName = "superuser",
            NormalizedUserName = "SUPERUSER",
            Email = "superuser@testdatamanager.dev",
            NormalizedEmail = "superuser@testdatamanager.dev",
            EmailConfirmed = true,
            SecurityStamp = string.Empty
        };

        _database.Add(superUser);
        
        var scheme = CreateScheme("My Test scheme!");
        _database.Add(scheme);
        
        var schemeData = CreateSchemeData(scheme, "hello_world", "<xml></xml>");
        _database.Add(schemeData);

        await _database.SaveChangesAsync();
    }

    private static Scheme CreateScheme(string title) => new()
    {
        Id = Guid.NewGuid(),
        Title = title
    };

    private static SchemeData CreateSchemeData(Scheme scheme, string key, string value) => new()
    {
        Id = Guid.NewGuid(),
        Key = key,
        Value = value,
        Scheme = scheme,
        SchemeId = scheme.Id
    };

    public void Dispose()
    {
        _database.Dispose();
    }
}
